using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ShopApplication_Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    public string FullName { get; set; }

    [NotMapped]
    public string StreetAddress { get; set; }
    [NotMapped]
    public string City { get; set; }
    [NotMapped]
    public string State { get; set; }
    [NotMapped]
    public string PostCode { get; set; }
}