using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoeStore.Model.Entity
{
    public class MasterCategories
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MasterCategoriesId { get; set; }

        [Required]
        [MaxLength(100)]
        public string? CategoryName { get; set; }
    }
}
