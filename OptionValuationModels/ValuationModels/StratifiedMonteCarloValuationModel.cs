using MathNet.Numerics.Distributions;
using OptionValuationModels.Options;
using OptionValuationModels.Results;
using OptionValuationModels.Underlyings;

namespace OptionValuationModels.ValuationModels
{
    /// <summary>
    ///     Extends the base Monte Carlo valuation class to incorporate stratification as a variance reduction technique.
    /// </summary>
    public class StratifiedMonteCarloValuationModel : MonteCarloValuationModel
    {
        #region Constructors

        /// <summary>
        ///     Creates a model that values options using the Monte Carlo method with stratification.
        /// </summary>
        /// <param name="numberOfBins">The number of bins to use in the stratification.</param>
        /// <param name="drawsPerBin">The number of draws to make for each bin.</param>
        public StratifiedMonteCarloValuationModel(int numberOfBins, int drawsPerBin) : base(numberOfBins * drawsPerBin)
        {
            // Require that the number of bins be greater than one so that the standard error can be calculated.
            if (numberOfBins <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfBins));
            }

            NumberOfBins = numberOfBins;
            DrawsPerBin = drawsPerBin;
        }

        #endregion

        #region Properties

        public int NumberOfBins { get; private set; }

        public int DrawsPerBin { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Calculates the standard error of stratified values.
        /// </summary>
        /// <param name="values">A matrix of the stratified values.</param>
        /// <param name="valuesAverage">The average of the values along the rows.</param>
        /// <returns>The standard error of the stratified values.</returns>
        private double CalculateStandardError(double[,] values, double[] valuesAverage)
        {
            double squaredDeviationsSum;
            double varianceSum = 0;

            // Iterate through each bin.
            for (int binIndex = 0; binIndex < NumberOfBins; binIndex++)
            {
                squaredDeviationsSum = 0;

                // Iterate through each draw in the current bin.
                for (int drawIndex = 0; drawIndex < DrawsPerBin; drawIndex++)
                {
                    squaredDeviationsSum += Math.Pow(values[binIndex, drawIndex] - valuesAverage[binIndex], 2);
                }

                varianceSum += squaredDeviationsSum / (DrawsPerBin - 1);
            }

            // Calculate the sample standard deviation as the square root of the average variance of each bin.
            double sampleStandardDeviation = Math.Sqrt(varianceSum / NumberOfBins);

            double standardError = sampleStandardDeviation / Math.Sqrt(_numberOfTrials);

            return standardError;
        }

        /// <summary>
        ///     Creates a matrix of stratified draws from the standard normal distribution using a probability integral transform.
        /// </summary>
        /// <returns>The matrix of stratified draws.</returns>
        private double[,] GetStratifiedRandomDraws()
        {
            double[,] standardNormalDraws = new double[NumberOfBins, DrawsPerBin];
            double[] tempUniformDraws = new double[DrawsPerBin];

            double lowerBound = 0;
            double upperBound;

            // Iterate through each bin.
            for(int binIndex = 0; binIndex < NumberOfBins; binIndex++)
            {
                // Calculate the upper bound.
                upperBound = lowerBound + 1.0 / NumberOfBins;

                // Draw from a continuous uniform distribution defined by the upper and lower bounds.
                ContinuousUniform.Samples(tempUniformDraws, lowerBound, upperBound);

                // Iterate through each draw in the current bin.
                for(int drawIndex = 0; drawIndex < DrawsPerBin; drawIndex++)
                {
                    // Perform a probability integral transform using the inverse CDF of the standard normal distribution.
                    standardNormalDraws[binIndex, drawIndex] = Normal.InvCDF(0, 1, tempUniformDraws[drawIndex]);
                }

                // The current upper bound becomes the lower bound on the next interation.
                lowerBound = upperBound;
            }

            return standardNormalDraws;
        }

        /// <summary>
        ///     Values a European style option using stratification.
        /// </summary>
        /// <param name="option">The option to value.</param>
        /// <param name="stock">The underlying stock.</param>
        /// <param name="interestRate">The prevailing annual risk-free interest rate.</param>
        /// <returns>The result containing the price and standard error.</returns>
        public override Result Value(Option option, Stock stock, double interestRate)
        {
            double[,] standardNormalDraws = GetStratifiedRandomDraws();

            double[,] optionPrices = new double[NumberOfBins, DrawsPerBin];
            double[] optionPriceAverages = new double[NumberOfBins];
            double endingStockPrice;
            double discountFactor = Math.Exp(-interestRate * option.TimeToMaturity);
            double optionPricesSum = 0;

            // Iterate through each bin.
            for (int binIndex = 0; binIndex < NumberOfBins; binIndex++)
            {
                // Iterate through each draw in the current bin.
                for (int drawIndex = 0; drawIndex < DrawsPerBin; drawIndex++)
                {
                    endingStockPrice = ApplyGeometricBrownianMotion(stock, option.TimeToMaturity, interestRate, standardNormalDraws[binIndex, drawIndex]);
                    optionPrices[binIndex, drawIndex] = discountFactor * option.Payoff(endingStockPrice);
                    optionPriceAverages[binIndex] += optionPrices[binIndex, drawIndex];
                }

                optionPriceAverages[binIndex] /= DrawsPerBin;

                optionPricesSum += optionPriceAverages[binIndex];
            }

            double optionPrice = optionPricesSum / NumberOfBins;

            double standardError = CalculateStandardError(optionPrices, optionPriceAverages);

            EstimationResult result = new EstimationResult(optionPrice, standardError);

            return result;

        }

        #endregion
    }
}
