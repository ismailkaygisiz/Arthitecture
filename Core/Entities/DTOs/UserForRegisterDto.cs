﻿using Core.Entities.Abstract;

namespace Core.Entities.DTOs
{
    public class UserForRegisterDto : IDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}