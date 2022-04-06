using OptionValuationModels.Options;
using OptionValuationModels.Results;
using OptionValuationModels.Underlyings;

namespace OptionValuationModels.ValuationModels
{
    /// <summary>
    ///     The interfact for an object that can value an option.
    /// </summary>
    public interface IValuationModel
    {
        #region Methods

        /// <summary>
        ///     Calculates a price for the specified option.
        /// </summary>
        /// <param name="option">The option to calculate a price for.</param>
        /// <param name="stock">The underlying of the option.</param>
        /// <param name="interestRate">The prevailing annual risk-free interest rate.</param>
        /// <returns>The result of the valuation.</returns>
        Result Value(Option option, Stock stock, double interestRate);

        #endregion
    }
}