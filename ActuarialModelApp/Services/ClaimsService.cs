using ActuarialModelApp.Domain;

namespace ActuarialModelApp.Services
{
    public class ClaimsService
    {
        private readonly List<Claim> _claims = new();

        public void RegisterClaim(Claim claim)
        {
            _claims.Add(claim);
        }

        public IEnumerable<Claim> GetClaimsByPolicy(string policyNumber)
        {
            return _claims.Where(c => c.PolicyNumber == policyNumber);
        }
    }
}
