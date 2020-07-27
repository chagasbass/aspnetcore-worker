using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Woker.Api.Dominio;

namespace Woker.Api.Controllers
{
    [ApiController]
    [Route("v1/pedidos")]
    public class PedidoController : ControllerBase
    {
        private readonly ILogger<PedidoController> _Logger;

        public PedidoController(ILogger<PedidoController> logger)
        {
            _Logger = logger;
        }

        [HttpPost]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> InserirProduto([FromBody] Pedido pedido)
        {
            try
            {
                #region Inserindo na Fila

                var factory = new ConnectionFactory() { HostName = "localhost" };

                using var connection =  factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare(queue: "pedidoQueue",
                    durable:false,
                    exclusive:false,
                    autoDelete:false,
                    arguments:null);

                var mensagem = JsonSerializer.Serialize(pedido);
                var corpoMensagem = Encoding.UTF8.GetBytes(mensagem);

                channel.BasicPublish(exchange: "",
                    routingKey: "pedidoQueue",
                    basicProperties: null,
                    body: corpoMensagem);

                Console.WriteLine("[x] mensagem Enviada{0}", mensagem);

                #endregion

                return Accepted(pedido);
            }
            catch (System.Exception ex)
            {
                _Logger.LogError("deu erro", ex);

                return BadRequest(ex);
            }
        }

    }
}
