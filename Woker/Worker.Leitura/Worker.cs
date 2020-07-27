using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Worker.Leitura.Configuracoes;
using Worker.Leitura.Servicos;

namespace Worker.Leitura
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServicoLeitura _servicoLeitura;
        private readonly IConfiguration _configuration;

        private  ConfiguracoesServico _configuracoesServico;
       

        public Worker(ILogger<Worker> logger, IServicoLeitura servicoLeitura, IConfiguration configuration)
        {
            _logger = logger;
            _servicoLeitura = servicoLeitura;
            _configuration = configuration;

            ResolverConfiguracaoDoServico();
        }

        private void ResolverConfiguracaoDoServico()
        {
            _configuracoesServico = new ConfiguracoesServico();
            new ConfigureFromConfigurationOptions<ConfiguracoesServico>(
                _configuration.GetSection("ConfiguracoesServico")).Configure(_configuracoesServico);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _servicoLeitura.ConsumirFila();

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(_configuracoesServico.Intervalo, stoppingToken);
            }
        }
    }
}
