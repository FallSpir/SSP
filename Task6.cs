using System.Collections;
using System.Diagnostics;

namespace SSP_Lab2;

public class UnoptimizedCode
{
    private ArrayList _data = new ArrayList();

    public void AddData(int value)
    {
        _data.Add(value);
    }

    public int GetSum()
    {
        int sum = 0;
        foreach (object item in _data)
        {
            sum += (int)item;
        }
        return sum;
    }
}

public class OptimizedCode
{
    private List<int> _data = new List<int>();

    public void AddData(int value)
    {
        _data.Add(value);
    }

    public int GetSum()
    {
        int sum = 0;
        foreach (int item in _data)
        {
            sum += item;
        }
        return sum;
    }
}

public static class Task6
{
    public static void Run()
    {
        Console.WriteLine("=== Задание 6: Оптимизация через устранение упаковки ===");

        Console.WriteLine("\n--- Сравнение производительности ---");

        long memBefore = GC.GetTotalMemory(true);
        var sw = Stopwatch.StartNew();
        ProcessWithBoxing();
        sw.Stop();
        long memAfter = GC.GetTotalMemory(false);
        long timeWithBoxing = sw.ElapsedMilliseconds;
        long memWithBoxing = (memAfter - memBefore) / (1024 * 1024);

        GC.Collect();
        GC.WaitForPendingFinalizers();

        memBefore = GC.GetTotalMemory(true);
        sw.Restart();
        ProcessWithoutBoxing();
        sw.Stop();
        memAfter = GC.GetTotalMemory(false);
        long timeWithoutBoxing = sw.ElapsedMilliseconds;
        long memWithoutBoxing = (memAfter - memBefore) / (1024 * 1024);

        Console.WriteLine($"{"Метод",-25} {"Время (ms)",-15} {"Память (MB)",-15}");
        Console.WriteLine(new string('-', 55));
        Console.WriteLine($"{"С упаковкой",-25} {timeWithBoxing,-15} {memWithBoxing,-15}");
        Console.WriteLine($"{"Без упаковки",-25} {timeWithoutBoxing,-15} {memWithoutBoxing,-15}");

        Console.WriteLine("\n--- Оптимизация существующего кода ---");

        var unoptimized = new UnoptimizedCode();
        memBefore = GC.GetTotalMemory(true);
        sw.Restart();
        for (int i = 0; i < 1000000; i++)
            unoptimized.AddData(i);
        int sum1 = unoptimized.GetSum();
        sw.Stop();
        memAfter = GC.GetTotalMemory(false);
        long timeUnopt = sw.ElapsedMilliseconds;
        long memUnopt = (memAfter - memBefore) / (1024 * 1024);

        GC.Collect();
        GC.WaitForPendingFinalizers();

        var optimized = new OptimizedCode();
        memBefore = GC.GetTotalMemory(true);
        sw.Restart();
        for (int i = 0; i < 1000000; i++)
            optimized.AddData(i);
        int sum2 = optimized.GetSum();
        sw.Stop();
        memAfter = GC.GetTotalMemory(false);
        long timeOpt = sw.ElapsedMilliseconds;
        long memOpt = (memAfter - memBefore) / (1024 * 1024);

        Console.WriteLine($"{"Реализация",-25} {"Время (ms)",-15} {"Память (MB)",-15}");
        Console.WriteLine(new string('-', 55));
        Console.WriteLine($"{"ArrayList (упаковка)",-25} {timeUnopt,-15} {memUnopt,-15}");
        Console.WriteLine($"{"List<int> (без упаковки)",-25} {timeOpt,-15} {memOpt,-15}");
    }

    static void ProcessWithBoxing()
    {
        var list = new List<object>();
        for (int i = 0; i < 1000000; i++)
            list.Add(i);
    }

    static void ProcessWithoutBoxing()
    {
        var list = new List<int>();
        for (int i = 0; i < 1000000; i++)
            list.Add(i);
    }
}