using Clinic.Route.Application.Interfaces;
using Clinic.Route.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Route.Application.Features.Examenes.Queries
{
    public class GetExamenesPacienteQuery
    {
        private readonly ISiglaRepository _siglaRepository;

        public GetExamenesPacienteQuery(ISiglaRepository siglaRepository)
        {
            _siglaRepository = siglaRepository;
        }

        public async Task<IEnumerable<ExamenPacienteDto>> ExecuteAsync(int codEmp, int codSed, int codTCl, int numOrd)
        {
            if (codEmp <= 0 || codSed <= 0 || codTCl <= 0 || numOrd <= 0 )
                throw new ArgumentException("Parámetros inválidos");

            return await _siglaRepository.ListarExamenesPacienteAsync(codEmp, codSed, codTCl, numOrd);
        }
    }
}
