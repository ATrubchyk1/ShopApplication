namespace ShopApplication_Utility;

public class SMTPSettings
{
    public string SmtpServer { get; set; }
    public int Port { get; set; }
    public string FromEmail { get; set; }
    public string Password { get; set; }
}