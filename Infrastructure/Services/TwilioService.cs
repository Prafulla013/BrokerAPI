using Application.Common.Interfaces;
using Common.Configurations;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Infrastructure.Services
{
    public class TwilioService : ITwilioService
    {
        private readonly TwilioConfiguration _twilioConfiguraiton;
        public TwilioService(IOptions<TwilioConfiguration> twilioOptions)
        {
            _twilioConfiguraiton = twilioOptions.Value;
            TwilioClient.Init(_twilioConfiguraiton.AccountId, _twilioConfiguraiton.Token);
        }

        public async Task SendSMSAsync(string receiver, string content)
        {
            if (_twilioConfiguraiton.IsTestMode)
            {
                receiver = _twilioConfiguraiton.ReceiverPhoneNumber;
            }

            var messageResource = await MessageResource.CreateAsync(body: content,
                                                                    from: new Twilio.Types.PhoneNumber(_twilioConfiguraiton.PhoneNumber),
                                                                    to: new Twilio.Types.PhoneNumber(receiver));

            Console.WriteLine($"SID: {messageResource.Sid} \n STATUS: {messageResource.Status}");
        }
    }
}
