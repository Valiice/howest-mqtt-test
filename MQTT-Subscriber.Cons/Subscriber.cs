using MQTTnet;
using MQTTnet.Client;
using System.Diagnostics;
using System.Text;

namespace MQTT_Subscriber.Cons
{
    internal class Subscriber
    {
        static async Task Main(string[] args)
        {
            var mqttFactory = new MqttFactory();
            IMqttClient client = mqttFactory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                            .WithClientId(Guid.NewGuid().ToString())
                            .WithTcpServer("test.mosquitto.org", 1883)
                            .WithCleanSession()
                            .Build();
            client.ConnectedAsync += async e => await MqttOnConnected(e, client);
            client.DisconnectedAsync += async e => MqttOnDisconnected(e);
            client.ApplicationMessageReceivedAsync += async e => MqttOnMessageReceived(e);

            await client.ConnectAsync(options);

            Console.ReadLine();

            await client.DisconnectAsync();
        }
        private static async Task MqttOnConnected(MqttClientConnectedEventArgs e, IMqttClient client)
        {
            Console.WriteLine($"MQTT Client: Connected with result: {e.ConnectResult.ResultCode}");
            var topicFilter = new MqttTopicFilterBuilder()
                                                        .WithTopic("Howest/A")
                                                        .Build();
            await client.SubscribeAsync(topicFilter);
        }
        private static void MqttOnDisconnected(MqttClientDisconnectedEventArgs e)
        {
            Console.WriteLine($"MQTT Client: Broker connection lost with reason: {e.Reason}.");
        }
        private static void MqttOnMessageReceived(MqttApplicationMessageReceivedEventArgs e) => Console.WriteLine($"MQTT Client: Received Message: {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}.");
    }
}