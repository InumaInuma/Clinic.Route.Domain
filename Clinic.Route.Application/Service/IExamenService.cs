using Clinic.Route.Application.Interfaces;
using Clinic.Route.Contracts;
using Clinic.Route.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Clinic.Route.Application.Helpers.ExamenKeys;

namespace Clinic.Route.Application.Service
{
    /// <summary>
    /// Contrato para el servicio de Exámenes.
    /// Define las operaciones disponibles relacionadas a los exámenes de un paciente.
    /// </summary>
    public interface IExamenService
    {
        /// <summary>
        /// Obtiene la lista de exámenes de un paciente desde la base de datos.
        /// </summary>
        Task<IEnumerable<ExamenPacienteDto>> GetExamenesAsync(int codEmp, int codSed, int codTCl, int numOrd);
        //Task<IEnumerable<ExamenesDeProcesosDto>> GetExamenesDeProcesosAsync(int codEmp, int codSed, int codTCl, int numOrd,int IdAmb);

        /// <summary>
        /// Inicializa la suscripción en tiempo real para notificar cambios en los exámenes.
        /// </summary>
        //void StartRealtimeAsync(int codEmp, int codSed, int codTCl, int numOrd);
        /// <summary>
        /// Inicializa la suscripción en tiempo real para notificar cambios en los exámenes.
        /// </summary>
        Task StartRealtimeAsync(int codEmp, int codSed, int codTCl, int numOrd); // <-- aquí cambia a Task

       
    }

    /// <summary>
    /// Implementación del servicio de exámenes.
    /// Se encarga de la lógica de aplicación relacionada a exámenes,
    /// conectando el repositorio con las notificaciones en tiempo real.
    /// </summary>
    public class ExamenService : IExamenService
    {
        private readonly IExamenRepository _repo;
        private readonly IRealtimeNotifier _notifier;

        /// <summary>
        /// Constructor: inyecta las dependencias necesarias.
        /// </summary>
        public ExamenService(IExamenRepository repo, IRealtimeNotifier notifier)
        {
            _repo = repo;
            _notifier = notifier;
        }

        // 🚨 MÉTODO MODIFICADO: Ahora construye la estructura jerárquica
        public async Task<IEnumerable<ExamenPacienteDto>> GetExamenesAsync(int cEmp, int cSed, int cTcl, int nOrd)
        {
            // Paso 1: Obtener la lista de exámenes principales del repositorio.
            var examenesPrincipales = await _repo.ListarExamenesPacienteAsync(cEmp, cSed, cTcl, nOrd);

            // Creamos una lista para almacenar los DTOs finales.
            var listaFinal = new List<ExamenPacienteDto>();

            // Paso 2 y 3: Iterar sobre los exámenes principales para obtener sus sub-exámenes.
            foreach (var examenPrincipal in examenesPrincipales)
            {
                // Mapeamos el examen principal a un DTO final.
                var examenDto = new ExamenPacienteDto
                {
                    CodPer = examenPrincipal.CodPer,
                    NomPer = examenPrincipal.NomPer ?? string.Empty,
                    NroDId = examenPrincipal.NroDId ?? string.Empty,
                    NroTlf = examenPrincipal.NroTlf ?? string.Empty,
                    EdaPac = examenPrincipal.EdaPac,
                    FecAte = examenPrincipal.FecAte.ToString("yyyy-MM-dd") ?? string.Empty,
                    NomCom = examenPrincipal.NomCom ?? string.Empty,
                    CodTCh = examenPrincipal.CodTCh,
                    CodSer = examenPrincipal.CodSer,
                    NomSer = examenPrincipal.NomSer ?? string.Empty,
                    estado = examenPrincipal.Estado  // 🚨 Mapeo del estado solo aquí
                };

                // Obtenemos los sub-exámenes. Asumo que el repositorio tiene este método.
                var subExamenes = await _repo.ListarExamenesDeProcesosAsync(cEmp, cSed, cTcl, nOrd, examenPrincipal.CodSer);
                
                // Mapeamos y anidamos los sub-exámenes en el DTO principal.
                examenDto.SubExamenes = subExamenes.Select(sub => new SubExamenDto
                {
                    CodSer = sub.CodSer, // Ajustar el mapeo según tu DTO
                    Nombre = sub.NomSer ?? string.Empty, // Ajustar el mapeo según tu DTO

                }).ToList();

                listaFinal.Add(examenDto);
            }

            // Paso 4: Devolver la lista final con la estructura jerárquica.
            return listaFinal;
        }

 

