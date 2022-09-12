using System.ComponentModel.DataAnnotations;

namespace BultyBook.Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }
    [Display(Name ="Display Order")]
    public int DisplayOrder { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Create Date Time")]
    public DateTime CreatedDateTime { get; set; } = DateTime.Now;
}

