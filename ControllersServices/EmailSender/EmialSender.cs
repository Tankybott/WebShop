using DataAccess.Repository.IRepository;
using Mailjet.Client.TransactionalEmails;
using Mailjet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Services.EmailSender
{
    public class EmailSender : IEmailSender
    {
        private readonly IMailjetClient _mailjetClient;
        private readonly IUnitOfWork _unitOfWork;

        public EmailSender(IMailjetClient mailjetClient, IUnitOfWork unitOfWork)
        {
            _mailjetClient = mailjetClient;
            _unitOfWork = unitOfWork;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var settings = await _unitOfWork.WebshopConfig.GetAsync();
            var fromEmail = "michalbukowskidev@gmail.com";
            var fromName = settings?.SiteName ?? "Default Shop";

            var mail = new TransactionalEmailBuilder()
                .WithFrom(new SendContact(fromEmail, fromName))
                .WithTo(new SendContact(email))
                .WithSubject(subject)
                .WithHtmlPart(htmlMessage)
                .Build();

            var response = await _mailjetClient.SendTransactionalEmailAsync(mail);

            if (response.Messages.Length != 1)
            {
                throw new Exception("Failed to send email.");
            }
        }
    }
}
