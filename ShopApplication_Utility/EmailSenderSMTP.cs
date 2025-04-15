using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;

namespace ShopApplication_Utility;
public class EmailSenderSMTP : IEmailSender
{
    
    private readonly IConfiguration _configuration;
    
    public EmailSenderSMTP(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        return Execute(email, subject, htmlMessage);
    }

    private Task Execute(string email, string subject, string body)
    {
        var smtpSettings = _configuration.GetSection("SMTPMail").Get<SMTPSettings>();;
        
        using (SmtpClient smtp = new SmtpClient(smtpSettings.SmtpServer, smtpSettings.Port))
        {
            smtp.Credentials = new NetworkCredential(smtpSettings.FromEmail, smtpSettings.Password);
            smtp.EnableSsl = true;
            
            MailMessage mail = new MailMessage(smtpSettings.FromEmail, email)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            try
            {
                smtp.Send(mail);
                Console.WriteLine("The letter is sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        return Task.CompletedTask;
    }
}