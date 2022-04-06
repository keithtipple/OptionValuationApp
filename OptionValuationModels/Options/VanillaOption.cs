using MathNet.Numerics.Distributions;
using OptionValuationModels.Underlyings;

namespace OptionValuationModels.Options
{
    /// <summary>
    ///     An option class that implements a vanilla option payoff.
    /// </summary>
    public class VanillaOption : Option, IAnalyticalSolution
    {
        #region Constructors

        /// <summary>
        ///     Constructs a vanilla option with the specified attributes.
        /// </summary>
        /// <param name="optionType">The type of option.</param>
        /// <param name="strikePrice">The strike price of the option.</param>
        /// <param name="timeToMaturity">The time to maturity of the option in years.</param>
        public VanillaOption(OptionType optionType, double strikePrice, double timeToMaturity)
            : base(optionType, strikePrice, timeToMaturity)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Calculates the price of a vanilla equity option using the analytical solution to the Black-Scholes partial differential equation.
        /// </summary>
        /// <param name="stock">The underlying stock.</param>
        /// <param name="interestRate">The prevailing annual risk-free interest rate.</param>
        /// <returns>The price of the option.</returns>
        public double CalculatePrice(Stock stock, double interestRate)
        {
            double d1Numerator = Math.Log(stock.CurrentPrice / StrikePrice) + (interestRate + Math.Pow(stock.Volatility, 2) / 2) * TimeToMaturity;
            double d1Denominator = stock.Volatility * Math.Pow(TimeToMaturity, 0.5);
            double d1 = d1Numerator / d1Denominator;

            double d2 = d1 - stock.Volatility * TimeToMaturity;

            double discountFactor = Math.Exp(-interestRate * TimeToMaturity);

            double price = OptionType == OptionType.Call
                            ? Normal.CDF(0, 1, d1) * stock.CurrentPrice - Normal.CDF(0, 1, d2) * StrikePrice * discountFactor
                            : Normal.CDF(0, 1, -d2) * StrikePrice * discountFactor - Normal.CDF(0, 1, -d1) * stock.CurrentPrice;
            return price;
        }

        /// <summary>
        ///     Calculates the payoff of the option using the vanilla payoff function.
        /// </summary>
        /// <param name="underlyingPrice">The price of the underlying security.</param>
        /// <returns>The intrinsic value of the option.</returns>
        public override double Payoff(double underlyingPrice)
        {
            int payoffDirection = OptionType == OptionType.Call ? 1 : -1;

            double intrinsicValue = Math.Max(payoffDirection * (underlyingPrice - StrikePrice), 0);

            return intrinsicValue;
        }

        #endregion
    }
}
