namespace SSP_Lab3;

public class DynamicCalculator
{
    private int _result = 0;

    public DynamicCalculator()
    {
        Console.WriteLine("  [Конструктор DynamicCalculator без параметров]");
    }

    public DynamicCalculator(int initialValue)
    {
        _result = initialValue;
        Console.WriteLine($"  [Конструктор с параметром: начальное значение = {initialValue}]");
    }

    public int Add(int a, int b)
    {
        int res = a + b;
        Console.WriteLine($"  Add({a}, {b}) = {res}");
        return res;
    }

    public double Add(double a, double b)
    {
        double res = a + b;
        Console.WriteLine($"  Add({a}, {b}) = {res}");
        return res;
    }

    public int Multiply(int a, int b)
    {
        int res = a * b;
        Console.WriteLine($"  Multiply({a}, {b}) = {res}");
        return res;
    }

    private void LogOperation(string operation)
    {
        Console.WriteLine($"  [LOG] Выполнена операция: {operation}");
    }

    private int CalculateSquare(int value)
    {
        int res = value * value;
        Console.WriteLine($"  CalculateSquare({value}) = {res}");
        return res;
    }

    public int GetResult() => _result;

    public int Result => _result;

    public void SetResult(int value)
    {
        _result = value;
        Console.WriteLine($"  Установлен результат: {_result}");
    }

    public static string GetVersion()
    {
        return "DynamicCalculator v1.0";
    }
}

public static class Task4
{
    public static void Run()
    {
        Console.WriteLine("=== ЗАДАНИЕ 4: КЛАСС DYNAMICCALCULATOR ===\n");

        Console.WriteLine("Создание экземпляра (черный ящик):");
        var calc = new DynamicCalculator(10);

        Console.WriteLine("\nПубличный метод для чтения приватного поля _result:");
        Console.WriteLine($"  GetResult(): {calc.GetResult()}");

        Console.WriteLine("\nПубличные методы:");
        calc.Add(5, 3);
        calc.Add(2.5, 1.5);
        calc.Multiply(4, 6);
        calc.SetResult(100);
        Console.WriteLine($"  GetResult() после SetResult(100): {calc.GetResult()}");

        Console.WriteLine("\nСтатический метод:");
        Console.WriteLine($"  Версия: {DynamicCalculator.GetVersion()}");

    }
}