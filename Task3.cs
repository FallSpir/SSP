using System.Reflection;

namespace SSP_Lab3;

public static class Task3
{
    public static void Run()
    {
        Console.WriteLine("=== ЗАДАНИЕ 3: ДИНАМИЧЕСКОЕ СОЗДАНИЕ ЭКЗЕМПЛЯРА ===\n");

        Type type = typeof(Product);

        Console.WriteLine("3.1 Создание через конструктор без параметров:");
        object product1 = Activator.CreateInstance(type);
        Console.WriteLine($"  Экземпляр создан: {product1.GetType().Name}");

        PropertyInfo nameProp = type.GetProperty("Name");
        PropertyInfo idProp = type.GetProperty("Id");
        PropertyInfo priceProp = type.GetProperty("Price");

        nameProp.SetValue(product1, "Laptop");
        idProp.SetValue(product1, 1);
        priceProp.SetValue(product1, 999.99);

        Console.WriteLine($"  Id: {idProp.GetValue(product1)}");
        Console.WriteLine($"  Name: {nameProp.GetValue(product1)}");
        Console.WriteLine($"  Price: {priceProp.GetValue(product1)}");

        Console.WriteLine("\n3.2 Создание через конструктор с параметрами:");
        ConstructorInfo ctor = type.GetConstructor(new[] { typeof(int), typeof(string), typeof(double) });
        object product2 = ctor.Invoke(new object[] { 2, "Smartphone", 599.99 });
        Console.WriteLine($"  Экземпляр создан: {product2.GetType().Name}");
        Console.WriteLine($"  Id: {idProp.GetValue(product2)}");
        Console.WriteLine($"  Name: {nameProp.GetValue(product2)}");
        Console.WriteLine($"  Price: {priceProp.GetValue(product2)}");

        Console.WriteLine("\n3.3 Подтверждение через публичный метод:");
        MethodInfo displayMethod = type.GetMethod("DisplayInfo");
        displayMethod.Invoke(product1, null);
        displayMethod.Invoke(product2, null);

        Console.WriteLine("\n3.4 Статическое поле TotalProducts:");
        FieldInfo totalField = type.GetField("TotalProducts");
        Console.WriteLine($"  Всего создано объектов: {totalField.GetValue(null)}");
    }
}