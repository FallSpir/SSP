using System.Reflection;

namespace SSP_Lab3;

public static class Task6
{
    public static void Run()
    {
        Console.WriteLine("=== ЗАДАНИЕ 6: ВЫЗОВ ПРИВАТНОГО МЕТОДА ===\n");

        Type type = typeof(DynamicCalculator);
        object calculator = Activator.CreateInstance(type);

        Console.WriteLine("6.1 Вызов приватного метода LogOperation через MethodInfo.Invoke:");
        try
        {
            MethodInfo logMethod = type.GetMethod("LogOperation",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (logMethod == null)
                throw new Exception("Метод LogOperation не найден");

            logMethod.Invoke(calculator, new object[] { "Тестовая операция" });
            Console.WriteLine("  Приватный метод LogOperation вызван успешно");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Ошибка: {ex.Message}");
        }

        Console.WriteLine("\n6.2 Вызов приватного метода CalculateSquare через MethodInfo.Invoke:");
        try
        {
            MethodInfo squareMethod = type.GetMethod("CalculateSquare",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (squareMethod == null)
                throw new Exception("Метод CalculateSquare не найден");

            int result = (int)squareMethod.Invoke(calculator, new object[] { 7 });
            Console.WriteLine($"  Результат CalculateSquare(7): {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Ошибка: {ex.Message}");
        }

        Console.WriteLine("\n6.3 Вызов публичных методов с разным количеством параметров:");
        MethodInfo addIntMethod = type.GetMethod("Add", new[] { typeof(int), typeof(int) });
        if (addIntMethod != null)
        {
            int result = (int)addIntMethod.Invoke(calculator, new object[] { 10, 20 });
            Console.WriteLine($"  Результат Add(10, 20): {result}");
        }

        MethodInfo addDoubleMethod = type.GetMethod("Add", new[] { typeof(double), typeof(double) });
        if (addDoubleMethod != null)
        {
            double result = (double)addDoubleMethod.Invoke(calculator, new object[] { 5.5, 3.2 });
            Console.WriteLine($"  Результат Add(5.5, 3.2): {result}");
        }

        MethodInfo multiplyMethod = type.GetMethod("Multiply");
        if (multiplyMethod != null)
        {
            int result = (int)multiplyMethod.Invoke(calculator, new object[] { 7, 8 });
            Console.WriteLine($"  Результат Multiply(7, 8): {result}");
        }

        Console.WriteLine("\n6.4 Вызов через делегат (второй способ):");
        MethodInfo addMethodForDelegate = type.GetMethod("Add", new[] { typeof(int), typeof(int) });
        if (addMethodForDelegate != null)
        {
            var addDelegate = (Func<int, int, int>)Delegate.CreateDelegate(
                typeof(Func<int, int, int>), calculator, addMethodForDelegate);
            int result = addDelegate(15, 25);
            Console.WriteLine($"  Результат через делегат Add(15, 25): {result}");
        }

    }
}