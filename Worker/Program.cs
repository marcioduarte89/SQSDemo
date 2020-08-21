using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Worker {
    public class Program 
    {
        public static void Main(string[] args) 
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => 
                {
                    services.AddHostedService<Worker>();
                    services.AddAWSService<IAmazonSQS>(hostContext.Configuration.GetAWSOptions());
                });
    }
}
