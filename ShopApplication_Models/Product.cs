using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApplication_Models;

public class Product
{
    public Product()
    {
        TempUnits = 1;
    }
    
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
    public string ShortDescription { get; set; }
    [Required]
    [Range(1, int.MaxValue)]
    public double Price { get; set; }
    public string Image { get; set; }

    [Display(Name = "Category Type")]
    public int CategoryId { get; set; }
    
    [Display(Name = "Application Type")]
    public int ApplicationTypeId { get; set; }
    
    [ForeignKey("CategoryId")]
    public virtual Category Category { get; set; }
    [ForeignKey("ApplicationTypeId")]
    public virtual ApplicationType ApplicationType { get; set; }

    [NotMapped] // not save in dataBase
    [Range(1, 10000, ErrorMessage = "must be greater then 0")]
    public int TempUnits { get; set; }
}