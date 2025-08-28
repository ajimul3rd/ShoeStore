using System.ComponentModel.DataAnnotations;

namespace ShoeStore.Shared.Dto
{
    public class MasterColorPaletteDto
    {
       
        public int? ColorId { get; set; }

        [Required]
        [MaxLength(100)]
        public string? ColorName { get; set; }
    }
}
