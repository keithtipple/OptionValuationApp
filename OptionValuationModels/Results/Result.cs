
namespace OptionValuationModels.Results
{
    /// <summary>
    ///     The result of a valuation model.
    /// </summary>
    public class Result
    {
        #region Constructors

        /// <summary>
        ///     Creates a result with the provided value.
        /// </summary>
        /// <param name="value">The result of the valuation.</param>
        public Result(double value)
        {
            // Require that the value be greater than or equal to zero.
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Value = value;
        }

        #endregion

        #region Properties

        public double Value { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Returns the value as a string.
        /// </summary>
        /// <returns>The value of the result.</returns>
        public override string ToString()
        {
            return $"Price: {Value:0.00000}";
        }

        #endregion
    }
}
