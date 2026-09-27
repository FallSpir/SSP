namespace SSP_Lab2;

public static class Task4
{
    public static void Run()
    {
        Console.WriteLine("=== Задание 4: Статические ссылки и утечки памяти ===");

        Console.WriteLine("\n--- Статическая утечка ---");
        StaticLeakDemo.AddData();
        StaticLeakDemo.CheckMemory();
        GC.Collect();
        Console.WriteLine("После GC.Collect():");
        StaticLeakDemo.CheckMemory();

        Console.WriteLine("\n--- Событийная утечка ---");
        var eventDemo = new EventLeakDemo();
        Console.WriteLine("До подписок:");
        Console.WriteLine($"Total Memory: {GC.GetTotalMemory(false) / (1024 * 1024)} MB");

        for (int i = 0; i < 100; i++)
            eventDemo.Subscribe();

        eventDemo.FireEvent();
        Console.WriteLine("После 100 подписок и вызова события:");
        Console.WriteLine($"Total Memory: {GC.GetTotalMemory(false) / (1024 * 1024)} MB");

        eventDemo.Unsubscribe();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Console.WriteLine("После отписки и GC:");
        Console.WriteLine($"Total Memory: {GC.GetTotalMemory(false) / (1024 * 1024)} MB");

        Console.WriteLine("\n--- Поколения объектов ---");
        var obj = new object();
        Console.WriteLine($"Поколение после создания: {GC.GetGeneration(obj)}");
        GC.Collect();
        Console.WriteLine($"Поколение после 1 GC: {GC.GetGeneration(obj)}");
        GC.Collect();
        Console.WriteLine($"Поколение после 2 GC: {GC.GetGeneration(obj)}");
        GC.Collect();
        Console.WriteLine($"Поколение после 3 GC: {GC.GetGeneration(obj)}");

        var smallObj = new byte[1024 * 10];
        var largeObj = new byte[1024 * 90];
        Console.WriteLine($"\nМалый объект (10KB) - поколение: {GC.GetGeneration(smallObj)}");
        Console.WriteLine($"Большой объект (90KB) - поколение: {GC.GetGeneration(largeObj)}");
    }
}

public static class StaticLeakDemo
{
    private static List<byte[]> _staticList = new List<byte[]>();

    public static void AddData()
    {
        for (int i = 0; i < 1000; i++)
        {
            _staticList.Add(new byte[1024 * 10]);
        }
    }

    public static void CheckMemory()
    {
        Console.WriteLine($"Total Memory: {GC.GetTotalMemory(false) / (1024 * 1024)} MB");
        Console.WriteLine($"Объектов в списке: {_staticList.Count}");
        Console.WriteLine($"Поколение списка: {GC.GetGeneration(_staticList)}");
    }
}

public class EventLeakDemo
{
    public event EventHandler BigEvent;
    private EventHandler _handler;

    public void Subscribe()
    {
        _handler = (sender, e) =>
        {
            var data = new byte[1024 * 100];
        };
        BigEvent += _handler;
    }

    public void FireEvent()
    {
        BigEvent?.Invoke(this, EventArgs.Empty);
    }

    public void Unsubscribe()
    {
        BigEvent -= _handler;
        _handler = null;
    }
}