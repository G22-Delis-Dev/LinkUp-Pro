#r ""nuget: MailKit, 4.0.0""
#r ""nuget: MimeKit, 4.0.0""

using System;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

try {
    var message = new MimeMessage();
    message.From.Add(new MailboxAddress(""LinkUp Pro"", ""cieloandujar067@gmail.com""));
    message.To.Add(MailboxAddress.Parse(""cieloandujar067@gmail.com""));
    message.Subject = ""Test MailKit"";
    message.Body = new TextPart(""plain"") { Text = ""Test body"" };

    using var client = new SmtpClient();
    client.Connect(""smtp.gmail.com"", 587, SecureSocketOptions.Auto);
    client.Authenticate(""cieloandujar067@gmail.com"", ""nkobfjgukubdrwvm"");
    client.Send(message);
    client.Disconnect(true);
    Console.WriteLine(""Success"");
} catch (Exception ex) {
    Console.WriteLine(""Error: "" + ex.Message);
    if (ex.InnerException != null) Console.WriteLine(""Inner: "" + ex.InnerException.Message);
}
