using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            var connFactory = new ConnectionFactory();
            connFactory.HostName = "42.193.20.184";
            connFactory.DispatchConsumersAsync = true;
            var connection = connFactory.CreateConnection();
            string exchangeName = "exchange1";
            string queueName = "queue1";
            string routingKey = "key1";
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchangeName, "direct");
            channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(queueName, exchangeName, routingKey);




            AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;
            channel.BasicConsume(queueName, autoAck: false, consumer: consumer);
            Console.ReadLine();

            async Task Consumer_Received(object sender, BasicDeliverEventArgs args)
            {
                try
                {
                    var bytes = args.Body.ToArray();
                    string msg = Encoding.UTF8.GetString(bytes);
                    Console.WriteLine($"{DateTime.Now}收到消息了:{msg}");
                    channel.BasicAck(args.DeliveryTag, multiple: false);
                    await Task.Delay(800);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    channel.BasicReject(args.DeliveryTag, requeue: true);
                }
            }
        }


    }
}
