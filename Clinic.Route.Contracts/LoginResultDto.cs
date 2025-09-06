using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Route.Contracts
{
    public class LoginResultDto
    {
        public int LoginResult { get; set; }
        public int? CodEmp { get; set; }
        public int? CodSed { get; set; }
        public int? CodTCl { get; set; }
        public int? NumOrd { get; set; }
    }
}
