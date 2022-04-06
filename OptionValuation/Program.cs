using OptionValuationModels;
using OptionValuationModels.Options;
using OptionValuationModels.Results;
using OptionValuationModels.ValuationModels;
using OptionValuationModels.Underlyings;

// The underlying stock of the options that will be valued.
Stock stock = new Stock(currentPrice: 100, volatility: 0.1);

double strikePrice = 100;
double timeToMaturity = 1;

// Create a list of different types of options to value;
List<Option> options = new List<Option>();
options.Add(new VanillaOption(OptionType.Call, strikePrice, timeToMaturity));
options.Add(new VanillaOption(OptionType.Put, strikePrice, timeToMaturity));
options.Add(new BinaryOption(OptionType.Call, strikePrice, timeToMaturity));
options.Add(new BinaryOption(OptionType.Put, strikePrice, timeToMaturity));

// The number of bins to be used for stratified Monte Carlo simulation.
int numberOfBins = 1000;
// The number of draws per bin to be used for stratified Monte Carlo simulation.
int drawsPerBin = 10;
// The number of trials to run in the monte carlo simulation.
int numberOfTrials = numberOfBins * drawsPerBin;

// Create a list of option valuation models.
List<IValuationModel> models = new List<IValuationModel>();

// Add a Monte Carlo valuation model.
models.Add(new MonteCarloValuationModel(numberOfTrials));

// Add a Monte Carlo valuation model that uses antithetic variables.
// Note: Since valuation with antithetic variables requires double the computation, use half the number of trials.
models.Add(new AntitheticMonteCarloValuationModel(numberOfTrials / 2));

// Add a Monte Carlo valuation model that uses stratification.
models.Add(new StratifiedMonteCarloValuationModel(numberOfBins, drawsPerBin));

double annualRiskFreeRate = 0.05;

string analyticalSolutionLabel = "Analytical solution";
int maxNumberOfCharacters = analyticalSolutionLabel.Length;
int TAB_CHAR_LENGTH = 8;

// Iterate through each valuation model to get the maximum number of characters used for each model label.
foreach (IValuationModel model in models)
{
    maxNumberOfCharacters = Math.Max(maxNumberOfCharacters, model.GetType().Name.Length);
}
// Calculate the number of characters needed for the results of all models to line up.
maxNumberOfCharacters += (TAB_CHAR_LENGTH - (maxNumberOfCharacters / TAB_CHAR_LENGTH));

// Iterate through each option.
foreach (Option option in options)
{
    // Print the current option.
    Console.WriteLine($"Valuing {option}");

    // If the option has an analytical solution for calculating its price, pretty print it to the console for comparison purposes.
    if (option is IAnalyticalSolution analyticalSolution)
    {
        string tabs = new string('\t', (maxNumberOfCharacters - analyticalSolutionLabel.Length) / TAB_CHAR_LENGTH + 1);
        Console.WriteLine($"\t{analyticalSolutionLabel}{tabs}Price: {analyticalSolution.CalculatePrice(stock, annualRiskFreeRate):0.00000}");
    }

    // Iterate through each valuation model.
    foreach (IValuationModel model in models)
    {
        // Value the option.
        Result result = model.Value(option, stock, annualRiskFreeRate);

        // Pretty print the result.
        string modelName = model.GetType().Name;
        string tabs = new string('\t', (maxNumberOfCharacters - modelName.Length) / TAB_CHAR_LENGTH + 1);
        Console.WriteLine($"\t{modelName}{tabs}{result}");
    }
    Console.WriteLine("\n");
}


