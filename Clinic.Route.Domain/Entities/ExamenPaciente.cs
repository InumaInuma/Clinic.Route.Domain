using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Route.Domain.Entities
{
    public class ExamenPaciente
    {
        public int CodPer { get; set; }
        public string NomPer { get; set; }
        public string NroDId { get; set; }
        public string NroTlf { get; set; }
        public int EdaPac { get; set; }
        public DateTime FecAte { get; set; }
        public string NomCom { get; set; }
        public string CodTCh { get; set; }
        public int CodSer { get; set; }
        public string NomSer { get; set; }
        public int Estado { get; set; }
    }
}
