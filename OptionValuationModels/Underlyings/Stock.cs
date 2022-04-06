
namespace OptionValuationModels.Underlyings
{
    /// <summary>
    ///     A stock with a current price and volatility.
    /// </summary>
    public class Stock
    {
        #region Constructors

        /// <summary>
        ///     Creates a stock with the specified attributes.
        /// </summary>
        /// <param name="currentPrice">The current price of the stock.</param>
        /// <param name="volatility">The volatility of the stock.</param>
        public Stock(double currentPrice, double volatility)
        {
            // Require that the current price be greater than zero.
            if (currentPrice <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(currentPrice));
            }

            // Require that the volatility be greater than zero.
            if (volatility <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(volatility));
            }

            CurrentPrice = currentPrice;
            Volatility = volatility;
        }

        #endregion

        #region Properties

        public double CurrentPrice { get; set; }

        public double Volatility { get; private set; }

        #endregion
    }
}
