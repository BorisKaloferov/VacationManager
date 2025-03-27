using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business_Layer;
using Data_Layer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVCApp.Controllers
{
    [Authorize(Roles = "CEO")]
    public class UsersController : Controller
    {
        private readonly IdentityContext _identityContext;
        private readonly UserManager<User> _userManager;

        public UsersController(IdentityContext identityContext, UserManager<User> userManager)
        {
            _identityContext = identityContext;
            _userManager = userManager;
        }

        // GET: Users
        public async Task<IActionResult> Index(string searchFirstName, string searchSurname, string searchRole)
        {
            var users = await _identityContext.ReadAllAsync(useNavigationalProperties: false);

            // Filter by first name.
            if (!string.IsNullOrEmpty(searchFirstName))
            {
                users = users.Where(u => u.Name.Contains(searchFirstName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Filter by surname.
            if (!string.IsNullOrEmpty(searchSurname))
            {
                users = users.Where(u => u.Surname.Contains(searchSurname, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Filter by role. For each user, retrieve their roles and filter accordingly.
            if (!string.IsNullOrEmpty(searchRole))
            {
                var filteredUsers = new List<User>();
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Any(r => r.Equals(searchRole, StringComparison.OrdinalIgnoreCase)))
                    {
                        filteredUsers.Add(user);
                    }
                }
                users = filteredUsers;
            }

            return View(users);
        }

        // GET: Users/Details/{id}
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _identityContext.ReadAsync(id, useNavigationalProperties: false);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string username, string password, string email, string name, string surname, int? teamId, Role role)
        {
            // Optionally, load a Team if teamId is provided.
            Team team = null;
            // If you have a TeamContext registered, you could load the team here.
            // For now, we'll leave it as null if no team is provided.

            var resultTuple = await _identityContext.CreateUserAsync(username, password, email, name, surname, team, role);
            if (resultTuple.Item1.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var error in resultTuple.Item1.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
        }

        // GET: Users/Edit/{id}
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _identityContext.ReadAsync(id, useNavigationalProperties: false);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: Users/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string username, string name, string surname)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            try
            {
                var user = await _identityContext.ReadAsync(id, useNavigationalProperties: false);
                if (user == null)
                    return NotFound();

                user.UserName = username;
                user.Name = name;
                user.Surname = surname;

                // Use UserManager.UpdateAsync to persist changes.
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(user);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        // GET: Users/Delete/{id}
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _identityContext.ReadAsync(id, useNavigationalProperties: true);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: Users/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _identityContext.ReadAsync(id, useNavigationalProperties: false);
            if (user == null)
                return NotFound();

            // Delete by username (or by id if you implement such a method).
            await _identityContext.DeleteUserByNameAsync(user.UserName);
            return RedirectToAction(nameof(Index));
        }
    }
}
