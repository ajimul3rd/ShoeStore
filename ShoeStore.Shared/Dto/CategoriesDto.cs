using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace ShoeStore.Shared.Dto
{
    public class CategoriesDto
    {
       
        public int? CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string? CategoryName { get; set; }

        // Navigation property for products in this category
        //[JsonIgnore]
        public List<ProductDto>? ProductDto { get; set; }
    }
}