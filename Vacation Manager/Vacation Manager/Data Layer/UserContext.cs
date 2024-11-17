using Business_Layer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Layer
{
    internal class UserContext : IDb<User, string>
    {
        private readonly VacationManagerDbContext _vacationManagerDbContext;

        public UserContext(VacationManagerDbContext vacationManagerDbContext)
        {
            _vacationManagerDbContext = vacationManagerDbContext;
        }
        public async Task CreateAsync(User entity)
        {
            try
            {
                Team teamFromDb = _vacationManagerDbContext.Teams.Find(entity.Team.Id);
                if (teamFromDb is not null) 
                { 
                    entity.Team = teamFromDb;
                }

                List<Vacation> vacations = new List<Vacation>();

                foreach (var item in entity.Vacations)
                {
                    Vacation vacationFromDb = _vacationManagerDbContext.Vacations.Find(item.Id);
                    if (vacationFromDb is not null)
                    {
                        vacations.Add(vacationFromDb);
                    }
                    else
                    {
                        vacations.Add(item);
                    }
                }
                entity.Vacations = vacations;
                _vacationManagerDbContext.Users.Add(entity);
                await _vacationManagerDbContext.SaveChangesAsync();

            }
            catch (Exception)
            {
                throw;
            }
        }

		public async Task<User> ReadAsync(string key, bool useNavigationalProperties = false, bool isReadOnly = true)
		{

			try
			{
				IQueryable<User> query = _vacationManagerDbContext.Users;

				if (useNavigationalProperties)
				{
					query = query.Include(u => u.Team).Include(u => u.Vacations);
				}

				if (isReadOnly)
				{
					query = query.AsNoTrackingWithIdentityResolution();
				}

				return await query.SingleOrDefaultAsync(u => u.Id == key);
			}
			catch (Exception)
			{
				throw;
			}
		}


		public async Task<List<User>> ReadAllAsync(bool useNavigationalProperties = false, bool isReadOnly = true)
		{

			try
			{
				IQueryable<User> query = _vacationManagerDbContext.Users;

				if (useNavigationalProperties)
				{
					query = query.Include(u => u.Team).Include(u => u.Vacations);
				}

				if (isReadOnly)
				{
					query = query.AsNoTrackingWithIdentityResolution();
				}

				return await query.ToListAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}


		public async Task UpdateAsync(User entity, bool useNavigationalProperties = false)
		{
			User userFromDb = await ReadAsync(entity.Id, useNavigationalProperties, false);

			if (userFromDb is null)
			{
				throw new ArgumentException("User with id " + entity.Id + " does not exist!");
			}

			_vacationManagerDbContext.Entry(userFromDb).CurrentValues.SetValues(entity);
			if (useNavigationalProperties)
			{
				Team teamFromDb = _vacationManagerDbContext.Teams.Find(entity.Team.Id);
				if (teamFromDb is not null)
				{
					entity.Team = teamFromDb;
				}

				List<Vacation> vacations = new List<Vacation>();

				foreach (var item in entity.Vacations)
				{
					Vacation vacationFromDb = _vacationManagerDbContext.Vacations.Find(item.Id);
					if (vacationFromDb is not null)
					{
						vacations.Add(vacationFromDb);
					}
					else
					{
						vacations.Add(item);
					}
				}
				entity.Vacations = vacations;
			}

			await _vacationManagerDbContext.SaveChangesAsync();
		}

        public async Task DeleteAsync(string key)
		{
			try
			{
				User user = await ReadAsync(key, false, false);

				if (user is null)
				{
					throw new ArgumentException("User with id " + key + " does not exist!");
				}

				_vacationManagerDbContext.Users.Remove(user);
				await _vacationManagerDbContext.SaveChangesAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
