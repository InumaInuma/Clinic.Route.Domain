using Clinic.Route.Contracts;
using Clinic.Route.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Route.Application.Interfaces
{
    public interface IExamenRepository
    {
        Task<IEnumerable<ExamenPaciente>> ListarExamenesPacienteAsync(int codEmp, int codSed, int codTCl, int numOrd);
        Task<IEnumerable<ExamenesDeProcesos>> ListarExamenesDeProcesosAsync(int codEmp, int codSed, int codTCl, int numOrd, int IdAmbi);
        Task<int> LoginPorDni(string NroDId);
        void SubscribeExamenes(int codEmp, int codSed, int codTCl, int numOrd, Func<Task> onChangeResubscribe);
    }
}
