using MathNet.Numerics.Distributions;
using OptionValuationModels.Underlyings;

namespace OptionValuationModels.Options
{
    /// <summary>
    ///     An option class that implements a binary (or digital) payoff.
    /// </summary>
    public class BinaryOption : Option, IAnalyticalSolution
    {
        #region Constructors

        /// <summary>
        ///     Constructs a binary option with the specified attributes.
        /// </summary>
        /// <param name="optionType">The type of option.</param>
        /// <param name="strikePrice">The strike price of the option.</param>
        /// <param name="timeToMaturity">The time to maturity of the option in years.</param>
        public BinaryOption(OptionType optionType, double strikePrice, double timeToMaturity)
            : base(optionType, strikePrice, timeToMaturity)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Calculates the price of a binary equity option using the analytical solution to the Black-Scholes partial differential equation.
        /// </summary>
        /// <param name="stock">The underlying stock.</param>
        /// <param name="interestRate">The prevailing annual risk-free interest rate.</param>
        /// <returns>The price of the option.</returns>
        public double CalculatePrice(Stock stock, double interestRate)
        {
            double d1Numerator = Math.Log(stock.CurrentPrice / StrikePrice) + (interestRate + Math.Pow(stock.Volatility, 2) / 2) * TimeToMaturity;
            double d1Denominator = stock.Volatility * Math.Sqrt(TimeToMaturity);
            double d1 = d1Numerator / d1Denominator;

            double d2 = d1 - stock.Volatility * Math.Sqrt(TimeToMaturity);

            double discountFactor = Math.Exp(-interestRate * TimeToMaturity);

            int optionTypeDirection = OptionType == OptionType.Call ? 1 : -1;

            double price = discountFactor * Normal.CDF(0, 1, optionTypeDirection * d2);

            return price;
        }

        /// <summary>
        ///     Calculates the payoff of the option using the binary payoff function.
        /// </summary>
        /// <param name="underlyingPrice">The price of the underlying security.</param>
        /// <returns>The intrinsic value of the option.</returns>
        public override double Payoff(double underlyingPrice)
        {
            bool inTheMoney = (OptionType == OptionType.Call && underlyingPrice > StrikePrice)
                                || (OptionType == OptionType.Put && underlyingPrice < StrikePrice);

            double intrinsicValue = inTheMoney ? 1 : 0;

            return intrinsicValue;
        }

        #endregion
    }
}
