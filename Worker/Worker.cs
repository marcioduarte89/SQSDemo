using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Worker 
{
    public class Worker : BackgroundService 
    {

        private readonly ILogger<Worker> _logger;
        private readonly IAmazonSQS _amazonSqs;
        private readonly string _queueName;

        public Worker(ILogger<Worker> logger, IAmazonSQS amazonSqs, IConfiguration configuration)
        {
            _logger = logger;
            _amazonSqs = amazonSqs;
            _queueName = configuration.GetValue<string>("AWS:Queue");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var receiveMessage = new ReceiveMessageRequest(_queueName) {
                    WaitTimeSeconds = 20 // long polling
                };

                var resultMessage = await _amazonSqs.ReceiveMessageAsync(receiveMessage, stoppingToken);

                foreach (var message in resultMessage.Messages) {

                    _logger.LogInformation("Message id: {messageId}", message.MessageId);
                    _logger.LogInformation("Message body: {messageBody}", message.Body);
                    _logger.LogInformation("Message receipt handle: {messageReceiptHandle}", message.ReceiptHandle);

                    var deleteMessage = new DeleteMessageRequest(_queueName, message.ReceiptHandle);
                    var response = await _amazonSqs.DeleteMessageAsync(deleteMessage, stoppingToken);
                    _logger.LogInformation("Deleted message status: {status}", response.HttpStatusCode);
                }
            }
        }
    }
}
