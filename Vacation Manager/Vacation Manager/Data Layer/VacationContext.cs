using Business_Layer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Layer
{
	internal class VacationContext : IDb<Vacation, int>
	{
		private readonly VacationManagerDbContext _vacationManagerDbContext;

		public VacationContext(VacationManagerDbContext vacationManagerDbContext)
		{
			_vacationManagerDbContext = vacationManagerDbContext;
		}
		public async Task CreateAsync(Vacation entity)
		{
			try
			{
				User userFromDb = _vacationManagerDbContext.Users.Find(entity.User.Id);
				if (userFromDb is not null)
				{
					entity.User = userFromDb;
				}

			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<Vacation> ReadAsync(int key, bool useNavigationalProperties = false, bool isReadOnly = true)
		{

			try
			{
				IQueryable<Vacation> query = _vacationManagerDbContext.Vacations;

				if (useNavigationalProperties)
				{
					query = query.Include(v => v.User);
				}

				if (isReadOnly)
				{
					query = query.AsNoTrackingWithIdentityResolution();
				}

				return await query.SingleOrDefaultAsync(v => v.Id == key);
			}
			catch (Exception)
			{
				throw;
			}
		}


		public async Task<List<Vacation	>> ReadAllAsync(bool useNavigationalProperties = false, bool isReadOnly = true)
		{

			try
			{
				IQueryable<Vacation> query = _vacationManagerDbContext.Vacations;

				if (useNavigationalProperties)
				{
					query = query.Include(v => v.User);
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


		public async Task UpdateAsync(Vacation entity, bool useNavigationalProperties = false)
		{
			Vacation vacationFromDb = await ReadAsync(entity.Id, useNavigationalProperties, false);

			if (vacationFromDb is null)
			{
				throw new ArgumentException("Vacation with id " + entity.Id + " does not exist!");
			}

			_vacationManagerDbContext.Entry(vacationFromDb).CurrentValues.SetValues(entity);
			if (useNavigationalProperties)
			{
				User userFromDb = _vacationManagerDbContext.Users.Find(entity.User.Id);
				if (userFromDb is not null)
				{
					entity.User = userFromDb;
				}
			}

			await _vacationManagerDbContext.SaveChangesAsync();
		}

		public async Task DeleteAsync(int key)
		{
			try
			{
				Vacation vacationFromDb = await ReadAsync(key, false, false);

				if (vacationFromDb is null)
				{
					throw new ArgumentException("Vacation with id " + key + " does not exist!");
				}

				_vacationManagerDbContext.Vacations.Remove(vacationFromDb);
				await _vacationManagerDbContext.SaveChangesAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
