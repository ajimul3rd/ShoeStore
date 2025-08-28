using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShoeStore.Model.Entity
{
    public class ProductDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AttributesId { get; set; }

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
        public  Product? Product { get; set; }
    }
}