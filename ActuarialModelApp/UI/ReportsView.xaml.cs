using System.Windows.Controls;
using ActuarialModelApp.Domain;
using ActuarialModelApp.Services;

namespace ActuarialModelApp.UI
{
    public partial class ReportsView : UserControl
    {
        private readonly ReportingService _reportingService = new();

        public ReportsView()
        {
            InitializeComponent();
        }

        private void GenerateReport_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var claims = new[]
            {
                new Claim { PaidAmount = 5_000, ReservedAmount = 10_000 },
                new Claim { PaidAmount = 2_000, ReservedAmount = 6_000 }
            };

            ReportBox.Text =
                $"TOTAL PAID: {_reportingService.TotalPaidClaims(claims):C}\n" +
                $"OUTSTANDING RESERVES: {_reportingService.OutstandingReserves(claims):C}";
        }
    }
}
