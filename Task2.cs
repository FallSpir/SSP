using System.Reflection;

namespace SSP_Lab3;

public static class Task2
{
    public static void Run()
    {
        Console.WriteLine("=== ЗАДАНИЕ 2: ИНСПЕКЦИЯ МЕТОДОВ ===\n");

        Type type = typeof(Product);

        Console.WriteLine("2.1 Только публичные методы (без флагов):");
        MethodInfo[] publicMethods = type.GetMethods();
        foreach (MethodInfo method in publicMethods)
        {
            if (!method.Name.StartsWith("get_") && !method.Name.StartsWith("set_"))
            {
                var parameters = string.Join(", ",
                    method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                Console.WriteLine($"  {method.ReturnType.Name} {method.Name}({parameters})");
            }
        }

        Console.WriteLine("\n2.2 Все методы включая приватные и статические:");
        MethodInfo[] allMethods = type.GetMethods(
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance |
            BindingFlags.Static);

        foreach (MethodInfo method in allMethods)
        {
            if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))
                continue;

            string visibility = method.IsPublic ? "public" :
                                method.IsPrivate ? "private" : "protected";
            string isStatic = method.IsStatic ? "static" : "";
            string isVirtual = method.IsVirtual ? "virtual" : "";
            var parameters = string.Join(", ",
                method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));

            Console.WriteLine($"  [{visibility}] {isStatic} {isVirtual} " +
                $"{method.ReturnType.Name} {method.Name}({parameters})");
        }

        Console.WriteLine("\n2.3 Разница между флагами:");
        int publicCount = type.GetMethods(BindingFlags.Public | BindingFlags.Instance).Length;
        int allCount = type.GetMethods(
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.Static).Length;
        Console.WriteLine($"  Публичных методов экземпляра: {publicCount}");
        Console.WriteLine($"  Всего методов (включая приватные и статические): {allCount}");
    }
}