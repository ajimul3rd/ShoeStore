﻿
using ShoeStore.Shared.Dto;
namespace ShoeStore.Auth
{
    public class AuthResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public UserInfoDto? UserInfo { get; set; }
    }
}
