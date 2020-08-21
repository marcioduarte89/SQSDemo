using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SQSClient 
{
    class Program 
    {
        static async Task Main(string[] args) 
        {
            Console.WriteLine("Write message to send to queue");
            var message = Console.ReadLine();
            var configurationRoot = GetConfiguration();
            var queue = configurationRoot.GetSection("AWS").GetSection("Queue").Value;
            var accessKeyId = configurationRoot.GetSection("AWS").GetSection("AccessKeyId").Value;
            var secretAccessKey = configurationRoot.GetSection("AWS").GetSection("SecretAccessKey").Value;
            var sessionToken = configurationRoot.GetSection("AWS").GetSection("SessionToken").Value;

            using (var sqs = new AmazonSQSClient(accessKeyId, secretAccessKey, sessionToken, RegionEndpoint.USEast2)) 
            {
                var sendMessageRequest = new SendMessageRequest(queue, message);

                var result = await sqs.SendMessageAsync(sendMessageRequest);

                Console.WriteLine($"Message result {result.MessageId}");
            }

            Console.ReadKey();
        }
        public static IConfigurationRoot GetConfiguration() 
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);

            return builder.Build();
        }
    }
}
