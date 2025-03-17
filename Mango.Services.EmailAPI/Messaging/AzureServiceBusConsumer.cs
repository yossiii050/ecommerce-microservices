using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Models.Dto;
using System.Text;
using Newtonsoft.Json;
using Mango.Services.EmailAPI.Services;
namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly string registerUserQueue;

        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        private ServiceBusProcessor _emailCartProcessor;
        private ServiceBusProcessor _userRegisterProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _emailService=emailService;
            _configuration = configuration;

            serviceBusConnectionString=_configuration.GetValue<string>("ServiceBusConnectionString");
        
            emailCartQueue=_configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            registerUserQueue=_configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");

            var client = new ServiceBusClient(serviceBusConnectionString);
            _emailCartProcessor=client.CreateProcessor(emailCartQueue);
            _userRegisterProcessor=client.CreateProcessor(registerUserQueue);

        }

        public async Task Start()
        {
            //email for cart
            _emailCartProcessor.ProcessMessageAsync+= OnEmailCartRequestReceived;
            _emailCartProcessor.ProcessErrorAsync+= ErrorHandler;
            await _emailCartProcessor.StartProcessingAsync();

            //email for register
            _userRegisterProcessor.ProcessMessageAsync+= OnUserRegisterRequestReceived;
            _userRegisterProcessor.ProcessErrorAsync+= ErrorHandler;
            await _userRegisterProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            //email for cart
            await _emailCartProcessor.StopProcessingAsync();
            await _emailCartProcessor.DisposeAsync();
            
            //email for register
            await _userRegisterProcessor.StopProcessingAsync();
            await _userRegisterProcessor.DisposeAsync();
        }

        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            //receive the message
            var message=args.Message;
            var body=Encoding.UTF8.GetString(message.Body);

            CartDto objMessage=JsonConvert.DeserializeObject<CartDto>(body);
            try
            {
                //TODO- try log email
                await _emailService.EmailCartAndLog(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task OnUserRegisterRequestReceived(ProcessMessageEventArgs args)
        {
            //receive the message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            string objMessage = JsonConvert.DeserializeObject<string>(body);
            try
            {
                //TODO- try log email
                await _emailService.RegisterUserEmailAndLog(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }


    }
}
