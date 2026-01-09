namespace ActuarialModelApp.Core
{
    public static class ReservingEngine
    {
        public static double BestEstimate(double[] incurred)
        {
            return incurred.Average();
        }

        public static double RiskMargin(double[] incurred)
        {
            return 1.96 * ActuarialEngine.CalculateStdDev(incurred);
        }
    }
}
