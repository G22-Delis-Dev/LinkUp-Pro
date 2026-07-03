using System;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

class Program
{
    static void Main()
    {
        try {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(""LinkUp Pro"", ""cieloandujar067@gmail.com""));
            message.To.Add(MailboxAddress.Parse(""cieloandujar067@gmail.com""));
            message.Subject = ""Test MailKit"";
            message.Body = new TextPart(""plain"") { Text = ""Test body"" };

            using var client = new SmtpClient();
            client.Connect(""smtp.gmail.com"", 587, SecureSocketOptions.Auto);
            client.Authenticate(""cieloandujar067@gmail.com"", ""utgvxkzuszegdtda"");
            client.Send(message);
            client.Disconnect(true);
            Console.WriteLine(""Success"");
        } catch (Exception ex) {
            Console.WriteLine(""Error: "" + ex.Message);
        }
    }
}
