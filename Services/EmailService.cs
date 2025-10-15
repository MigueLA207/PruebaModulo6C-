using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace PruebaMiguelArias.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");
        var senderEmail = emailSettings["SenderEmail"];
        var senderPassword = emailSettings["Password"];
        var smtpServer = emailSettings["SmtpServer"];
        var port = int.Parse(emailSettings["Port"]);
        var senderName = emailSettings["SenderName"];
        
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(senderName, senderEmail));
        emailMessage.To.Add(new MailboxAddress("", toEmail));
        emailMessage.Subject = subject;
        
        var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
        emailMessage.Body = bodyBuilder.ToMessageBody();

        // Usamos el cliente SMTP de MailKit
        using (var client = new SmtpClient())
        {
            // Nos conectamos de forma segura al servidor de Gmail
            await client.ConnectAsync(smtpServer, port, SecureSocketOptions.StartTls);
            // Nos autenticamos con la contraseña de aplicación
            await client.AuthenticateAsync(senderEmail, senderPassword);
            // Enviamos el correo
            await client.SendAsync(emailMessage);
            // Nos desconectamos
            await client.DisconnectAsync(true);
        }
    }
    
    
}