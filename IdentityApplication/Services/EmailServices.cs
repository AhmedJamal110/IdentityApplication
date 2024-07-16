using IdentityApplication.API.Dto.AccountDto;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace IdentityApplication.API.Services
{
    public class EmailServices
    {
        private readonly IConfiguration _config;

        public EmailServices(IConfiguration config)
        {
            _config = config;
        }
        public async Task<bool> sendEmailAsync(EmailSendDto model)
        {

            MailjetClient client = new MailjetClient(_config["MailJet:ApiKey"], _config["MailJet:SecretKey"]);


            var email = new TransactionalEmailBuilder()
                    .WithFrom(new SendContact(_config["Email:from"], _config["Email:ApplicationName"]))
                    .WithTo(new SendContact(model.To))
                    .WithHtmlPart(model.Body)
                    .WithSubject(model.Subject)
                    .Build();

            var response = await client.SendTransactionalEmailAsync(email);

            if (response.Messages != null)
            {
                if (response.Messages[0].Status == "success")
                {
                    return true;
                }
            }


            return false;


        }

    }
}
