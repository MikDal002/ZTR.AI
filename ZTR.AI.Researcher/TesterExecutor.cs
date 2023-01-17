using Microsoft.Extensions.Logging;

namespace ZT.AI.Researcher;

public class TesterExecutor<T> where T : IGlobalOptions
{
    private readonly ITester<T> _tester;

    private readonly ILogger<TesterExecutor<T>> _logger;

    public TesterExecutor(ITester<T> tester, ILogger<TesterExecutor<T>> logger)
    {
        _tester = tester;
        _logger = logger;
    }

    public void RunSuite(T options)
    {
        _logger.LogInformation("Start suite...");
        List<TestFunction> functionsToTest;
        if (options.TestFunction == TestFunction.All)
        {
            functionsToTest = Enum.GetValues<TestFunction>().Where(d => d != TestFunction.All).ToList();
        }
        else functionsToTest = new List<TestFunction>() {options.TestFunction};

        var ext = options.Output.Extension;
        var nameWithoutExt = Path.GetFileNameWithoutExtension(options.Output.FullName);

        foreach (var function in functionsToTest)
        {
            _logger.LogInformation($"Start testing for {function}");
            var fileInfo = new FileInfo(nameWithoutExt + "-" + function.ToString() + ext);
            options.Output = fileInfo;
            options.TestFunction = function;
            _tester.Run(options);
        }
        _logger.LogInformation("Suite finished...");
    }
}