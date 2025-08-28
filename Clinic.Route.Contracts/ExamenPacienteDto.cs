using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Route.Contracts
{
    public class ExamenPacienteDto
    {
        public int CodPer { get; set; }
        public string NomPer { get; set; }
        public string NroDId { get; set; }
        public string NroTlf { get; set; }
        public int EdaPac { get; set; }
        public string FecAte { get; set; }
        public string NomCom { get; set; }
        public string CodTCh { get; set; }
        public int CodSer { get; set; }
        public string NomSer { get; set; }
        public int? estado { get; set; }
        // Lista para los sub-exámenes
        public List<SubExamenDto> SubExamenes { get; set; } = new List<SubExamenDto>();
    }

    public class SubExamenDto
    {
        // Propiedades del sub-examen
        public int CodSer { get; set; } // ID del sub-examen
        public string Nombre { get; set; } // Nombre del sub-examen

    }
}
