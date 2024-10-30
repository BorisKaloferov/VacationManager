using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer
{
    public enum Role
    {
        CEO,
        Developer,
        [Display(Name = "Team Lead")]
        TeamLead,
        Unassigned
    }
}
