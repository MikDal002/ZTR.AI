using System;
using System.Diagnostics;

partial class Build
{
    bool IsWindowsWhenReleaseOrAnyOsWhenOther()
    {
        var isWindows = IsWindows();
        if (isWindows && Configuration == Configuration.Release) return true;
        return Configuration.Release != Configuration;
    }

    static bool IsWindows()
    {
        try
        {
            var p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = "uname",
                    Arguments = "-s"
                }
            };
            p.Start();
            string uname = p.StandardOutput.ReadToEnd().Trim();
            Serilog.Log.Information($"You run this built on {uname} machine.");
            // MSYS_NT - this name return uname on Github Action's machine.
            return uname.Contains("MSYS_NT", StringComparison.InvariantCultureIgnoreCase);
        }
        catch (Exception)
        {
            return true;
        }
    }
}