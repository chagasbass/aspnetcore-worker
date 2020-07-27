using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using Worker.Leitura.Dominio;

namespace Worker.Leitura.Servicos
{
    public class ServicoLeitura:IServicoLeitura
    {
        private readonly ILogger<ServicoLeitura> _logger;

        public ServicoLeitura(ILogger<ServicoLeitura> logger)
        {
            _logger = logger;
        }

        public void ConsumirFila()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "pedidoQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body.ToArray());
                        var pedido = JsonSerializer.Deserialize<Pedido>(message);

                        Console.WriteLine($"Order: {pedido.NumeroPedido}|{pedido.Titulo}|{pedido.Preco:N2}");

                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        //Logger
                        channel.BasicNack(ea.DeliveryTag, false, true);
                    }
                };
                channel.BasicConsume(queue: "pedidoQueue",
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
