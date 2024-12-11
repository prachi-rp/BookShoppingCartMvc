using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookShoppingCartMvcUi.Data;
using BookShoppingCartMvcUi.Models;
using BookShoppingCartMvcUi.Shared;

namespace BookShoppingCartMvcUi.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public UsersController(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            string userName = HttpContext.Session.GetString("UserName");
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] UserDTO user)
        {
            if (ModelState.IsValid)
            {
                //_context.Add(user);
                //await _context.SaveChangesAsync();
                //return RedirectToAction("Login", "Users");

                // Handle file upload
                if (user.ImageFile != null)
                {
                    if (user.ImageFile.Length > 1 * 1024 * 1024)
                    {
                        return BadRequest(new { message = "Image file cannot exceed 1 MB." });
                    }

                    string[] allowedExtensions = { ".jpeg", ".jpg", ".png" };
                    string imageName = await _fileService.SaveFile(user.ImageFile, allowedExtensions);
                    user.Image = imageName;
                }

                // Map BookDTO to Book entity
                var users = new User
                {
                    Name = user.Name,
                    Password = user.Password,
                    Email = user.Email,
                    Phone = user.Phone,
                    Image = user.Image
                };

                _context.Add(users);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Users");
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            //if (user == null)
            //{
            //    return NotFound();
            //}
            //return View(user);
            return View();
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] UserDTO userToUpdate)
{
    if (id != userToUpdate.Id)
    {
        return NotFound();
    }

    if (ModelState.IsValid)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            string oldImage = user.Image;
            if (userToUpdate.ImageFile != null)
            {
                if (userToUpdate.ImageFile.Length > 1 * 1024 * 1024)
                {
                    return BadRequest(new { message = "Image file cannot exceed 1 MB." });
                }

                string[] allowedExtensions = { ".jpeg", ".jpg", ".png" };
                string newImageName = await _fileService.SaveFile(userToUpdate.ImageFile, allowedExtensions);
                userToUpdate.Image = newImageName;
            }

            // Update the user details
            user.Name = userToUpdate.Name;
            user.Password = userToUpdate.Password;
            user.Email = userToUpdate.Email;
            user.Phone = userToUpdate.Phone;
            user.Image = userToUpdate.Image ?? user.Image;

            _context.Update(user);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(oldImage) && oldImage != user.Image)
            {
                _fileService.DeleteFile(oldImage);
            }

            return RedirectToAction("Index", "Home");
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(userToUpdate.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
    }

    // If ModelState is invalid, map the User entity back to UserDTO
    var existingUser = await _context.Users.FindAsync(id);
    if (existingUser == null)
    {
        return NotFound();
    }

    var userDTO = new UserDTO
    {
        Id = existingUser.Id,
        Name = existingUser.Name,
        Email = existingUser.Email,
        Phone = existingUser.Phone,
        Password = existingUser.Password,
        Image = existingUser.Image
    };

    return View(userDTO);
}


        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                if (!string.IsNullOrWhiteSpace(user.Image))
                {
                    _fileService.DeleteFile(user.Image);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        // GET: Users/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] User user)
        {
            var userLogin = await _context.Users.FirstOrDefaultAsync(u=> u.Name == user.Name && u.Password ==user.Password);

            if (userLogin != null)
            {
                HttpContext.Session.SetString("UserName", userLogin.Name);
                HttpContext.Session.SetInt32("UserId", userLogin.Id);
                if(userLogin.Image != null)
                {
                    HttpContext.Session.SetString("UserImagePath", userLogin.Image);
                }
                return RedirectToAction("Index","Home");
            }
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

             return RedirectToAction("Index", "Home");
        }

    }
}
