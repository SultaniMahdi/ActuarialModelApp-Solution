namespace ActuarialModelApp.Domain
{
    public class Claim
    {
        public Guid ClaimId { get; set; } = Guid.NewGuid();
        public string PolicyNumber { get; set; } = "";

        public DateTime LossDate { get; set; }
        public DateTime ReportDate { get; set; }

        public double ReportedAmount { get; set; }
        public double ReservedAmount { get; set; }
        public double PaidAmount { get; set; }

        public ClaimStatus Status { get; set; } = ClaimStatus.Open;
    }

    public enum ClaimStatus
    {
        Open,
        Closed,
        Reopened
    }
}
