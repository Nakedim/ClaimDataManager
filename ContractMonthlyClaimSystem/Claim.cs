using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractMonthlyClaimSystem_
{
    public class Claim
    {
       
            public int ClaimId { get; set; }
            public string? LecturerName { get; set; }
           
            public string ModuleCode { get; set; }

            public decimal HoursWorked { get; set; }

            public decimal HourlyRate { get; set; }
            public decimal TotalAmount => HoursWorked * HourlyRate;



            public string ClaimMonth { get; set; }

            public string Status { get; set; } = "Draft";
        }
}
