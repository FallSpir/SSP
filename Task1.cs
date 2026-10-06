namespace SSP_Lab4;

public class MemoryLeakExample
{
    private static List<byte[]> _staticList = new List<byte[]>();

    public static void CreateMemoryLeak()
    {
        for (int i = 0; i < 1000; i++)
            _staticList.Add(new byte[1024 * 10]);
    }

    public static void ClearMemoryLeak()
    {
        _staticList.Clear();
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }
}

public class EventLeakExample
{
    public event EventHandler BigEvent;
    private List<EventHandler> _handlers = new List<EventHandler>();

    public void SubscribeLeak()
    {
        EventHandler handler = (sender, e) =>
        {
            var data = new byte[1024 * 100];
        };
        BigEvent += handler;
        _handlers.Add(handler);
    }

    public void UnsubscribeAll()
    {
        foreach (var handler in _handlers)
            BigEvent -= handler;
        _handlers.Clear();
    }
}

public static class Task1
{
    public static void Run()
    {
        Console.WriteLine("=== Задание 1: Исследование утечек памяти ===\n");

        Console.WriteLine("1.1 Статическая утечка:");
        Console.WriteLine($"  Память до: {GC.GetTotalMemory(false) / (1024 * 1024)} MB");

        MemoryLeakExample.CreateMemoryLeak();
        Console.WriteLine($"  Память после добавления 1000 объектов: {GC.GetTotalMemory(false) / (1024 * 1024)} MB");

        GC.Collect();
        GC.WaitForPendingFinalizers();
        Console.WriteLine($"  Память после GC.Collect(): {GC.GetTotalMemory(false) / (1024 * 1024)} MB");
        Console.WriteLine("  Память не освободилась - статический список удерживает объекты!");

        MemoryLeakExample.ClearMemoryLeak();
        Console.WriteLine($"  Память после ClearMemoryLeak(): {GC.GetTotalMemory(false) / (1024 * 1024)} MB");

        Console.WriteLine("\n1.2 Событийная утечка:");
        var eventExample = new EventLeakExample();
        Console.WriteLine($"  Память до подписок: {GC.GetTotalMemory(false) / (1024 * 1024)} MB");

        for (int i = 0; i < 100; i++)
            eventExample.SubscribeLeak();

        Console.WriteLine($"  Память после 100 подписок: {GC.GetTotalMemory(false) / (1024 * 1024)} MB");

        eventExample.UnsubscribeAll();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Console.WriteLine($"  Память после отписки и GC: {GC.GetTotalMemory(false) / (1024 * 1024)} MB");
    }
}