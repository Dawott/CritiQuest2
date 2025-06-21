﻿namespace CritiQuest2.Server.Model.DTOs
{
   
        public class RegisterRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string DisplayName { get; set; } = string.Empty;
        }

        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class AuthenticationReturn
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public string Token { get; set; } = string.Empty;
            public UserDto? User { get; set; }
        }

        public class UserDto
        {
            public string Id { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string DisplayName { get; set; } = string.Empty;
            public DateTime JoinedAt { get; set; }
        }
    }
