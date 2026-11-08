namespace SSP_Lab6;

class Program
{
    static async Task Main(string[] args)
    {
        await MqttTask.Run();
    }
}