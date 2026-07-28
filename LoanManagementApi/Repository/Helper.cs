namespace LoanManagementApi.Repository
{
    public class Helper:IHelper
    {
        public decimal CalculateEmi(double principal, double annualInterestRate, int tenureInMonths)
        {
            // 1. Calculate monthly interest rate (r) from annual rate
            double monthlyRate = annualInterestRate / (12 * 100);

            // 2. Handle 0% interest rate edge case to avoid division by zero
            if (monthlyRate == 0)
            {
                return (decimal)Math.Round(principal / tenureInMonths, 2);
            }

            // 3. Compute (1 + r)^n
            double compoundFactor = Math.Pow(Convert.ToDouble(1 + monthlyRate), tenureInMonths);

            // 4. Apply the standard EMI formula
            double emi = principal * monthlyRate * compoundFactor / (compoundFactor - 1);

            // 5. Return the result rounded to standard currency decimal places
            return (decimal)Math.Round(emi, 2);
        }
    }
}
