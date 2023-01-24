using System.Globalization;
using Light.GuardClauses;

namespace ZT.AI.Researcher
{
    public class RawResultsHandler
    {
        public void SaveResults(IGlobalOptions options, int steps, IEnumerable<double> results)
        {
            options.AlgorithmName.MustNotBeNullOrWhiteSpace();

            var fileName = Path.GetFileNameWithoutExtension(options.Output.FullName);
            var newName = Path.Join(options.Output.Directory.FullName, "Raw", fileName + "-" + steps + ".dat");

            
            File.Delete(newName);
            File.AppendAllLines(newName, new [] {"v set"});

            File.AppendAllLines(newName, results.Select(d => d.ToString("F5", CultureInfo.InvariantCulture) + " " + options.AlgorithmName));
        }
    }
}