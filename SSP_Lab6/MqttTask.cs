using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Protocol;
using System.Text;
using System.Text.Json;

namespace SSP_Lab6;

public static class MqttTask
{
    private static readonly List<double> _receivedTemperatures = new();
    private static readonly string _logFile = "sensor_log.txt";

    public static async Task Run()
    {
        Console.WriteLine("=== IoT Симулятор датчика ===\n");

        var sensor = new EnvironmentAwareSensor();

        MqttFactory factory = new MqttFactory();
        MqttClient client = (MqttClient)factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer("broker.hivemq.com", 1883)
            .WithClientId("IoTSensor_" + Guid.NewGuid().ToString()[..6])
            .WithCleanSession()
            .Build();

        client.ConnectedHandler = new MqttClientConnectedHandlerDelegate(async args =>
        {
            Console.WriteLine("✅ Подключено к брокеру broker.hivemq.com");
            await client.SubscribeAsync("iot/sensor/response", MqttQualityOfServiceLevel.AtLeastOnce);
            Console.WriteLine("📬 Подписались на iot/sensor/response\n");
        });

        client.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(args =>
        {
            Console.WriteLine("❌ Отключено от брокера");
        });

        client.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(args =>
        {
            string msg = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);
            Console.WriteLine($"📥 Получено от сервера: {msg}");
        });

        try
        {
            await client.ConnectAsync(options);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка подключения: {ex.Message}");
            Console.WriteLine("Работаем в автономном режиме...\n");
        }

        Console.WriteLine("Команды: door, ac, heater, add, remove, light, status, exit\n");
        _ = Task.Run(() => ProcessCommands(sensor));

        int cycle = 0;
        while (true)
        {
            await Task.Delay(5000);
            var data = sensor.ReadData();

            Console.WriteLine($"\n[{cycle++}] {data}");
            DrawTemperatureChart(data.Temperature);

            // Задание 1: гудок если температура ниже 22
            if (data.Temperature < 22.0)
            {
                Console.Beep(800, 300);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("🔔 Температура ниже 22°C!");
                Console.ResetColor();
            }

            // Задание 2: запись в файл
            await File.AppendAllTextAsync(_logFile, data.ToString() + Environment.NewLine);

            // Отправка на брокер
            if (client.IsConnected)
            {
                string json = JsonSerializer.Serialize(data);
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic("iot/sensor/data")
                    .WithPayload(json)
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build();

                await client.PublishAsync(message);
                Console.WriteLine("📤 Отправлено на брокер");
            }

            // Задание 4: накапливаем 20 точек и рисуем график
            if (_receivedTemperatures.Count < 20)
            {
                _receivedTemperatures.Add(data.Temperature);
                Console.WriteLine($"📊 Точек собрано: {_receivedTemperatures.Count}/20");
                if (_receivedTemperatures.Count == 20)
                    SaveAndDrawChart();
            }
        }
    }

    private static void DrawTemperatureChart(double temperature, double min = -10, double max = 40)
    {
        int width = 40;
        int position = Math.Clamp((int)((temperature - min) / (max - min) * width), 0, width);

        Console.Write("[");
        Console.Write(new string('░', position));
        Console.ForegroundColor = temperature switch
        {
            > 30 => ConsoleColor.Red,
            > 25 => ConsoleColor.Yellow,
            > 15 => ConsoleColor.Green,
            > 5 => ConsoleColor.Cyan,
            _ => ConsoleColor.Blue
        };
        Console.Write("█");
        Console.ResetColor();
        Console.Write(new string('░', width - position));
        Console.WriteLine($"] {temperature:F1}°C");
    }

    private static void SaveAndDrawChart()
    {
        Console.WriteLine("\n=== График последних 20 точек ===");
        double max = _receivedTemperatures.Max();
        double min = _receivedTemperatures.Min();

        File.WriteAllLines("chart_data.txt", _receivedTemperatures.Select((t, i) => $"{i + 1}: {t:F1}°C"));
        Console.WriteLine("💾 Сохранено в chart_data.txt\n");

        foreach (var t in _receivedTemperatures)
            DrawTemperatureChart(t, min - 1, max + 1);
    }

    private static async Task ProcessCommands(EnvironmentAwareSensor sensor)
    {
        while (true)
        {
            var input = Console.ReadLine()?.ToLower().Trim();
            switch (input)
            {
                case "door":
                    Console.Write("open/close: ");
                    if (Console.ReadLine()?.ToLower().Trim() == "open") sensor.OpenDoor();
                    else sensor.CloseDoor();
                    break;
                case "ac":
                    Console.Write("on/off: ");
                    if (Console.ReadLine()?.ToLower().Trim() == "on") sensor.TurnOnAC();
                    else sensor.TurnOffAC();
                    break;
                case "heater":
                    Console.Write("on/off: ");
                    if (Console.ReadLine()?.ToLower().Trim() == "on") sensor.TurnOnHeater();
                    else sensor.TurnOffHeater();
                    break;
                case "add":
                    sensor.AddPerson();
                    Console.WriteLine("👤 Человек добавлен");
                    break;
                case "remove":
                    sensor.RemovePerson();
                    Console.WriteLine("👤 Человек удалён");
                    break;
                case "light":
                    Console.Write("Уровень освещения (лк): ");
                    if (double.TryParse(Console.ReadLine(), out double lux))
                        sensor.SetLighting(lux);
                    break;
                case "status":
                    Console.WriteLine($"\n📊 {sensor.ReadData()}\n");
                    break;
                case "exit":
                    Environment.Exit(0);
                    break;
            }
        }
    }
}