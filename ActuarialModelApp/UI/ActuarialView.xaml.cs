using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using ActuarialModelApp.Core;
using ScottPlot;
using ScottPlot.Statistics;

namespace ActuarialModelApp.UI
{
    public partial class ActuarialView : UserControl
    {
        public ActuarialView()
        {
            InitializeComponent();
        }

        private async void Run_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            double lambda = double.Parse(TxtLambda.Text);
            double mu = double.Parse(TxtMu.Text);
            double sigma = double.Parse(TxtSigma.Text);
            int sims = int.Parse(TxtSimulations.Text);

            var losses = await Task.Run(() =>
            {
                var freq = ActuarialEngine.GeneratePoisson(lambda, sims);
                return ActuarialEngine.CollectiveRiskModel(freq, mu, sigma);
            });

            double mean = losses.Average();
            double var99 = ActuarialEngine.CalculateVaR(losses, 0.99);

            TxtOutput.Text =
                $"MEAN LOSS : {mean:C0}\n" +
                $"VaR 99%   : {var99:C0}";

            PlotAggregate.Plot.Clear();

            // ✅ ScottPlot 5 correct histogram usage
            double min = losses.Min();
            double max = losses.Max();

            var hist = new Histogram(min, max, binCount: 50);
            hist.AddRange(losses);

            PlotAggregate.Plot.Add.Bars(hist.BinCenters, hist.Counts);
            PlotAggregate.Plot.Title("Aggregate Loss Distribution");

            PlotAggregate.Refresh();
        }
    }
}
