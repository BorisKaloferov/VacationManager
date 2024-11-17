using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer
{
    public class User : IdentityUser
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        public Team? Team { get; set; }

        [ForeignKey("Team")]
        public int? TeamId { get; set; }

        public List<Vacation> Vacations { get; set; }

        public User()
        {
           Vacations = new List<Vacation>();
        }

        public User(string name, string surname, Team team)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Surname = surname;
            Team = team;
            TeamId = team.Id;
            Vacations = new List<Vacation>();
        }

    }
}
