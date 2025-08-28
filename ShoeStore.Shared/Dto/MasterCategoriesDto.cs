using System.ComponentModel.DataAnnotations;

namespace ShoeStore.Shared.Dto
{
    public class MasterCategoriesDto
    {
      
        public int? MasterCategoriesId { get; set; }

        [Required]
        [MaxLength(100)]
        public string? CategoryName { get; set; }
    }
}
