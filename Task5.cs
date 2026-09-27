namespace SSP_Lab2;

public static class Task5
{
    public static void Run()
    {
        Console.WriteLine("=== Задание 5: Исследование Large Object Heap ===");

        Console.WriteLine("\n--- Создание объектов в LOH ---");
        long memBefore = GC.GetTotalMemory(false);
        Console.WriteLine($"Память до выделения: {memBefore / (1024 * 1024)} MB");

        var largeObjects = new List<byte[]>();
        for (int i = 0; i < 100; i++)
        {
            largeObjects.Add(new byte[100 * 1024]);
        }

        long memAfter = GC.GetTotalMemory(false);
        Console.WriteLine($"Память после выделения 100 объектов по 100KB: {memAfter / (1024 * 1024)} MB");

        GC.Collect();
        GC.WaitForPendingFinalizers();
        long memAfterGC = GC.GetTotalMemory(false);
        Console.WriteLine($"Память после GC.Collect(): {memAfterGC / (1024 * 1024)} MB");

        Console.WriteLine("\n--- Сравнение размещения объектов ---");
        var obj90KB = new byte[90 * 1024];
        var obj80KB = new byte[80 * 1024];

        Console.WriteLine($"Объект 90KB - поколение: {GC.GetGeneration(obj90KB)}");
        Console.WriteLine($"Объект 80KB - поколение: {GC.GetGeneration(obj80KB)}");

        Console.WriteLine("\n--- Мониторинг LOH ---");
        var obj10KB = new byte[10 * 1024];
        var obj50KB = new byte[50 * 1024];
        var obj90KB2 = new byte[90 * 1024];
        var obj200KB = new byte[200 * 1024];

        Console.WriteLine($"Объект 10KB  - поколение: {GC.GetGeneration(obj10KB)}");
        Console.WriteLine($"Объект 50KB  - поколение: {GC.GetGeneration(obj50KB)}");
        Console.WriteLine($"Объект 90KB  - поколение: {GC.GetGeneration(obj90KB2)}");
        Console.WriteLine($"Объект 200KB - поколение: {GC.GetGeneration(obj200KB)}");
    }
}