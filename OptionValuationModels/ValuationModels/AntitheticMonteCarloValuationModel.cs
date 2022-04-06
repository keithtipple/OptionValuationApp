using MathNet.Numerics.Distributions;
using OptionValuationModels.Options;
using OptionValuationModels.Results;
using OptionValuationModels.Underlyings;

namespace OptionValuationModels.ValuationModels
{
    /// <summary>
    ///     Extends the base Monte Carlo valuation class to incorporate antithetic variables as a variance reduction technique.
    /// </summary>
    public class AntitheticMonteCarloValuationModel : MonteCarloValuationModel
    {
        #region Constructors

        /// <summary>
        ///     Creates a model that values options using the Monte Carlo method with antithetic variables.
        /// </summary>
        /// <param name="numberOfTrials">The number of trials used in the simulation.</param>
        public AntitheticMonteCarloValuationModel(int numberOfTrials) : base(numberOfTrials)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Values a Eurpean style option using antithetic variables.
        /// </summary>
        /// <param name="option">The option to value.</param>
        /// <param name="stock">The underlying stock.</param>
        /// <param name="interestRate">The prevailing annual risk-free interest rate.</param>
        /// <returns>The result containing the price and standard error.</returns>
        public override Result Value(Option option, Stock stock, double interestRate)
        {
            double[] standardNormalDraws = new double[_numberOfTrials];
            Normal.Samples(standardNormalDraws, 0, 1);

            double discountFactor = Math.Exp(-interestRate * option.TimeToMaturity);

            double endingStockPrice;
            double antitheticEndingStockPrice;
            double optionPrice;
            double antitheticOptionPrice;
            double[] averagedOptionPrices = new double[_numberOfTrials];
            double averagedOptionPricesSum = 0;

            // Iterate over each random draw.
            for (int i = 0; i < _numberOfTrials; i++)
            {
                // Calculate the new stock price in the standard way for a Monte Carlo simulation.
                endingStockPrice = ApplyGeometricBrownianMotion(stock, option.TimeToMaturity, interestRate, standardNormalDraws[i]);
                optionPrice = discountFactor * option.Payoff(endingStockPrice);

                // Calculate the new stock price using the antithetic variable
                antitheticEndingStockPrice = ApplyGeometricBrownianMotion(stock, option.TimeToMaturity, interestRate, -standardNormalDraws[i]);
                antitheticOptionPrice = discountFactor * option.Payoff(antitheticEndingStockPrice);

                // Average the two resulting option prices.
                averagedOptionPrices[i] = (optionPrice + antitheticOptionPrice) / 2;

                averagedOptionPricesSum += averagedOptionPrices[i];
            }

            double averagedOptionPrice = averagedOptionPricesSum / _numberOfTrials;

            double standardError = CalculateStandardError(averagedOptionPrices, averagedOptionPrice);

            EstimationResult result = new EstimationResult(averagedOptionPrice, standardError);

            return result;
        }

        #endregion
    }
}
