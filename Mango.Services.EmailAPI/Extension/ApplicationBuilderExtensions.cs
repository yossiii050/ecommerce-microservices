using Mango.Services.EmailAPI.Messaging;
using System.Reflection.Metadata;

namespace Mango.Services.EmailAPI.Extension
{
    public static class ApplicationBuilderExtensions
    {
        private static IAzureServiceBusConsumer ServiceBusConsumer { get; set; }
        public static IApplicationBuilder UserAzuerServiceBusConsumer(this IApplicationBuilder app)
        {
            ServiceBusConsumer=app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
            var hostApplicationLife=app.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostApplicationLife.ApplicationStarted.Register(onStart);
            hostApplicationLife.ApplicationStopping.Register(onStop);

            return app;
        }

        private static void onStop()
        {
            ServiceBusConsumer.Stop();
        }

        private static void onStart()
        {
            ServiceBusConsumer.Start();
        }
    }
}
