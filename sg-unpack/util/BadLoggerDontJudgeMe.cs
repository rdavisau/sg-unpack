using System;

public static class BadLoggerDontJudgeMe
{
    public static bool Verbose { get; set; }

    public static void LogVerboseLine(string s = "")
    {
        if (Verbose)
            LogLine(s);
    }

    public static void LogVerbose(string s = "")
    {
        if (Verbose)
            Log(s);
    }

    public static void LogLine(string s = "") => Log(s + Environment.NewLine);
    public static void Log(string s = "") => Console.Write(s);
}