namespace XZone.Application.Services.IServices
{
    public interface IEmailService
    {

        public Task<bool> SendMessage(string email, string subject, string message);
    }
}
