namespace SapSecurity.Services.Sms;

public interface ISmsManager
{
    /// <summary>
    /// send sms to user
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    Task<bool> SendSms(string phoneNumber, string message);
}