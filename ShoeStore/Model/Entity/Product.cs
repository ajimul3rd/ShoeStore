using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoeStore.Model.Entity
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        [Required]
        [ForeignKey("Categories")]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        // Navigation properties
        public Categories? Categories { get; set; }
        public  ProductDetails? ProductDetails { get; set; }
        public  List<ProductVariant>? ProductVariant { get; set; }
    }
}