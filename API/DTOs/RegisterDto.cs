using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace API.DTOs
{
    public class RegisterDto
    {

        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}