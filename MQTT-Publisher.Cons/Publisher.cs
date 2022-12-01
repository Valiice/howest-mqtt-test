using MQTTnet;
using MQTTnet.Client;
using System;

namespace MQTT_Test.Cons
{
    internal class Publisher
    {
        static async Task Main(string[] args){
            
            var mqttFactory = new MqttFactory();
            IMqttClient client = mqttFactory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                            .WithClientId(Guid.NewGuid().ToString())
                            .WithTcpServer("https://test.mosquitto.org/", 1883)
                            .WithCredentials("", "")
                            .WithTls()
                            .WithCleanSession()
                            .Build();
            client.ConnectedAsync += async e => MqttOnConnected(e);
            client.DisconnectedAsync += async e => MqttOnDisconnected(e);

            await client.ConnectAsync(options);

            Console.WriteLine("Please press a key to publish the message");

            string payload = Console.ReadLine();

            await PublicMessageAsync(client, payload);

            await client.DisconnectAsync();
        }

        private static async Task PublicMessageAsync(IMqttClient client, string payload)
        {
            string messagePayload = payload;
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("Howest/A")
                .WithPayload(messagePayload)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            if (client.IsConnected)
                await client.PublishAsync(message);
        }
        private static void MqttOnConnected(MqttClientConnectedEventArgs e) => Console.WriteLine($"MQTT Client: Connected with result: {e.ConnectResult.ResultCode}");
        private static void MqttOnDisconnected(MqttClientDisconnectedEventArgs e) => Console.WriteLine($"MQTT Client: Broker connection lost with reason: {e.Reason}.");
    }
}