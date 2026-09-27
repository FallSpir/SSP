using System.Diagnostics;

namespace SSP_Lab2;

public static class Task3
{
    public static void Run()
    {
        Console.WriteLine("=== Задание 3: Управление памятью ===");

        using (var monitor = new MemoryMonitor())
        {
            Console.WriteLine("\n1. Базовое выделение памяти:");
            monitor.AllocateMemory(10);
            monitor.PrintMemoryInfo();

            Console.WriteLine("\n2. Создание объектов в LOH:");
            monitor.AllocateLOHObjects(100);
            monitor.PrintMemoryInfo();

            Console.WriteLine("\n3. Тест упаковки:");
            var stopwatch = Stopwatch.StartNew();
            monitor.SimulateBoxing();
            stopwatch.Stop();
            Console.WriteLine($"Время выполнения с упаковкой: {stopwatch.ElapsedMilliseconds}ms");

            Console.WriteLine("\n4. После принудительной сборки:");
            monitor.Cleanup();
            monitor.PrintMemoryInfo();

            Console.WriteLine("\n5. Оптимизированная версия (без упаковки):");
            stopwatch.Restart();
            var optimizedList = new List<int>();
            for (int i = 0; i < 1000000; i++)
            {
                optimizedList.Add(i);
            }
            stopwatch.Stop();
            Console.WriteLine($"Время выполнения без упаковки: {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}