namespace ActuarialModelApp.Core
{
    public static class PricingEngine
    {
        public static double CalculateGrossPremium(
            double expectedFrequency,
            double expectedSeverity,
            double expenseRatio,
            double profitMargin,
            double riskLoading)
        {
            double purePremium = expectedFrequency * expectedSeverity;
            return purePremium * (1 + expenseRatio + profitMargin + riskLoading);
        }
    }
}
