﻿using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
	public class AccountController : BaseApiController
	{
		private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
		{
			_context = context;
            _tokenService = tokenService;
        }

		[HttpPost("register")]
		public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
		{
			if (await DoesUserExists(registerDto.Username)) return BadRequest("Username is already taken");

			using var hmac = new HMACSHA512();
			var user = new AppUser()
			{
				UserName = registerDto.Username.ToLower(),
				PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
				PasswordSalt = hmac.Key
			};

			_context.Add(user);

			await _context.SaveChangesAsync();

			var responseDto = MapUserToDto(user);

			return responseDto;
		}

		[HttpPost("login")]
		public async Task<ActionResult<UserDto>> Login(LoginDto loginDto){
			var user = await _context.Users.SingleOrDefaultAsync<AppUser>(x => x.UserName == loginDto.Username.ToLower());
			if (user == null)
				return BadRequest($"No user found with username '{loginDto.Username.ToLower()}'");

			using var hmac = new HMACSHA512(user.PasswordSalt);
			byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

			for (int i = 0; i < computedHash.Length; i++) {
				if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
			}

            var responseDto = MapUserToDto(user);

            return responseDto;
        }

		private async Task<bool> DoesUserExists(string username)
		{
			return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
		}

		private UserDto MapUserToDto(AppUser appuser)
		{
			return new UserDto(
				Username: appuser.UserName,
				Token: _tokenService.CreateToken(appuser)
				);
        }
	}
}

