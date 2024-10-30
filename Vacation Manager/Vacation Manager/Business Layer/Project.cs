using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public List<Team> Teams { get; set; }

        public Project()
        {
            Teams = new List<Team>();
        }

        public Project(string name)
        {
            Name = name;
            Teams = new List<Team>();
        }

        public Project(string name, string description)
        {
            Name = name;
            Description = description;
            Teams = new List<Team>();
        }
    }
}
