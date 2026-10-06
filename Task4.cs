namespace SSP_Lab4;

public class BuggyCode
{
    private static List<IDisposable> _cache = new List<IDisposable>();
    private static event EventHandler _globalEvent;
    private byte[] _data = new byte[1024 * 1024];

    public BuggyCode()
    {
        _globalEvent += OnEvent;
        _cache.Add(new MemoryStream(1024));
    }

    private void OnEvent(object sender, EventArgs e)
    {
        var temp = new byte[1024 * 100];
    }

    public void SubscribeAll()
    {
        for (int i = 0; i < 1000; i++)
            new BuggyCode();
    }
}

public class FixedCode : IDisposable
{
    private static List<FixedCode> _cache = new List<FixedCode>();
    private static event EventHandler _globalEvent;
    private byte[] _data = new byte[1024 * 1024];
    private bool _disposed;
    private MemoryStream _stream;
    private EventHandler _handler;

    public FixedCode()
    {
        _handler = OnEvent;
        _globalEvent += _handler;
        _stream = new MemoryStream(1024);
        _cache.Add(this);
    }

    private void OnEvent(object sender, EventArgs e)
    {
        var temp = new byte[1024 * 100];
    }

    public static void SubscribeAll()
    {
        for (int i = 0; i < 1000; i++)
            new FixedCode();
    }

    public static void ClearAll()
    {
        foreach (var obj in _cache)
        {
            _globalEvent -= obj._handler;
            obj._stream?.Dispose();
            obj._disposed = true;
        }
        _cache.Clear();
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _globalEvent -= _handler;
            _stream?.Dispose();
            _cache.Remove(this);
            _disposed = true;
        }
    }
}

public static class Task4
{
    public static void Run()
    {
        Console.WriteLine("=== Задание 4: Поиск и исправление утечек ===\n");

        Console.WriteLine("Утечки в BuggyCode:");
        Console.WriteLine("  Утечка 1: _globalEvent += OnEvent — подписка без отписки");
        Console.WriteLine("  Утечка 2: _cache.Add(new MemoryStream) — IDisposable без освобождения");
        Console.WriteLine("  Утечка 3: поле _data (1MB) в каждом объекте не освобождается\n");

        Console.WriteLine("Код с утечками:");
        long memBefore = GC.GetTotalMemory(true);
        var buggy = new BuggyCode();
        buggy.SubscribeAll();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        long memAfter = GC.GetTotalMemory(false);
        Console.WriteLine($"  Память до: {memBefore / (1024 * 1024)} MB");
        Console.WriteLine($"  Память после 1000 объектов: {memAfter / (1024 * 1024)} MB");
        Console.WriteLine($"  Утечка: {(memAfter - memBefore) / (1024 * 1024)} MB");

        Console.WriteLine("\nИсправленный код:");
        memBefore = GC.GetTotalMemory(true);
        FixedCode.SubscribeAll();
        FixedCode.ClearAll();
        memAfter = GC.GetTotalMemory(false);
        Console.WriteLine($"  Память до: {memBefore / (1024 * 1024)} MB");
        Console.WriteLine($"  Память после 1000 объектов + ClearAll: {memAfter / (1024 * 1024)} MB");
        Console.WriteLine($"  Разница: {(memAfter - memBefore) / (1024 * 1024)} MB");
    }
}