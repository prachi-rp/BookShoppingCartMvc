using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookShoppingCartMvcUi.Data;
using BookShoppingCartMvcUi.Models;
using BookShoppingCartMvcUi.Shared;

namespace BookShoppingCartMvcUi.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public UsersApiController(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromForm] UserDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (userDto.ImageFile != null)
            {
                if (userDto.ImageFile.Length > 1 * 1024 * 1024)
                {
                    return BadRequest(new { message = "Image file cannot exceed 1 MB." });
                }

                string[] allowedExtensions = { ".jpeg", ".jpg", ".png" };
                userDto.Image = await _fileService.SaveFile(userDto.ImageFile, allowedExtensions);
            }

            var user = new User
            {
                Name = userDto.Name,
                Password = userDto.Password,
                Email = userDto.Email,
                Phone = userDto.Phone,
                Image = userDto.Image
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromForm] UserDTO userDto)
        {
            if (id != userDto.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            string oldImage = user.Image;

            if (userDto.ImageFile != null)
            {
                if (userDto.ImageFile.Length > 1 * 1024 * 1024)
                {
                    return BadRequest(new { message = "Image file cannot exceed 1 MB." });
                }

                string[] allowedExtensions = { ".jpeg", ".jpg", ".png" };
                string newImageName = await _fileService.SaveFile(userDto.ImageFile, allowedExtensions);
                userDto.Image = newImageName;
            }

            user.Name = userDto.Name;
            user.Password = userDto.Password;
            user.Email = userDto.Email;
            user.Phone = userDto.Phone;
            user.Image = userDto.Image ?? user.Image;

            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                if (!string.IsNullOrWhiteSpace(oldImage) && oldImage != user.Image)
                {
                    _fileService.DeleteFile(oldImage);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            if (!string.IsNullOrWhiteSpace(user.Image))
            {
                _fileService.DeleteFile(user.Image);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
