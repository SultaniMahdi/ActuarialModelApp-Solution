using ActuarialModelApp.Domain;

namespace ActuarialModelApp.Services
{
    public class ReportingService
    {
        public double TotalPaidClaims(IEnumerable<Claim> claims)
        {
            return claims.Sum(c => c.PaidAmount);
        }

        public double OutstandingReserves(IEnumerable<Claim> claims)
        {
            return claims.Sum(c => c.ReservedAmount - c.PaidAmount);
        }
    }
}
