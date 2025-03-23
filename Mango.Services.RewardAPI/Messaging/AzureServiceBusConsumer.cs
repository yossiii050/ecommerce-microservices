using Azure.Messaging.ServiceBus;

using System.Text;
using Newtonsoft.Json;
using Mango.Services.RewardAPI.Services;
using Mango.Services.RewardAPI.Message;

namespace Mango.Services.RewardAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string OrderCreatedTopic;
        private readonly string OrderCreatedRewardSunscription;

        private readonly IConfiguration _configuration;
        private readonly RewardService _rewardService;

        private ServiceBusProcessor _rewardProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, RewardService rewardService)
        {
            _rewardService=rewardService;
            _configuration = configuration;

            serviceBusConnectionString=_configuration.GetValue<string>("ServiceBusConnectionString");

            OrderCreatedTopic=_configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            OrderCreatedRewardSunscription=_configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Rewards_subscription");

            var client = new ServiceBusClient(serviceBusConnectionString);
            _rewardProcessor=client.CreateProcessor(OrderCreatedTopic, OrderCreatedRewardSunscription);

        }

        public async Task Start()
        {
            _rewardProcessor.ProcessMessageAsync+= OnNewOrderRewardsRequestReceived;
            _rewardProcessor.ProcessErrorAsync+= ErrorHandler;
            await _rewardProcessor.StartProcessingAsync();

        }

        public async Task Stop()
        {
            await _rewardProcessor.StopProcessingAsync();
            await _rewardProcessor.DisposeAsync();
            

        }

        private async Task OnNewOrderRewardsRequestReceived(ProcessMessageEventArgs args)
        {
            //receive the message
            var message=args.Message;
            var body=Encoding.UTF8.GetString(message.Body);

            RewardsMessage objMessage =JsonConvert.DeserializeObject<RewardsMessage>(body);
            try
            {
                await _rewardService.UpdateRewards(objMessage);
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
