using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer
{
    public class Vacation
    {
        [Key]
        public int Id { get; set; }

        [Required] 
        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime CreationDate { get; set; }

        [Required]
        public bool IsHalfDay { get; set; }

        [Required]
        public bool IsConfirmed { get; set; }

        [Required]
        public string VacationType { get; set; }

        public string SickSheet { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        public string UserId { get; set; }

        public Vacation()
        {
            
        }

        public Vacation(string name, DateTime startDate, DateTime endDate, DateTime creationDate, bool isHalfDay, bool isConfirmed, string vacationType, string sickSheet, User user)
        {
            Name= name;
            StartDate = startDate;
            EndDate = endDate;
            CreationDate = creationDate;
            IsConfirmed = isConfirmed;
            VacationType = vacationType;
            SickSheet = sickSheet;
            User = user;
            UserId = user.Id;
            IsHalfDay = isHalfDay;
        }

        public Vacation(string name, DateTime startDate, DateTime endDate, DateTime creationDate, bool isHalfDay, bool isConfirmed, string vacationType, User user)
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            CreationDate = creationDate;
            IsConfirmed = isConfirmed;
            VacationType = vacationType;
            User = user;
            UserId = user.Id;
            IsHalfDay = isHalfDay;
        }
    }
}
