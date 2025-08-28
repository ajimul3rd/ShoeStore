using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoeStore.Model.Entity
{
    public class MasterColorPalette
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ColorId { get; set; }

        [Required]
        [MaxLength(100)]
        public string? ColorName { get; set; }
    }
}
