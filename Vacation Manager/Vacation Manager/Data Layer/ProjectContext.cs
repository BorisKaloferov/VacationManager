using Business_Layer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Layer
{
	internal class ProjectContext : IDb <Project, int>
	{
		private readonly VacationManagerDbContext _vacationManagerDbContext;

		public ProjectContext(VacationManagerDbContext vacationManagerDbContext)
		{
			_vacationManagerDbContext = vacationManagerDbContext;
		}
		public async Task CreateAsync(Project entity)
		{
			try
			{

				List<Team> teams = new List<Team>();

				foreach (var item in entity.Teams)
				{
					Team teamFromDb = _vacationManagerDbContext.Teams.Find(item.Id);
					if (teamFromDb is not null)
					{
						teams.Add(teamFromDb);
					}
					else
					{
						teams.Add(item);
					}
				}
				entity.Teams = teams;
				_vacationManagerDbContext.Projects.Add(entity);
				await _vacationManagerDbContext.SaveChangesAsync();

			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<Project> ReadAsync(int key, bool useNavigationalProperties = false, bool isReadOnly = true)
		{

			try
			{
				IQueryable<Project> query = _vacationManagerDbContext.Projects;

				if (useNavigationalProperties)
				{
					query = query.Include(p => p.Teams);
				}

				if (isReadOnly)
				{
					query = query.AsNoTrackingWithIdentityResolution();
				}

				return await query.SingleOrDefaultAsync(p => p.Id == key);
			}
			catch (Exception)
			{
				throw;
			}
		}


		public async Task<List<Project>> ReadAllAsync(bool useNavigationalProperties = false, bool isReadOnly = true)
		{

			try
			{
				IQueryable<Project> query = _vacationManagerDbContext.Projects;

				if (useNavigationalProperties)
				{
					query = query.Include(u => u.Teams);
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


		public async Task UpdateAsync(Project entity, bool useNavigationalProperties = false)
		{
			Project projectFromDb = await ReadAsync(entity.Id, useNavigationalProperties, false);

			if (projectFromDb is null)
			{
				throw new ArgumentException("Project with id " + entity.Id + " does not exist!");
			}

			_vacationManagerDbContext.Entry(projectFromDb).CurrentValues.SetValues(entity);
			if (useNavigationalProperties)
			{
				List<Team> teams = new List<Team>();

				foreach (var item in entity.Teams)
				{
					Team teamFromDb = _vacationManagerDbContext.Teams.Find(item.Id);
					if (teamFromDb is not null)
					{
						teams.Add(teamFromDb);
					}
					else
					{
						teams.Add(item);
					}
				}
				entity.Teams = teams;
			}

			await _vacationManagerDbContext.SaveChangesAsync();
		}

		public async Task DeleteAsync(int key)
		{
			try
			{
				Project	 projectFromDb = await ReadAsync(key, false, false);

				if (projectFromDb is null)
				{
					throw new ArgumentException("Project with id " + key + " does not exist!");
				}

				_vacationManagerDbContext.Projects.Remove(projectFromDb);
				await _vacationManagerDbContext.SaveChangesAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