        /// <summary>
        /// Inicia un proceso de escucha en la BD para detectar cambios en los exámenes
        /// y notificar automáticamente al front-end (ej. Angular) usando SignalR.
        /// </summary>
        public async Task StartRealtimeAsync(int cEmp, int cSed, int cTcl, int nOrd)
        {
            // 1. Iniciar la suscripción para los exámenes principales
            await _repo.SubscribeExamenesAsync(cEmp, cSed, cTcl, nOrd, async () =>
            {
                // Cuando hay un cambio en TRAYECTO_TICKET, el front-end debe volver a cargar TODO.
                // Al detectar un cambio, volvemos a obtener la lista completa y anidada.
                var data = await GetExamenesAsync(cEmp, cSed, cTcl, nOrd);

                var group = GroupKeyBuilder.Build(cEmp, cSed, cTcl, nOrd);

                // Notificamos a todos los clientes del grupo con la nueva estructura
                await _notifier.NotifyExamenesActualizadosAsync(group, data);
            });


            // 2. Suscribirse a cambios en los SUB-EXÁMENES
            // No podemos depender de ListarExamenesPacienteAsync, porque puede estar vacío al inicio.
            // En su lugar, nos suscribimos a los subexámenes cuando el trigger de OrdenxServicio se activa.
            // Esto se logra con una suscripción 'genérica' que recarga todo al detectar un cambio.
            // La clave es que el trigger de NotificacionSubExamen es el que envía la notificación.

            // 🚨 Nueva lógica: Nos suscribimos de forma genérica a los cambios de subexámenes.
            // Esta suscripción también recargará los datos si se produce un cambio en OrdenxServicio.
            // La clave para este tipo de suscripción debe ser única, ya que no tiene el IdAmbi.

         
            await _repo.SubscribeSubExamenesAsync(cEmp, cSed, cTcl, nOrd, async () =>
            {
                // Al detectar un cambio en OrdenxServicio, el front-end debe volver a cargar TODO.
                var data = await GetExamenesAsync(cEmp, cSed, cTcl, nOrd);
                var group = GroupKeyBuilder.Build(cEmp, cSed, cTcl, nOrd);
                await _notifier.NotifyExamenesActualizadosAsync(group, data);
            });

            //----------------------------------------------------------------------------------------------
            //var examenesPrincipales = await _repo.ListarExamenesPacienteAsync(cEmp, cSed, cTcl, nOrd);

            //// 3. Iterar y suscribir a los subexámenes de cada examen principal
            //// Usamos Task.WhenAll para lanzar todas las suscripciones en paralelo
            //var tareasDeSuscripcion = new List<Task>();

            //foreach (var examenPrincipal in examenesPrincipales)
            //{
            //    // No necesitas Task.Run() si el método que llamas ya es asíncrono
            //    var tarea = _repo.SubscribeSubExamenesAsync(cEmp, cSed, cTcl, nOrd, examenPrincipal.CodSer, async () =>
            //    {
            //        var data = await GetExamenesAsync(cEmp, cSed, cTcl, nOrd);
            //        var group = GroupKeyBuilder.Build(cEmp, cSed, cTcl, nOrd);
            //        await _notifier.NotifyExamenesActualizadosAsync(group, data);
            //    });
            //    tareasDeSuscripcion.Add(tarea);
            //}
            //// 4. Esperar a que todas las suscripciones de subexámenes terminen.
            //await Task.WhenAll(tareasDeSuscripcion);
        }

     
    }

}
