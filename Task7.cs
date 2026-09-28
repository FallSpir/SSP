using System.Reflection;

namespace SSP_Lab3;

[AttributeUsage(AttributeTargets.Class)]
public class PluginAttribute : Attribute
{
    public string Name { get; }
    public PluginAttribute(string name) => Name = name;
}

public interface IPlugin
{
    string Name { get; }
    string Version { get; }
    void Execute();
}

public interface IParameterizedPlugin : IPlugin
{
    void ExecuteWithParams(Dictionary<string, object> parameters);
}

[Plugin("Calculator")]
public class CalculatorPlugin : IPlugin
{
    public string Name => "Calculator";
    public string Version => "1.0.0";

    public void Execute()
    {
        Console.WriteLine("Calculator plugin loaded");
    }

    public double Add(double a, double b) => a + b;
    public double Subtract(double a, double b) => a - b;
}

[Plugin("Logger")]
public class LoggerPlugin : IParameterizedPlugin
{
    public string Name => "Logger";
    public string Version => "1.0.0";

    public void Execute()
    {
        Console.WriteLine("Logger plugin executed");
    }

    public void ExecuteWithParams(Dictionary<string, object> parameters)
    {
        if (parameters.TryGetValue("message", out var msg))
        {
            Console.WriteLine($"[LOG] {msg}");
        }
    }
}

public class PluginManager
{
    private readonly List<IPlugin> _plugins = new List<IPlugin>();
    private readonly string _configPath;

    public PluginManager(string configPath = "plugins.txt")
    {
        _configPath = configPath;
    }

    public void LoadPluginsFromConfig()
    {
        if (!File.Exists(_configPath))
        {
            Console.WriteLine($"  Конфигурационный файл не найден: {_configPath}");
            return;
        }

        string[] pluginNames = File.ReadAllLines(_configPath);
        Assembly assembly = Assembly.GetExecutingAssembly();

        foreach (string pluginName in pluginNames)
        {
            if (string.IsNullOrWhiteSpace(pluginName)) continue;

            Type[] types = assembly.GetTypes();
            foreach (var type in types)
            {
                PluginAttribute attr = type.GetCustomAttribute<PluginAttribute>();
                if (attr != null && attr.Name == pluginName.Trim())
                {
                    if (typeof(IPlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    {
                        IPlugin plugin = (IPlugin)Activator.CreateInstance(type);
                        _plugins.Add(plugin);
                        Console.WriteLine($"  Loaded plugin: {plugin.Name} v{plugin.Version}");
                    }
                }
            }
        }
    }

    public IEnumerable<IPlugin> GetPlugins() => _plugins;

    public IPlugin GetPlugin(string name)
    {
        return _plugins.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public void ExecutePluginMethod(IPlugin plugin, string methodName, object[] parameters)
    {
        Type pluginType = plugin.GetType();
        MethodInfo method = pluginType.GetMethod(methodName);

        if (method != null)
        {
            var result = method.Invoke(plugin, parameters);
            Console.WriteLine($"  Method {methodName} executed. Result: {result ?? "null"}");
        }
        else
        {
            Console.WriteLine($"  Method {methodName} not found in plugin {plugin.Name}");
        }
    }

    public T ExecutePluginMethod<T>(IPlugin plugin, string methodName, object[] parameters)
    {
        Type pluginType = plugin.GetType();
        MethodInfo method = pluginType.GetMethod(methodName);

        if (method != null && method.ReturnType == typeof(T))
        {
            return (T)method.Invoke(plugin, parameters);
        }
        return default;
    }
}

public static class Task7
{
    public static void Run()
    {
        Console.WriteLine("=== ЗАДАНИЕ 7: СИСТЕМА ПЛАГИНОВ ===\n");

        string configPath = "plugins.txt";
        File.WriteAllLines(configPath, new[] { "Calculator", "Logger" });

        Console.WriteLine("7.1 Загрузка плагинов из конфигурационного файла:");
        var manager = new PluginManager(configPath);
        manager.LoadPluginsFromConfig();

        Console.WriteLine("\n7.2 Выполнение плагинов:");
        foreach (var plugin in manager.GetPlugins())
        {
            Console.WriteLine($"\n  Плагин: {plugin.Name} v{plugin.Version}");
            plugin.Execute();

            PluginAttribute attr = plugin.GetType().GetCustomAttribute<PluginAttribute>();
            if (attr != null)
                Console.WriteLine($"  Атрибут [Plugin]: {attr.Name}");
        }

        Console.WriteLine("\n7.3 Вызов метода через рефлексию (CalculatorPlugin):");
        var calcPlugin = manager.GetPlugin("Calculator");
        if (calcPlugin != null)
        {
            manager.ExecutePluginMethod(calcPlugin, "Add", new object[] { 10.0, 5.0 });
            manager.ExecutePluginMethod(calcPlugin, "Subtract", new object[] { 10.0, 3.0 });

            double result = manager.ExecutePluginMethod<double>(calcPlugin, "Add", new object[] { 7.0, 3.0 });
            Console.WriteLine($"  Результат через generic метод Add(7, 3): {result}");
        }

        Console.WriteLine("\n7.4 Вызов метода с параметрами (LoggerPlugin):");
        var loggerPlugin = manager.GetPlugin("Logger");
        if (loggerPlugin is IParameterizedPlugin paramPlugin)
        {
            paramPlugin.ExecuteWithParams(new Dictionary<string, object>
            {
                { "message", "Выполнено успешно" }
            });
        }
    }
}