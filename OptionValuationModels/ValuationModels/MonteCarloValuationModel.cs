using MathNet.Numerics.Distributions;
using OptionValuationModels.Options;
using OptionValuationModels.Results;
using OptionValuationModels.Underlyings;

namespace OptionValuationModels.ValuationModels
{
    /// <summary>
    ///     Uses Monte Carlo simulation to price a European option.
    /// </summary>
    public class MonteCarloValuationModel : IValuationModel
    {
        #region Fields

        /// <summary>
        ///     The number of trials to run.
        /// </summary>
        protected readonly int _numberOfTrials;

        #endregion

        #region Constructors

        /// <summary>
        ///     Creates a model that values options using the Monte Carlo method.
        /// </summary>
        /// <param name="numberOfTrials">The number of trials used in the simulation.</param>
        public MonteCarloValuationModel(int numberOfTrials)
        {
            // Require that the number of trials be greater than one so that the standard error can be calculated.
            if(numberOfTrials <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfTrials));
            }

            _numberOfTrials = numberOfTrials;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Calculates the standard error of a set of values.
        /// </summary>
        /// <param name="values">The values to calculate the standard error for.</param>
        /// <param name="valuesAverage">The average of the values.</param>
        /// <returns>The standard error of the specified values.</returns>
        protected virtual double CalculateStandardError(double[] values, double valuesAverage)
        {
            double squaredDeviationsSum = 0;

            // Iterate over each random draw.
            for (int i = 0; i < _numberOfTrials; i++)
            {
                squaredDeviationsSum += Math.Pow(values[i] - valuesAverage, 2);
            }

            double sampleStandardDeviation = Math.Sqrt(squaredDeviationsSum / (_numberOfTrials - 1));

            double standardError = sampleStandardDeviation / Math.Sqrt(_numberOfTrials);

            return standardError;
        }

        /// <summary>
        ///     Values a European style option.
        /// </summary>
        /// <param name="option">The option to value.</param>
        /// <param name="stock">The underlying stock.</param>
        /// <param name="interestRate">The prevailing annual risk-free interest rate.</param>
        /// <returns>The result containing the price and standard error.</returns>
        public virtual Result Value(Option option, Stock stock, double interestRate)
        {
            double[] standardNormalDraws = new double[_numberOfTrials];
            Normal.Samples(standardNormalDraws, 0, 1);

            double discountFactor = Math.Exp(-interestRate * option.TimeToMaturity);

            double[] optionPrices = new double[_numberOfTrials];
            double endingStockPrice;
            double optionPricesSum = 0;

            // Iterate over each random draw.
            for (int i = 0; i < _numberOfTrials; i++)
            {
                endingStockPrice = ApplyGeometricBrownianMotion(stock, option.TimeToMaturity, interestRate, standardNormalDraws[i]);
                optionPrices[i] = discountFactor * option.Payoff(endingStockPrice);
                optionPricesSum += optionPrices[i];
            }

            double optionPrice = optionPricesSum / _numberOfTrials;

            double standardError = CalculateStandardError(optionPrices, optionPrice);

            EstimationResult result = new EstimationResult(optionPrice, standardError);

            return result;
        }

        #endregion

        #region Static

        /// <summary>
        ///     Calculates the new price of a stock under geometric brownian motion.
        /// </summary>
        /// <param name="stock">The stock that follows a geometric brownian motion.</param>
        /// <param name="timePeriod">The time period over which the geometric brownian motion is applied.</param>
        /// <param name="interestRate">The prevailing annual risk-free interest rate.</param>
        /// <param name="standardNormalDraw">A random draw from a standard normal distribution.</param>
        /// <returns>The new price of the stock.</returns>
        protected static double ApplyGeometricBrownianMotion(Stock stock, double timePeriod, double interestRate, double standardNormalDraw)
        {
            double logNormalReturn = Math.Exp(interestRate - 0.5 * Math.Pow(stock.Volatility, 2) * timePeriod + stock.Volatility * Math.Sqrt(timePeriod) * standardNormalDraw);

            double newStockPrice = stock.CurrentPrice * logNormalReturn;

            return newStockPrice;
        }

        #endregion
    }
}
