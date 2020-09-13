using System;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Payment.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Payment
{
       class Program
    {
        // For events work how handlers effectively.
        private static readonly AutoResetEvent _waitHandle = new AutoResetEvent(false);
        private static ConnectionFactory myFactory ()
        {
            // RabbitMQ Doc.
            // https://www.rabbitmq.com/dotnet-api-guide.html
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri(Environment.GetEnvironmentVariable("conRabbitMQ"));
            return factory;
        }

        static void Main(string[] args)
        {
            ConnectionFactory factory = myFactory();
            string queue = "order_queue";
            // All process of Consume.
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {           
                // Queue Declaration ( Is necessary ).
                channel.QueueDeclare(queue: queue,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                // Consume process based on rabbitmq events
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += consumeMessages;
                // this consumer tag identifies the subscription
                // when it has to be cancelled

                // Consume process
                String consumerTag = channel.BasicConsume(queue, true, consumer);

                Console.WriteLine("Aguardando mensagens para processamento");

                // Tratando o encerramento da aplicação com
                // Control + C ou Control + Break
                Console.CancelKeyPress += (o, e) =>
                {
                    Console.WriteLine("\n Saindo...");

                    // Libera a continuação da thread principal
                    _waitHandle.Set();
                    e.Cancel = true;
                };

                // Aguarda que o evento CancelKeyPress ocorra
                _waitHandle.WaitOne();
            }
        }
        private static void consumeMessages(object sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            Order order = JsonConvert.DeserializeObject<Order>(message);
            Console.WriteLine(Environment.NewLine +
            $" {order.id} >>[Nova mensagem recebida] " + message);
            aprovProcess(order);
            notifyMessages(order);
        }

        private static void aprovProcess(Order order)
        { 
            Random rand = new Random(); int n = rand.Next(1,4);
            // Fake process
            if (n <= 2)order.Stats = "Aprovado";
            else order.Stats = "Reprovado";
            Console.WriteLine($"{order.id} >> Pagamento foi: {order.Stats}");
        }

        private static void notifyMessages(Order order)
        {
            ConnectionFactory factory = myFactory();
            string queue = "payment_queue";
            string exchange = "payment_ex";
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {           
                        // Exchange
                        channel.ExchangeDeclare(exchange: exchange, type:ExchangeType.Direct);

                        // Queue
                        channel.QueueDeclare(queue: queue,
                                            durable: false,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);

                        // Binding Ex with Queue
                        channel.QueueBind(queue, exchange, "");
                        
                        // Serealize Order
                        string message = JsonConvert.SerializeObject(order);
                        var body = Encoding.UTF8.GetBytes(message);

                        // Publish
                        channel.BasicPublish(exchange: exchange,
                                            routingKey: "",
                                            basicProperties: null,
                                            body: body);
            }

            Console.WriteLine($"{order.id} >> Ordem de Serviço teve seu pagamento Processado.");
        }
    }
}
