using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer
{
    public class Team
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public Project Project { get; set; }

        [ForeignKey("Project")]
        public int ProjectId { get; set; }

        [Required]
        public User Leader { get; set; }

        [Required]
        
        public string LeaderId { get; set;}

        public List<User> Users { get; set;}
        public Team()
        {
            Users = new List<User>();
        }
        public Team(int id, string name, User leader, List<User> users)
        {
            Id = id;
            Name = name;
            Leader = leader;
            LeaderId = leader.Id;
            Users = users;
        }
    }
}
