
namespace OptionValuationModels.Options
{
    /// <summary>
    ///     The base option class that holds the attributes and behavior common to all options.
    /// </summary>
    public abstract class Option
    {
        #region Constructors

        /// <summary>
        ///     Constructs an option with the specified attributes.
        /// </summary>
        /// <param name="optionType">The type of option.</param>
        /// <param name="strikePrice">The strike price of the option.</param>
        /// <param name="timeToMaturity">The time to maturity of the option in years.</param>
        public Option(OptionType optionType, double strikePrice, double timeToMaturity)
        {
            // Require that the strike price be greater than zero.
            if (strikePrice <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(strikePrice));
            }

            // Require that the time to maturity be greater than zero.
            if (timeToMaturity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(timeToMaturity));
            }

            OptionType = optionType;
            StrikePrice = strikePrice;
            TimeToMaturity = timeToMaturity;
        }

        #endregion

        #region Properties

        public OptionType OptionType { get; private set; }

        public double StrikePrice { get; private set; }

        public double TimeToMaturity { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///     The payoff function of the option.
        /// </summary>
        /// <param name="underlyingPrice">The price of the underlying security.</param>
        /// <returns>The intrinsic value of the option.</returns>
        public abstract double Payoff(double underlyingPrice);

        /// <summary>
        ///     Returns the various attributes of the option.
        /// </summary>
        /// <returns>The attributes that describe the option.</returns>
        public override string ToString()
        {
            return $"{GetType().Name} | Option type: {OptionType} | Strike price: {StrikePrice} | Time to maturity: {TimeToMaturity}";
        }

        #endregion
    }
}
