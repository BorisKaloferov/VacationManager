using Business_Layer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data_Layer
{
    public class IdentityContext
    {
        UserManager<User> userManager;
        private readonly VacationManagerDbContext context;

        public IdentityContext(VacationManagerDbContext context, UserManager<User> userManager)
        {
            this.userManager = userManager;
            this.context = context;
        }
        #region Seeding Data with this Project
        public async Task SeedDataAsync
            (string adminPass, string adminEmail)
        {
            int userRole = await context
                .UserRoles.CountAsync();
            if (userRole == 0)
            {
                await ConfigureAdminAccountAsync
                    (adminPass, adminEmail);
            }
        }
        public async Task ConfigureAdminAccountAsync
            (string password, string email)
        {
            User adminIdentityUser =
               await context.Users.FirstAsync();

            if (adminIdentityUser != null)
            {
                await userManager.AddToRoleAsync(adminIdentityUser,
                    Role.CEO.ToString());
                await userManager.AddPasswordAsync(adminIdentityUser,
   password);
                await userManager.SetEmailAsync(adminIdentityUser,
                    email);
            }
        }


        #endregion 
        public async Task<User> LogInUserAsync(string username, string password)
        {
            try
            {
                User userFromDb = await userManager.FindByNameAsync(username);
                if (userFromDb == null)
                {
                    return null;
                }
                else
                {
                    IdentityResult result =
                        await userManager.PasswordValidators[0]
                        .ValidateAsync(userManager, userFromDb, password);

                    if (result.Succeeded)
                    {
                        return await context.Users.FindAsync(userFromDb.Id);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region CRUD Operations

        public async Task<Tuple<IdentityResult, User>> CreateUserAsync(string username, string password, string email,
    string name, string surname, Team team, Role role)
        {
            try
            {
                // Create a new user instance and assign the basic properties.
                User user = new User();
                user.UserName = username;
                user.Email = email;
                user.Name = name;
                user.Surname = surname;
                user.Team = team; // Optionally, attach a team if provided.

                // Create the user with the specified password.
                IdentityResult result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    return new Tuple<IdentityResult, User>(result, user);
                }

                else if (role == Role.CEO)
                {
                    await userManager.AddToRoleAsync(user, Role.CEO.ToString());
                }
                else if (role == Role.Developer)
                {
                    await userManager.AddToRoleAsync(user, Role.Developer.ToString());
                }
                else if (role == Role.TeamLead)
                {
                    await userManager.AddToRoleAsync(user, Role.TeamLead.ToString());
                }
                // If the role is Unassigned, no role is added.

                return new Tuple<IdentityResult, User>(IdentityResult.Success, user);
            }
            catch (Exception ex)
            {
                IdentityError error = new IdentityError { Code = "Registration", Description = ex.Message };
                IdentityResult result = IdentityResult.Failed(error);
                return new Tuple<IdentityResult, User>(result, null);
            }
        }
        
        


        public async Task<User> ReadAsync(string key, bool useNavigationalProperties = false)
        {
            try
            {

                if (!useNavigationalProperties)
                {
                    return await userManager.FindByIdAsync(key);
                }
                else {
                    return await context.Users.Include(u => u.Team)
                                 .Include(u => u.Vacations).FirstOrDefaultAsync(u => u.Id == key);
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<User>> ReadAllAsync(bool useNavigationalProperties = false)
        {
            try
            {

                if (!useNavigationalProperties)
                {
                    return await context.Users.ToListAsync();
                }
                else
                {
                    return await context.Users.Include(u => u.Team)
                                 .Include(u => u.Vacations).ToListAsync();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateAsync(string id, string username, string name, int age)
        {
            try
            {
                if (!String.IsNullOrEmpty(username))
                {
                    User user = await context.Users.FindAsync(id);
                    user.Name = name;
                    user.UserName = username;
                    await userManager.UpdateAsync(user);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task DeleteUserByNameAsync(string username)
        {
            try
            {
                User user = await FindUserByNameAsync(username);

                if (user == null)
                {
                    throw new InvalidOperationException("User not found for deletion!");
                }

                await userManager.DeleteAsync(user);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> FindUserByNameAsync(string username)
        {
            try
            {
                // Identity returns null if there is no such username.
                return await userManager.FindByNameAsync(username);
            }
            catch (Exception)
            {
                throw;
            }
        }


        #endregion
    }
}
