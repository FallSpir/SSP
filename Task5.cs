using System.Reflection;

namespace SSP_Lab3;

public static class Task5
{
    public static void Run()
    {
        Console.WriteLine("=== ЗАДАНИЕ 5: ДОСТУП К ПРИВАТНОМУ ПОЛЮ ===\n");

        Product product = new Product(1, "Laptop", 1000.0);
        Type type = typeof(Product);

        Console.WriteLine("5.1 Работа с приватным полем _price:");
        try
        {
            FieldInfo priceField = type.GetField("_price",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (priceField == null)
                throw new Exception("Поле _price не найдено");

            double oldValue = (double)priceField.GetValue(product);
            Console.WriteLine($"  Текущее значение _price: {oldValue}");

            priceField.SetValue(product, 1500.0);
            double newValue = (double)priceField.GetValue(product);
            Console.WriteLine($"  Новое значение _price: {newValue}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Ошибка: {ex.Message}");
        }

        Console.WriteLine("\n5.2 Вызов приватного метода ValidatePrice:");
        try
        {
            FieldInfo priceField = type.GetField("_price",
                BindingFlags.NonPublic | BindingFlags.Instance);

            MethodInfo validateMethod = type.GetMethod("ValidatePrice",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (validateMethod == null)
                throw new Exception("Метод ValidatePrice не найден");

            priceField.SetValue(product, -500.0);
            Console.WriteLine($"  Цена до валидации: {priceField.GetValue(product)}");

            validateMethod.Invoke(product, null);
            Console.WriteLine($"  Цена после валидации: {priceField.GetValue(product)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Ошибка: {ex.Message}");
        }

        Console.WriteLine("\n5.3 Работа с приватным полем _name:");
        try
        {
            FieldInfo nameField = type.GetField("_name",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (nameField == null)
                throw new Exception("Поле _name не найдено");

            string currentName = (string)nameField.GetValue(product);
            Console.WriteLine($"  Текущее имя: {currentName}");
            nameField.SetValue(product, "Gaming Laptop");
            Console.WriteLine($"  Новое имя: {nameField.GetValue(product)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Ошибка: {ex.Message}");
        }

        Console.WriteLine("\n5.4 Вызов защищенного метода UpdateCategory:");
        try
        {
            MethodInfo updateCategory = type.GetMethod("UpdateCategory",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (updateCategory == null)
                throw new Exception("Метод UpdateCategory не найден");

            PropertyInfo categoryProp = type.GetProperty("Category");
            Console.WriteLine($"  Категория до: {categoryProp.GetValue(product)}");
            updateCategory.Invoke(product, new object[] { "Electronics" });
            Console.WriteLine($"  Категория после: {categoryProp.GetValue(product)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Ошибка: {ex.Message}");
        }

        Console.WriteLine("\n5.5 Попытка получить несуществующее поле:");
        try
        {
            FieldInfo missingField = type.GetField("_nonExistentField",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (missingField == null)
                throw new Exception("Поле _nonExistentField не найдено");

            missingField.GetValue(product);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Ошибка: {ex.Message}");
        }

    }
}