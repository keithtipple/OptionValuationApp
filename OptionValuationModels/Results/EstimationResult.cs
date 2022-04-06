
namespace OptionValuationModels.Results
{
    /// <summary>
    ///     Extends the base Result class to include a standard error property.
    /// </summary>
    public class EstimationResult : Result
    {
        #region Constructors

        /// <summary>
        ///     Creates a result that has a standard error associated with it.
        /// </summary>
        /// <param name="value">The value of the result.</param>
        /// <param name="standardError">The standard error associated with the specified value.</param>
        public EstimationResult(double value, double standardError) : base(value)
        {
            // Require that the standard error be greater than or equal to zero.
            if (standardError < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(standardError));
            }

            StandardError = standardError;
        }

        #endregion

        #region Properties

        public double StandardError { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Concatenates the standard error to the value.
        /// </summary>
        /// <returns>The value and standard error.</returns>
        public override string ToString()
        {
            return $"{base.ToString()} | Standard Error: {StandardError: 0.00000}";
        }

        #endregion
    }
}
