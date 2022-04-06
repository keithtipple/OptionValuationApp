using OptionValuationModels.Underlyings;

namespace OptionValuationModels.Options
{
    /// <summary>
    ///     An interface that represents the ability to calculate the price of an equity option using an analytical solution.
    /// </summary>
    public interface IAnalyticalSolution
    {
        #region Methods

        /// <summary>
        ///     Calculates the price of an equity option using an analytical solution.
        /// </summary>
        /// <param name="stock">The underlying stock.</param>
        /// <param name="interestRate">The prevailing annual risk-free interest rate.</param>
        /// <returns>The price of the equity option using an anlytical solution.</returns>
        double CalculatePrice(Stock stock, double interestRate);

        #endregion
    }
}
