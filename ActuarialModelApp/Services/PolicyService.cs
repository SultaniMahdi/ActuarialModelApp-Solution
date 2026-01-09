using ActuarialModelApp.Domain;

namespace ActuarialModelApp.Services
{
    public class PolicyService
    {
        private readonly List<Policy> _policies = new();

        public void AddPolicy(Policy policy)
        {
            _policies.Add(policy);
        }

        public IEnumerable<Policy> GetAllPolicies()
        {
            return _policies;
        }

        public Policy? GetPolicy(string policyNumber)
        {
            return _policies.FirstOrDefault(p => p.PolicyNumber == policyNumber);
        }
    }
}
