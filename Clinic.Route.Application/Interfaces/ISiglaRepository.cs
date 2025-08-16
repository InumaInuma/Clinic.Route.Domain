using Clinic.Route.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Route.Application.Interfaces
{
    public interface ISiglaRepository
    {
        Task<IEnumerable<ExamenPacienteDto>> ListarExamenesPacienteAsync(int codEmp, int codSed, int codTCl, int numOrd);
       

    }
}
