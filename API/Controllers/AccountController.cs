using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExist(registerDto.Username)) return BadRequest("Username is taken !");

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {

                UserName = registerDto.Username.ToLower(),

                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key

            };


            context.Users.Add(user);
            await context.SaveChangesAsync();

            return new UserDto
            {

                Username = user.UserName,
                Token = tokenService.CreateToken(user)
            };

        }




        private async Task<bool> UserExist(string username)
        {

            return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {

            var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());
            if (user == null) return Unauthorized("Invalid username or password !");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {

                if (user.PasswordHash[i] != computedHash[i])
                {
                    return Unauthorized("Password not match !");
                }


            }

            return new UserDto
            {
                Username = user.UserName,
                Token = tokenService.CreateToken(user)

            };

        }


    }
}