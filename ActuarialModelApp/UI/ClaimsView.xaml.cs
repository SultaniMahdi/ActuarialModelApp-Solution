using System.Windows.Controls;
using ActuarialModelApp.Domain;
using ActuarialModelApp.Services;

namespace ActuarialModelApp.UI
{
    public partial class ClaimsView : UserControl
    {
        private readonly ClaimsService _claimsService = new();

        public ClaimsView()
        {
            InitializeComponent();
        }

        private void RegisterClaim_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _claimsService.RegisterClaim(new Claim
            {
                PolicyNumber = "POL-DEMO",
                LossDate = DateTime.Today.AddDays(-5),
                ReportDate = DateTime.Today,
                ReportedAmount = 15_000,
                ReservedAmount = 12_000,
                PaidAmount = 3_000
            });

            ClaimsGrid.ItemsSource = null;
            ClaimsGrid.ItemsSource = _claimsService.GetClaimsByPolicy("POL-DEMO");
        }
    }
}
