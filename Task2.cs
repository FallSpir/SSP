namespace SSP_Lab4;

public static class Task2
{
    public static void Run()
    {
        Console.WriteLine("=== Задание 2: Исследование LOH ===\n");

        Console.WriteLine("2.1 Создание объектов разных размеров:");
        var small = new byte[80_000];
        var large = new byte[90_000];

        Console.WriteLine($"  small (80KB): Gen{GC.GetGeneration(small)}");
        Console.WriteLine($"  large (90KB): Gen{GC.GetGeneration(large)}");

        Console.WriteLine("\n2.2 Мониторинг LOH:");
        long memBefore = GC.GetTotalMemory(false);
        Console.WriteLine($"  Память до выделения: {memBefore / (1024 * 1024)} MB");

        var largeObjects = new List<byte[]>();
        for (int i = 0; i < 100; i++)
            largeObjects.Add(new byte[100 * 1024]);

        long memAfter = GC.GetTotalMemory(false);
        Console.WriteLine($"  Память после 100 объектов по 100KB: {memAfter / (1024 * 1024)} MB");

        GC.Collect(2);
        GC.WaitForPendingFinalizers();
        long memAfterGC = GC.GetTotalMemory(false);
        Console.WriteLine($"  Память после GC.Collect(2): {memAfterGC / (1024 * 1024)} MB");
        Console.WriteLine("  Память не освободилась - ссылки в списке удерживают объекты LOH!");
    }
}