using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace ActuarialModelApp
{
    public class ActuarialEngine
    {
        // 1. Schadenhäufigkeitsverteilungen (Frequency)
        public static int[] GeneratePoisson(double lambda, int n)
        {
            int[] results = new int[n];

            // OPTIMIZATION: Use Gaussian approximation for large Lambda to prevent infinite loops.
            // The Knuth method (multiplication) underflows when lambda > ~30.
            if (lambda > 30)
            {
                Parallel.For(0, n, i =>
                {
                    // Normal approx: N(lambda, sqrt(lambda))
                    // Continuity correction is optional but helpful for int conversion
                    double val = lambda + Math.Sqrt(lambda) * Random.Shared.NextGaussian();
                    results[i] = Math.Max(0, (int)Math.Round(val));
                });
            }
            else
            {
                double L = Math.Exp(-lambda);
                Parallel.For(0, n, i =>
                {
                    int k = 0;
                    double p = 1.0;
                    do
                    {
                        k++;
                        p *= Random.Shared.NextDouble();
                    } while (p > L && k < 10000); // Safety cap
                    results[i] = k - 1;
                });
            }
            return results;
        }

        // 2. Schadenhöhenverteilungen (Severity)
        public static double[] GenerateLognormal(double mu, double sigma, int n)
        {
            double[] results = new double[n];
            // Parallelize simulation for speed
            Parallel.For(0, n, i =>
            {
                results[i] = Math.Exp(mu + sigma * Random.Shared.NextGaussian());
            });
            return results;
        }

        public static double[] GenerateGamma(double shape, double scale, int n)
        {
            double[] results = new double[n];
            Parallel.For(0, n, i =>
            {
                var rng = Random.Shared; 
                if (shape < 1)
                {
                    // Johnk's generator
                    double u, v, x, y;
                    do
                    {
                        u = rng.NextDouble();
                        v = rng.NextDouble();
                        x = Math.Pow(u, 1.0 / shape);
                        y = Math.Pow(v, 1.0 / (1.0 - shape));
                    } while (x + y > 1);
                    results[i] = scale * x / (x + y) * (-Math.Log(rng.NextDouble()));
                }
                else
                {
                    // Marsaglia-Tseng
                    double d = shape - 1.0 / 3.0;
                    double c = 1.0 / Math.Sqrt(9.0 * d);
                    double v, z;
                    while (true)
                    {
                        do
                        {
                            z = rng.NextGaussian();
                            v = 1.0 + c * z;
                        } while (v <= 0);

                        v = v * v * v;
                        double u = rng.NextDouble();
                        if (u < 1.0 - 0.0331 * z * z * z * z)
                        {
                            results[i] = scale * d * v;
                            break;
                        }
                        if (Math.Log(u) < 0.5 * z * z + d * (1.0 - v + Math.Log(v)))
                        {
                            results[i] = scale * d * v;
                            break;
                        }
                    }
                }
            });
            return results;
        }

        public static double[] GeneratePareto(double scale, double shape, int n)
        {
            double[] results = new double[n];
            Parallel.For(0, n, i =>
            {
                // Inverse Transform Sampling: x_m / (U)^(1/alpha)
                results[i] = scale / Math.Pow(Random.Shared.NextDouble(), 1.0 / shape);
            });
            return results;
        }

        // 3. Kollektivmodell (Gesamtschaden / Aggregate Loss)
        public static double[] CollectiveRiskModel(int[] frequencies, double mu, double sigma)
        {
            double[] aggregateLosses = new double[frequencies.Length];
            
            // Partitioner helps balance load better than standard Parallel.For when loop bodies vary in work
            Parallel.ForEach(Partitioner.Create(0, frequencies.Length), range =>
            {
                var rng = Random.Shared;
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    double yearlyTotal = 0;
                    int count = frequencies[i];
                    for (int j = 0; j < count; j++)
                    {
                        yearlyTotal += Math.Exp(mu + sigma * rng.NextGaussian());
                    }
                    aggregateLosses[i] = yearlyTotal;
                }
            });
            return aggregateLosses;
        }

        // 4. Risikokapital (VaR / TVaR)
        public static double CalculateVaR(double[] losses, double confidenceLevel)
        {
            if (losses == null || losses.Length == 0) return 0;
            
            // Optimization: Clone and Array.Sort is faster than LINQ OrderBy
            double[] sorted = (double[])losses.Clone(); 
            Array.Sort(sorted);
            
            int index = (int)Math.Floor(confidenceLevel * sorted.Length);
            return sorted[Math.Clamp(index, 0, sorted.Length - 1)];
        }

        public static double CalculateTVaR(double[] losses, double confidenceLevel)
        {
            if (losses == null || losses.Length == 0) return 0;
            
            double[] sorted = (double[])losses.Clone();
            Array.Sort(sorted);
            
            int startIndex = (int)Math.Floor(confidenceLevel * sorted.Length);
            
            // Manual sum is faster than LINQ for simple types
            double sum = 0;
            int count = 0;
            for (int i = startIndex; i < sorted.Length; i++)
            {
                sum += sorted[i];
                count++;
            }
            
            return count > 0 ? sum / count : sorted[^1];
        }

        // 5. Statistische Kennzahlen
        public static double CalculateStdDev(double[] values)
        {
            if (values == null || values.Length < 2) return 0;
            
            double mean = values.Average();
            double sumSqDiff = 0;
            foreach(var v in values)
            {
                sumSqDiff += (v - mean) * (v - mean);
            }
            return Math.Sqrt(sumSqDiff / (values.Length - 1));
        }
    }

    public class PremiumResult
    {
        public double RiskPremium { get; set; }
        public double SafetyMargin { get; set; }
        public double GrossPremium { get; set; }
    }

    public static class RandomExtensions
    {
        // Using Random.Shared eliminates the need to pass a Random instance, 
        // but if you want to keep the extension method style:
        public static double NextGaussian(this Random rand, double mean = 0, double stdDev = 1)
        {
            // Box-Muller Transformation
            // Note: We do not cache the second value here to ensure thread safety 
            // without complex locking mechanisms.
            double u1 = 1.0 - rand.NextDouble();
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + stdDev * randStdNormal;
        }
    }
}