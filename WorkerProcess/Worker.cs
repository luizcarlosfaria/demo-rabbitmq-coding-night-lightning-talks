using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WorkerProcess
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connectionFactory = new ConnectionFactory()
            {
                HostName = "message_broker",
                Port = 5672,
                UserName = "user",
                Password = "password",
            };

            using var connection = connectionFactory.CreateConnection();

            using var rabbitMQmodel = connection.CreateModel();

            rabbitMQmodel.QueueDeclare("coding-night", true, false, false, null);


            var consumer = new EventingBasicConsumer(rabbitMQmodel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    Console.WriteLine(message);

                    /*##########*/
                    //Business Code
                    /*##########*/
                    rabbitMQmodel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception)
                {
                    rabbitMQmodel.BasicNack(ea.DeliveryTag, false, true);
                    throw;
                }

            };
            rabbitMQmodel.BasicConsume(queue: "coding-night",
                                 autoAck: false,
                                 consumer: consumer);



            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
