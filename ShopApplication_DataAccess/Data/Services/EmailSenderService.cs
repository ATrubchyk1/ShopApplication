using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using ShopApplication_DataAccess.Data.Repository.IRepository;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models.ViewModels;
using ShopApplication_Utility;

namespace ShopApplication_DataAccess.Data.Services;

public class EmailSenderService : IEmailSenderService
{
    private readonly IOrderHeaderRepository _orderHeaderRepository;
    private readonly IWebHostEnvironment _webHostEnvironment; //get pattern for email 
    private readonly IEmailSender _emailSender;

    public EmailSenderService(
        IOrderHeaderRepository orderHeaderRepository,
        IWebHostEnvironment webHostEnvironment,
        IEmailSender emailSender)
    {
        _orderHeaderRepository = orderHeaderRepository;
        _webHostEnvironment = webHostEnvironment;
        _emailSender = emailSender;
    }

    public async Task SendEmailAsync(int orderId,  ProductUserVM productUserVm)
    {
        var orderHeader = await _orderHeaderRepository.FirstOrDefaultAsync(x => x.Id == orderId);
        // send Email to admin
        // read and save html doc
        var pathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar +
                             "templates" + Path.DirectorySeparatorChar + "Inquiry.html";
        var subject = $"New Inquiry #{orderHeader.Id}";
        var htmlBody = "";
        using (StreamReader sr = System.IO.File.OpenText(pathToTemplate))
        {
            htmlBody = sr.ReadToEnd();
        }
        
        var productListSb = new StringBuilder();
            
        foreach (var product in productUserVm.ProductsList)
        {
            productListSb.Append($" - Name {product.Name} <span style=`font-size:14px;`>(ID: {product.Id})</span><br/>");
        }
        
        if (productUserVm.ApplicationUser != null)
        {
            var messageBody = string.Format(htmlBody,
                productUserVm.ApplicationUser.FullName,
                productUserVm.ApplicationUser.Email,
                productUserVm.ApplicationUser.PhoneNumber,
                productListSb.ToString());
            
            // send to admin Email
            await _emailSender.SendEmailAsync(Constants.AdminEmail, subject, messageBody);
        }
    }
}