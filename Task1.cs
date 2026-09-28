using System.Reflection;

namespace SSP_Lab3;

public class Product
{
    private int _id;
    private string _name;
    protected double _price;
    public static int TotalProducts = 0;

    public int Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public string Category { get; private set; }

    public Product()
    {
        _id = 0;
        _name = "Unknown";
        Category = "Default";
        TotalProducts++;
    }

    public Product(int id, string name, double price)
    {
        _id = id;
        _name = name;
        _price = price;
        Id = id;
        Name = name;
        Price = price;
        Category = "Default";
        TotalProducts++;
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"Product: {_name}, Price: {_price}");
    }

    public double ApplyDiscount(double percent)
    {
        return _price * (1 - percent / 100);
    }

    private void ValidatePrice()
    {
        if (_price < 0) _price = 0;
    }

    protected void UpdateCategory(string newCategory)
    {
        Category = newCategory;
    }

    public static int GetTotalCount()
    {
        return TotalProducts;
    }
}

public static class Task1
{
    public static void Run()
    {
        Console.WriteLine("=== ЗАДАНИЕ 1: БАЗОВАЯ ИНСПЕКЦИЯ ТИПА ===\n");

        Type productType = typeof(Product);

        Console.WriteLine("1.1 Базовая информация:");
        Console.WriteLine($"  Имя типа: {productType.Name}");
        Console.WriteLine($"  Полное имя: {productType.FullName}");
        Console.WriteLine($"  Пространство имен: {productType.Namespace}");
        Console.WriteLine($"  Сборка: {productType.Assembly.GetName().Name}");
        Console.WriteLine($"  Базовый тип: {productType.BaseType?.Name ?? "None"}");

        Console.WriteLine("\n1.2 Характеристики:");
        Console.WriteLine($"  IsClass: {productType.IsClass}");
        Console.WriteLine($"  IsAbstract: {productType.IsAbstract}");
        Console.WriteLine($"  IsSealed: {productType.IsSealed}");
        Console.WriteLine($"  IsPublic: {productType.IsPublic}");
        Console.WriteLine($"  IsValueType: {productType.IsValueType}");

        Console.WriteLine("\n1.3 Конструкторы:");
        foreach (ConstructorInfo ctor in productType.GetConstructors())
        {
            var parameters = string.Join(", ",
                ctor.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
            Console.WriteLine($"  {ctor.Name}({parameters}) [Public: {ctor.IsPublic}]");
        }

        Console.WriteLine("\n1.4 Методы:");
        int methodCount = 0;
        foreach (MethodInfo method in productType.GetMethods())
        {
            if (methodCount++ < 10)
            {
                var params_str = string.Join(", ",
                    method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                Console.WriteLine($"  {method.ReturnType.Name} {method.Name}({params_str})");
            }
        }
        Console.WriteLine($"  ... и еще {productType.GetMethods().Length - 10} методов");

        Console.WriteLine("\n1.5 Свойства:");
        foreach (PropertyInfo prop in productType.GetProperties())
        {
            string access = $"{(prop.CanRead ? "get" : "")}{(prop.CanWrite ? "set" : "")}";
            Console.WriteLine($"  {prop.PropertyType.Name} {prop.Name} {{ {access} }}");
        }

        Console.WriteLine("\n1.6 Поля:");
        foreach (FieldInfo field in productType.GetFields(
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance |
            BindingFlags.Static))
        {
            string visibility = field.IsPublic ? "public" : (field.IsPrivate ? "private" : "protected");
            string isStatic = field.IsStatic ? "static" : "";
            Console.WriteLine($"  {visibility} {isStatic} {field.FieldType.Name} {field.Name}");
        }
    }
}