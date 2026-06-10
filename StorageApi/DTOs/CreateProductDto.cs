using System.ComponentModel.DataAnnotations;

namespace StorageApi.DTOs;

public class CreateProductDto
{
    [Required]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name needs to at least include 3-100 chars!")]
    public string Name { get; set; } = string.Empty;
    [Required]
    [Range(1,50, ErrorMessage = "The price range needs to be between 1-50!")]
    public int Price { get; set; }
    [Required]
    public string? Category { get; set; }
    [Required]
    public string? Shelf { get; set; }
    [Required]
    [Range(1, 100, ErrorMessage = "Stock value can only be between 1-100!")]
    public int Count { get; set; }
    public string? Description { get; set; }
}
