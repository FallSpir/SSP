using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace SSP_Lab4;

public static class Task5
{
    private static List<string> _logs = new List<string>();
    private static Random _rnd = new Random();
    private static Dictionary<string, int> _callCounts = new Dictionary<string, int>();
    private static Dictionary<string, long> _callTimes = new Dictionary<string, long>();
    private static Stopwatch _globalSw = new Stopwatch();

    public static void Run()
    {
        Console.WriteLine("=== Задание 5: Call Tree профилирование ===\n");

        _globalSw.Start();
        GenerateData(10_000);
        var result = ProcessLogs();
        _globalSw.Stop();

        Console.WriteLine($"Найдено аномалий: {result.Count}, Общее время: {_globalSw.ElapsedMilliseconds} мс\n");

        PrintCallTree();

        Console.WriteLine("\n=== Оптимизация: DateTime.ParseExact вместо DateTime.TryParse ===\n");
        CompareParsePerformance();
    }

    private static void GenerateData(int count)
    {
        var sw = Stopwatch.StartNew();
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        for (int i = 0; i < count; i++)
        {
            var sb = new StringBuilder();
            sb.Append($"USER{_rnd.Next(1, 100)}: ");
            sb.Append($"{DateTime.Now.AddMinutes(-_rnd.Next(0, 10000)):yyyy-MM-dd HH:mm:ss.fff} | ");
            sb.Append(_rnd.Next(0, 4) switch
            {
                0 => "INFO | ",
                1 => "WARNING | ",
                2 => "ERROR | ",
                _ => "DEBUG | "
            });
            for (int j = 0; j < 50; j++)
                sb.Append(chars[_rnd.Next(chars.Length)]);
            sb.Append($" | Code: {_rnd.Next(100, 999)}");
            _logs.Add(sb.ToString());
        }
        for (int i = 0; i < 100; i++)
            _logs.Add($"ANOMALY: {Guid.NewGuid()} | CRITICAL | Ошибка");
        sw.Stop();
        AddCall("GenerateData", sw.ElapsedMilliseconds);
    }

    private static List<string> ProcessLogs()
    {
        var sw = Stopwatch.StartNew();
        var anomalies = new List<string>();

        var filterSw = Stopwatch.StartNew();
        var filtered = new List<string>();
        foreach (var log in _logs)
        {
            if (log.IndexOf("ERROR", StringComparison.OrdinalIgnoreCase) >= 0 ||
                log.IndexOf("WARNING", StringComparison.OrdinalIgnoreCase) >= 0 ||
                log.IndexOf("CRITICAL", StringComparison.OrdinalIgnoreCase) >= 0)
                filtered.Add(log);
        }
        filterSw.Stop();
        AddCall("FilterLogs (IndexOf)", filterSw.ElapsedMilliseconds);

        var anomalySw = Stopwatch.StartNew();
        foreach (var log in filtered)
        {
            if (IsAnomaly(log))
                anomalies.Add(log);
        }
        anomalySw.Stop();
        AddCall("IsAnomaly (total with children)", anomalySw.ElapsedMilliseconds);

        sw.Stop();
        AddCall("ProcessLogs", sw.ElapsedMilliseconds);
        return anomalies;
    }

    private static bool IsAnomaly(string log)
    {
        var methodSw = Stopwatch.StartNew();

        if (log.Length < 100)
        {
            methodSw.Stop();
            AddCall("IsAnomaly.CheckLength", methodSw.ElapsedMilliseconds);
            return false;
        }

        var splitSw = Stopwatch.StartNew();
        var parts = log.Split('|');
        splitSw.Stop();
        AddCall("IsAnomaly.String.Split", splitSw.ElapsedMilliseconds);

        if (parts.Length < 3)
        {
            methodSw.Stop();
            AddCall("IsAnomaly", methodSw.ElapsedMilliseconds);
            return false;
        }

        var dateSw = Stopwatch.StartNew();
        bool dateOk = DateTime.TryParse(parts[1]?.Trim(), out var logDate);
        dateSw.Stop();
        AddCall("IsAnomaly.DateTime.TryParse", dateSw.ElapsedMilliseconds);

        if (!dateOk)
        {
            methodSw.Stop();
            AddCall("IsAnomaly", methodSw.ElapsedMilliseconds);
            return false;
        }

        var regexSw = Stopwatch.StartNew();
        var codeMatch = Regex.Match(parts.Length > 3 ? parts[3] : "", @"Code:\s*(\d+)");
        var code = codeMatch.Success && int.TryParse(codeMatch.Groups[1].Value, out int c) ? c : 0;
        regexSw.Stop();
        AddCall("IsAnomaly.Regex.Match", regexSw.ElapsedMilliseconds);

        var checkSw = Stopwatch.StartNew();
        var hasAnomaly = log.Contains("ANOMALY", StringComparison.OrdinalIgnoreCase);
        var isRecent = logDate > DateTime.Now.AddDays(-1);
        bool result = (hasAnomaly && isRecent) || (code > 500 && isRecent) || (hasAnomaly && code > 500);
        checkSw.Stop();
        AddCall("IsAnomaly.Checks", checkSw.ElapsedMilliseconds);

        methodSw.Stop();
        AddCall("IsAnomaly (own time)", methodSw.ElapsedMilliseconds);
        return result;
    }

