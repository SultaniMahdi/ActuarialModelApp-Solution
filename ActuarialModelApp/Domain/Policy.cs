namespace ActuarialModelApp.Domain
{
    public class Policy
    {
        public Guid PolicyId { get; set; } = Guid.NewGuid();
        public string PolicyNumber { get; set; } = "";
        public string InsuredName { get; set; } = "";

        public DateTime InceptionDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        public double SumInsured { get; set; }
        public double AnnualPremium { get; set; }

        public PolicyStatus Status { get; set; } = PolicyStatus.Active;
    }

    public enum PolicyStatus
    {
        Quote,
        Active,
        Suspended,
        Cancelled,
        Expired
    }
}
