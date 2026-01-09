using System.Windows.Controls;
using ActuarialModelApp.Domain;
using ActuarialModelApp.Services;

namespace ActuarialModelApp.UI
{
    public partial class PolicyView : UserControl
    {
        private readonly PolicyService _policyService = new();

        public PolicyView()
        {
            InitializeComponent();
            Refresh();
        }

        private void AddPolicy_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _policyService.AddPolicy(new Policy
            {
                PolicyNumber = $"POL-{DateTime.Now.Ticks}",
                InsuredName = "Sample Client",
                InceptionDate = DateTime.Today,
                ExpiryDate = DateTime.Today.AddYears(1),
                SumInsured = 100_000,
                AnnualPremium = 1_200
            });

            Refresh();
        }

        private void Refresh()
        {
            PolicyGrid.ItemsSource = null;
            PolicyGrid.ItemsSource = _policyService.GetAllPolicies();
        }
    }
}
