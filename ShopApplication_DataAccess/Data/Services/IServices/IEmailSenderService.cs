using ShopApplication_Models.ViewModels;

namespace ShopApplication_DataAccess.Data.Services.IServices;

public interface IEmailSenderService
{
    public Task SendEmailAsync(int orderId, ProductUserVM productUserVM);
}