    private static void AddCall(string methodName, long timeMs)
    {
        if (!_callCounts.ContainsKey(methodName))
        {
            _callCounts[methodName] = 0;
            _callTimes[methodName] = 0;
        }
        _callCounts[methodName]++;
        _callTimes[methodName] += timeMs;
    }

    private static void PrintCallTree()
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                      CALL TREE (CPU TIME)                        ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════════╝\n");

        long totalTime = _globalSw.ElapsedMilliseconds;

        PrintNode("Main", totalTime, totalTime);
        PrintNode("  ├── GenerateData", _callTimes["GenerateData"], totalTime);
        PrintNode("  └── ProcessLogs", _callTimes["ProcessLogs"], totalTime);
        PrintNode("        ├── FilterLogs (IndexOf)", _callTimes["FilterLogs (IndexOf)"], _callTimes["ProcessLogs"]);
        PrintNode("        └── IsAnomaly (total with children)", _callTimes["IsAnomaly (total with children)"], _callTimes["ProcessLogs"]);

        long isAnomalyTotal = _callTimes["IsAnomaly (total with children)"];
        PrintNode("              ├── IsAnomaly.String.Split", _callTimes.GetValueOrDefault("IsAnomaly.String.Split", 0), isAnomalyTotal);
        PrintNode("              ├── IsAnomaly.DateTime.TryParse", _callTimes.GetValueOrDefault("IsAnomaly.DateTime.TryParse", 0), isAnomalyTotal);
        PrintNode("              ├── IsAnomaly.Regex.Match", _callTimes.GetValueOrDefault("IsAnomaly.Regex.Match", 0), isAnomalyTotal);
        PrintNode("              ├── IsAnomaly.Checks", _callTimes.GetValueOrDefault("IsAnomaly.Checks", 0), isAnomalyTotal);
        PrintNode("              └── IsAnomaly (own time)", _callTimes.GetValueOrDefault("IsAnomaly (own time)", 0), isAnomalyTotal);

        Console.WriteLine("\n╔═══════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║ Горячий путь: Main -> ProcessLogs -> IsAnomaly -> Regex.Match    ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════════╝");

        Console.WriteLine("\nСтатистика вызовов:");
        foreach (var kvp in _callCounts)
        {
            string prefix = kvp.Key.Contains("Regex") ? "  [HOT] " : "        ";
            Console.WriteLine($"{prefix}{kvp.Key}: {kvp.Value} вызовов, {_callTimes[kvp.Key]} мс");
        }
    }

    private static void PrintNode(string name, long time, long parentTime)
    {
        double percent = parentTime > 0 ? (double)time / parentTime * 100 : 0;
        string percentStr = percent.ToString("F1");
        int barLength = (int)(percent / 2);
        string bar = new string('█', Math.Min(barLength, 50));
        string indicator = name.Contains("Regex") ? " [HOT x3]" :
                           name.Contains("IsAnomaly (total") ? " [HOT x2]" :
                           name.Contains("TryParse") ? " [HOT]" : "";
        Console.WriteLine($"{name.PadRight(45)} {time,5} мс ({percentStr,5}%) {bar}{indicator}");
    }

    private static void CompareParsePerformance()
    {
        const int iterations = 100000;
        string dateStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string format = "yyyy-MM-dd HH:mm:ss.fff";

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
            DateTime.TryParse(dateStr, out _);
        sw.Stop();
        Console.WriteLine($"  DateTime.TryParse    ({iterations} итераций): {sw.ElapsedMilliseconds} мс");

        sw.Restart();
        for (int i = 0; i < iterations; i++)
        {
            try { DateTime.ParseExact(dateStr, format, null); }
            catch { }
        }
        sw.Stop();
        Console.WriteLine($"  DateTime.ParseExact  ({iterations} итераций): {sw.ElapsedMilliseconds} мс");
        Console.WriteLine("  ParseExact быстрее так как не перебирает форматы - формат задан явно.");
    }
}