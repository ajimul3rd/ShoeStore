using System.ComponentModel.DataAnnotations;

namespace ShoeStore.Shared.Dto
{
    public class MasterSizeChartDto
    {
        public int? SizesChartId { get; set; }

        [Required]
        [MaxLength(10)]
        public string? Size { get; set; }
    }
}

