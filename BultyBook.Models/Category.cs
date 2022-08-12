using System.ComponentModel.DataAnnotations;

namespace BultyBook.Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }
    public int DisplayOrder { get; set; }

    [DataType(DataType.Date)]
    public DateTime CreatedDateTime { get; set; } = DateTime.Now;
}

