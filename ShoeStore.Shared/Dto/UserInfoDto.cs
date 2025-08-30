using ShoeStore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShoeStore.Shared.Dto
{
    public class UserInfoDto
    {
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRole? Role { get; set; }


    }
}
