using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace TmntCardManager.Services;

public class EmailSender : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var fromEmail = "bileysh06@gmail.com"; 
        var appPassword = "lcdrznxdioxyyhkb\n"; 

        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromEmail, appPassword)
        };

        var message = new MailMessage(
            from: fromEmail,
            to: email,
            subject: subject,
            body: htmlMessage
        )
        {
            IsBodyHtml = true 
        };

        await client.SendMailAsync(message);
    }
}