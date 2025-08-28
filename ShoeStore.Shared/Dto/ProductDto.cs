using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoeStore.Shared.Dto
{
    public class ProductDto
    {
        public int? ProductId { get; set; }

        [Required]
        [ForeignKey("Categories")]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        // Navigation properties
        public CategoriesDto? CategoriesDto { get; set; }
        public  ProductDetailsDto? ProductDetailsDto { get; set; }
        public  List<ProductVariantDto>? ProductVariantDto { get; set; }
    }
}