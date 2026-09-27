namespace SSP_Lab2;

public class MemoryMonitor : IDisposable
{
    private bool _disposed;
    private List<byte[]> _allocatedMemory = new List<byte[]>();
    private Random _random = new Random();

    public void AllocateMemory(int sizeInMB)
    {
        var data = new byte[sizeInMB * 1024 * 1024];
        _allocatedMemory.Add(data);
    }

    public void AllocateLOHObjects(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var largeObject = new byte[90000 + i];
        }
    }

    public void SimulateBoxing()
    {
        var list = new List<object>();
        for (int i = 0; i < 1000000; i++)
        {
            list.Add(i);
        }
    }

    public void PrintMemoryInfo()
    {
        Console.WriteLine($"GC Generation: {GC.GetGeneration(this)}");
        Console.WriteLine($"Total Memory: {GC.GetTotalMemory(false) / (1024 * 1024)} MB");
        Console.WriteLine($"Collection Count Gen0: {GC.CollectionCount(0)}");
        Console.WriteLine($"Collection Count Gen1: {GC.CollectionCount(1)}");
        Console.WriteLine($"Collection Count Gen2: {GC.CollectionCount(2)}");
    }

    public void Cleanup()
    {
        _allocatedMemory.Clear();
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            Cleanup();
            _disposed = true;
        }
    }
}

public static class Task2
{
    public static void Run()
    {
        Console.WriteLine("=== Задание 2: Класс MemoryMonitor ===");
        using (var monitor = new MemoryMonitor())
        {
            monitor.AllocateMemory(10);
            monitor.PrintMemoryInfo();
        }
        Console.WriteLine("MemoryMonitor успешно освобождён через Dispose.");
    }
}