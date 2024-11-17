using Business_Layer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Layer
{
	internal class TeamContext : IDb<Team, int>
	{
		private readonly VacationManagerDbContext _vacationManagerDbContext;

		public TeamContext(VacationManagerDbContext vacationManagerDbContext)
		{
			_vacationManagerDbContext = vacationManagerDbContext;
		}
		public async Task CreateAsync(Team entity)
		{
			try
			{
				Project projectFromDb = _vacationManagerDbContext.Projects.Find(entity.Project.Id);
				if (projectFromDb is not null)
				{
					entity.Project = projectFromDb;
				}

				User leaderFromDb = _vacationManagerDbContext.Users.Find(entity.Leader.Id);
				if (leaderFromDb is not null)
				{
					entity.Leader = leaderFromDb;
				}

				List<User> users = new List<User>();

				foreach (var item in entity.Users)
				{
					User userFromDb = _vacationManagerDbContext.Users.Find(item.Id);
					if (userFromDb is not null)
					{
						users.Add(userFromDb);
					}
					else
					{
						users.Add(item);
					}
				}
				entity.Users = users;
				_vacationManagerDbContext.Teams.Add(entity);
				await _vacationManagerDbContext.SaveChangesAsync();

			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<Team> ReadAsync(int key, bool useNavigationalProperties = false, bool isReadOnly = true)
		{

			try
			{
				IQueryable<Team> query = _vacationManagerDbContext.Teams;

				if (useNavigationalProperties)
				{
					query = query.Include(t => t.Project).Include(t => t.Leader).Include(t => t.Users);
				}

				if (isReadOnly)
				{
					query = query.AsNoTrackingWithIdentityResolution();
				}

				return await query.SingleOrDefaultAsync(t => t.Id == key);
			}
			catch (Exception)
			{
				throw;
			}
		}


		public async Task<List<Team>> ReadAllAsync(bool useNavigationalProperties = false, bool isReadOnly = true)
		{

			try
			{
				IQueryable<Team> query = _vacationManagerDbContext.Teams;

				if (useNavigationalProperties)
				{
					query = query.Include(t => t.Project).Include(t => t.Leader).Include(t => t.Users);
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


		public async Task UpdateAsync(Team entity, bool useNavigationalProperties = false)
		{
			Team teamFromDb = await ReadAsync(entity.Id, useNavigationalProperties, false);

			if (teamFromDb is null)
			{
				throw new ArgumentException("Team with id " + entity.Id + " does not exist!");
			}

			_vacationManagerDbContext.Entry(teamFromDb).CurrentValues.SetValues(entity);
			if (useNavigationalProperties)
			{
				Project projectFromDb = _vacationManagerDbContext.Projects.Find(entity.Project.Id);
				if (projectFromDb is not null)
				{
					entity.Project = projectFromDb;
				}

				User leaderFromDb = _vacationManagerDbContext.Users.Find(entity.Leader.Id);
				if (leaderFromDb is not null)
				{
					entity.Leader = leaderFromDb;
				}

				List<User> users = new List<User>();

				foreach (var item in entity.Users)
				{
					User userFromDb = _vacationManagerDbContext.Users.Find(item.Id);
					if (userFromDb is not null)
					{
						users.Add(userFromDb);
					}
					else
					{
						users.Add(item);
					}
				}
				entity.Users = users;
			}

			await _vacationManagerDbContext.SaveChangesAsync();
		}

		public async Task DeleteAsync(int key)
		{
			try
			{
				Team teamFromDb = await ReadAsync(key, false, false);

				if (teamFromDb is null)
				{
					throw new ArgumentException("Team with id " + key + " does not exist!");
				}

				_vacationManagerDbContext.Teams.Remove(teamFromDb);
				await _vacationManagerDbContext.SaveChangesAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
