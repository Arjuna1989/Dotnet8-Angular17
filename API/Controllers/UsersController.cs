using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(DataContext context) : BaseApiController
    {
        private readonly DataContext _context = context;

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            return Ok(await _context.Users.ToListAsync());

        }

        [Authorize]
        [HttpGet("{Id}")]
        public async Task<ActionResult<AppUser>> GetUsers(int Id)
        {
            return Ok(await _context.Users.FirstOrDefaultAsync(x => x.Id == Id));

        }
    }
}