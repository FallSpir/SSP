using System.Collections;
using System.Diagnostics;

namespace SSP_Lab4;

public class UnoptimizedCode
{
    private ArrayList _data = new ArrayList();
    public void Add(int value) => _data.Add(value);
}

public class OptimizedCode
{
    private List<int> _data = new List<int>();
    public void Add(int value) => _data.Add(value);
}

public static class Task3
{
    public static void Run()
    {
        Console.WriteLine("=== Задание 3: Оптимизация Boxing ===\n");

        Console.WriteLine("3.1 Сравнение производительности:");

        long memBefore = GC.GetTotalMemory(true);
        var sw = Stopwatch.StartNew();
        var boxed = new List<object>();
        for (int i = 0; i < 1000000; i++)
            boxed.Add(i);
        sw.Stop();
        long memAfter = GC.GetTotalMemory(false);
        Console.WriteLine($"  С упаковкой:    время = {sw.ElapsedMilliseconds}ms, память = {(memAfter - memBefore) / (1024 * 1024)} MB");

        GC.Collect();
        GC.WaitForPendingFinalizers();

        memBefore = GC.GetTotalMemory(true);
        sw.Restart();
        var unboxed = new List<int>();
        for (int i = 0; i < 1000000; i++)
            unboxed.Add(i);
        sw.Stop();
        memAfter = GC.GetTotalMemory(false);
        Console.WriteLine($"  Без упаковки:   время = {sw.ElapsedMilliseconds}ms, память = {(memAfter - memBefore) / (1024 * 1024)} MB");

        Console.WriteLine("\n3.2 Оптимизация существующего кода:");

        var unopt = new UnoptimizedCode();
        memBefore = GC.GetTotalMemory(true);
        sw.Restart();
        for (int i = 0; i < 1000000; i++)
            unopt.Add(i);
        sw.Stop();
        memAfter = GC.GetTotalMemory(false);
        Console.WriteLine($"  ArrayList (упаковка):    время = {sw.ElapsedMilliseconds}ms, память = {(memAfter - memBefore) / (1024 * 1024)} MB");

        GC.Collect();
        GC.WaitForPendingFinalizers();

        var opt = new OptimizedCode();
        memBefore = GC.GetTotalMemory(true);
        sw.Restart();
        for (int i = 0; i < 1000000; i++)
            opt.Add(i);
        sw.Stop();
        memAfter = GC.GetTotalMemory(false);
        Console.WriteLine($"  List<int> (без упаковки): время = {sw.ElapsedMilliseconds}ms, память = {(memAfter - memBefore) / (1024 * 1024)} MB");
    }
}