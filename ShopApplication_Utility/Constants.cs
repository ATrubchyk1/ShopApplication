using System.Collections.ObjectModel;

namespace ShopApplication_Utility;

public static class Constants
{
    public const string ImagePath = @"\images\product\";
    public const string SessionCart = "ShoppingSession";
    public const string SessionInquiryId = "InquirySession";
    
    public const string AdminRole = "Admin";
    public const string UserRole = "User";
    
    public const string AdminEmail = "andron23490@gmail.com";
    
    public const string CategoryName = "Category";
    public const string ApplicationTypeName = "ApplicationType";
    
    public const string Success = "Success";
    public const string Error = "Error";
    
    public const string StatusPending = "Pending";
    public const string StatusApproved = "Approved";
    public const string StatusInProcess = "Processing";
    public const string StatusShipped = "Shipped";
    public const string StatusCancelled = "Cancelled";
    public const string StatusRefunded = "Refunded";

    public static readonly IEnumerable<string> StatusList = new ReadOnlyCollection<string>(
       new List<string>
       {
           StatusPending,StatusApproved, StatusInProcess, StatusShipped, StatusCancelled, StatusRefunded
       });
}