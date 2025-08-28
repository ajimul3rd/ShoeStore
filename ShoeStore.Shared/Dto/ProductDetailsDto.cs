using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShoeStore.Shared.Dto
{
    public class ProductDetailsDto
    {
        public int? AttributesId { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [MaxLength(50)]
        public string? ShoeType { get; set; }

        [MaxLength(50)]
        public string? Style { get; set; }

        [MaxLength(50)]
        public string? Material { get; set; }

        [MaxLength(50)]
        public string? HeelType { get; set; }

        [MaxLength(50)]
        public string? WaterResistance { get; set; }

        [MaxLength(50)]
        public string? SoleMaterial { get; set; }

        // Navigation property
        [JsonIgnore]
        public  ProductDto? ProductDto { get; set; }
    }
}