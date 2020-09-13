using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Order.Models;
using StackExchange.Redis;

namespace Order
{
    class Program
    {
        // For events work how handlers effectively.
        private static readonly AutoResetEvent _waitHandle = new AutoResetEvent(false);

        // Conection to Reddis, for store data of Service Orders.
        // Example in:
        // https://github.com/renatogroffe/ASPNETCore3.1_StackExchange.Redis/tree/master/APICotacoes
        private static readonly ConnectionMultiplexer _conRedis = ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("conRedis"));

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
            string queue = Environment.GetEnvironmentVariable("OrderMode");
            if (queue != "checkout_queue" && queue != "payment_queue")
            {
                throw new Exception("Please, inform checkout_queue or payment_queue");
            }
            startConsume(queue);
        }

        private static void startConsume(string queue)
        {
            ConnectionFactory factory = myFactory();
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
                
                // Here, we have a dellegate that performs a consumption function at each new event. Crazy dude.
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
            string queue = Environment.GetEnvironmentVariable("OrderMode");
            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            Order.Models.Order order = JsonConvert.DeserializeObject<Order.Models.Order>(message);
            
            if (queue == "checkout_queue")
            {
                order.id = Guid.NewGuid();
            }
            
            Console.WriteLine(Environment.NewLine +
            $" {order.id} >>[Nova mensagem recebida] " + message);
            
            if (queue == "checkout_queue")
            {
                order.CreatedAt = DateTime.Now;
                order.Stats = "Pending";         
            }
            
            saveOrder(order, queue);
            
            if (queue == "checkout_queue") 
            {
                notifyMessages(order);
            }
        }


        private static void saveOrder(Order.Models.Order order, string queue)
        {
            RedisKey key = order.id.ToString();
            StackExchange.Redis.IDatabase orderKeys = _conRedis.GetDatabase();
            orderKeys.StringSet(key, JsonConvert.SerializeObject(order));
            if (queue == "checkout_queue")
            {
                Console.WriteLine($"{order.id} >> Ordem de Serviço REGISTRADA no Redis.");
            }
            else
            {
                Console.WriteLine($"{order.id} >> Ordem de Serviço ATUALIZADA no Redis.");
            }
        }

        private static void notifyMessages(Order.Models.Order order)
        {
            ConnectionFactory factory = myFactory();
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {           
                    string toQueue = "order_queue";
                    string toExchange = "order_ex";
                        // Exchange
                        channel.ExchangeDeclare(exchange: toExchange, type:ExchangeType.Direct);

                        // Queue
                        channel.QueueDeclare(queue: toQueue,
                                            durable: false,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);

                        // Binding Ex with Queue
                        channel.QueueBind(toQueue, toExchange, "");
                        
                        // Serealize Order
                        string message = JsonConvert.SerializeObject(order);
                        var body = Encoding.UTF8.GetBytes(message);

                        // Publish
                        channel.BasicPublish(exchange: toExchange,
                                            routingKey: "",
                                            basicProperties: null,
                                            body: body);
            }

            Console.WriteLine($" {order.id} >> Ordem de Serviço enviada para processar o pagamento.");
        }
    }
}
