using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ShoeStore.Shared.Dto
{
    public class ProductVariantDto
    {
        public int? VariantsId { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Color { get; set; }

        // JSON serialization for array of strings
        [Column(TypeName = "nvarchar(max)")]
        public string? ImageUrlsJson { get; set; }

        public List<SizeVariantDto>? SizeVariant { get; set; }
        // Navigation properties
        [JsonIgnore]
        public ProductDto? ProductDto { get; set; }

        // Helper property for easier access to image URLs
        [NotMapped]
        public string[] ImageUrls
        {
            get => string.IsNullOrEmpty(ImageUrlsJson) ? new string[0] : JsonSerializer.Deserialize<string[]>(ImageUrlsJson);
            set => ImageUrlsJson = JsonSerializer.Serialize(value);
        }
    }

}