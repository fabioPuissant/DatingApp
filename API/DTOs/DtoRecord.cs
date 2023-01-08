using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public record LoginDto([Required] string Username, [Required] string Password);
    public record RegisterDto([Required] string Username, [Required] string Password);
    public record UserDto(string Username, string Token);


}

