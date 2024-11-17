using Business_Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Layer
{
    internal class UserContext : IDb<User, int>
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
                _vacationManagerDbContext.SaveChanges();

            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task Delete(int key)
        {
            throw new NotImplementedException();
        }

        public Task<User> Read(int key, bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> ReadAll(bool useNavigationalProperties = false, bool isReadOnly = true)
        {
            throw new NotImplementedException();
        }

        public Task Update(User entity, bool useNavigationalProperties = false)
        {
            throw new NotImplementedException();
        }
    }
}
