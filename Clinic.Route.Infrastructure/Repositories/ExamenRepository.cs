using Clinic.Route.Application.Interfaces;
using Clinic.Route.Contracts;
using Clinic.Route.Domain.Entities;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Route.Infrastructure.Repositories
{
    public class ExamenRepository : IExamenRepository
    {
        private readonly string _cs;
        private readonly ILogger<ExamenRepository> _logger;
        private static bool _sqlDependencyStarted = false;

        // 🔒 Mantener vivas las suscripciones por grupo (codEmp-codSed-codTCl-numOrd)
        private class ActiveSub
        {
            public SqlConnection? Connection { get; set; }
            public SqlCommand? Command { get; set; }
            public SqlDataReader? Reader { get; set; }
            public SqlDependency? Dependency { get; set; }
        }

        private readonly ConcurrentDictionary<string, ActiveSub> _subs = new();

        public ExamenRepository(IConfiguration config, ILogger<ExamenRepository> logger)
        {
            _cs = config.GetConnectionString("SiglaDb")!;
            _logger = logger;
        }

        public async Task<IEnumerable<ExamenPaciente>> ListarExamenesPacienteAsync(int codEmp, int codSed, int codTCl, int numOrd)
        {
           
            try
            {
                using var connection = new SqlConnection(_cs);
                var parametros = new DynamicParameters();
                parametros.Add("@CodEmp", codEmp, DbType.Int32);
                parametros.Add("@CodSed", codSed, DbType.Int32);
                parametros.Add("@CodTCl", codTCl, DbType.Int32);
                parametros.Add("@NumOrd", numOrd, DbType.Int32);


                var result = await connection.QueryAsync<ExamenPaciente>(
                    "SP_LISTAR_EXAMENES_PACIENTES_PROCESOS",
                    parametros,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error SQL en ListarExamenesPacienteAsync");
                throw new Exception("Ocurrió un error al obtener los exámenes del paciente desde la base de datos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en ListarExamenesPacienteAsync");
                throw;
            }
        }
        //procedimiento almacenado ahora devuelve un solo valor entero
       public async Task<int> LoginPorDni (string NroDId)
        {
            try
            {
                using var connection = new SqlConnection(_cs);
                var parametros = new DynamicParameters();
                parametros.Add("@NroDni", NroDId, DbType.String);

                // Usamos ExecuteScalarAsync para obtener un solo valor (el 0 o 1)
                var result = await connection.ExecuteScalarAsync<int>(
                    "SP_LOGINPOR_DNI_AUDITORIA",
                    parametros,
                    commandType: CommandType.StoredProcedure
                );

                // Devolvemos el resultado directamente
                return result;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error SQL en LoginPorDni");
                throw new Exception("Ocurrió un error al logearse el paciente desde la base de datos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en LoginPorDni");
                throw;
            }
        }
        public async Task<IEnumerable<ExamenesDeProcesos>> ListarExamenesDeProcesosAsync(int codEmp, int codSed, int codTCl, int numOrd,int IdAmbi)
        {

            try
            {
                using var connection = new SqlConnection(_cs);
                var parametros = new DynamicParameters();
                parametros.Add("@CodEmp", codEmp, DbType.Int32);
                parametros.Add("@CodSed", codSed, DbType.Int32);
                parametros.Add("@CodTCl", codTCl, DbType.Int32);
                parametros.Add("@NumOrd", numOrd, DbType.Int32);
                parametros.Add("@IdAmbi", IdAmbi, DbType.Int32);


                var result = await connection.QueryAsync<ExamenesDeProcesos>(
                    "SP_LISTAR_EXAMENESXPROCESOS_PACIENTES",
                    parametros,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error SQL en ListarExamenesDeProcesosAsync");
                throw new Exception("Ocurrió un error al obtener los exámenes del paciente desde la base de datos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en ListarExamenesDeProcesosAsync");
                throw;
            }
        }

        public void SubscribeExamenes(int codEmp, int codSed, int codTCl, int numOrd, Func<Task> onChangeResubscribe)
        {
            if (!_sqlDependencyStarted)
            {
                SqlDependency.Start(_cs);
                _sqlDependencyStarted = true;
                _logger.LogInformation("🚀 SqlDependency iniciado");
            }
            var key = $"{codEmp}-{codSed}-{codTCl}-{numOrd}";
            //Suscribir(codEmp, codSed, codTCl, numOrd, onChangeResubscribe);
            Suscribir(key, codEmp, codSed, codTCl, numOrd, onChangeResubscribe);
        }

        private void Suscribir(string key, int codEmp, int codSed, int codTCl, int numOrd, Func<Task> onChangeResubscribe)
        {
            try
            {
                // Si ya existe, primero intentamos limpiar para re-crear
                if (_subs.TryRemove(key, out var old))
                {
                    try { old.Reader?.Close(); } catch { }
                    try { old.Connection?.Close(); } catch { }
                    try { old.Reader?.Dispose(); } catch { }
                    try { old.Command?.Dispose(); } catch { }
                    try { old.Connection?.Dispose(); } catch { }
                }

                var cn = new SqlConnection(_cs);
                var cmd = new SqlCommand(@"
                                 SELECT Id,
                                      IdTicked,
                                      Estado,
                                      FechaCambio,
                                      IdAmbiente 
                                 FROM dbo.NotificacionTrayecto 
                                 WHERE CodEmp = @CodEmp
                                 AND CodSed = @CodSed
                                 AND CodTCl = @CodTCl
                                 AND NumOrd = @NumOrd", cn);

                //cmd.Parameters.Add("@CodEmp", SqlDbType.Int).Value = codEmp;
                //cmd.Parameters.Add("@CodSed", SqlDbType.Int).Value = codSed;
                //cmd.Parameters.Add("@CodTCl", SqlDbType.Int).Value = codTCl;
                //cmd.Parameters.Add("@NumOrd", SqlDbType.Int).Value = numOrd;
                cmd.Parameters.Add("@CodEmp", SqlDbType.Int).Value = codEmp;
                cmd.Parameters.Add("@CodSed", SqlDbType.Int).Value = codSed;
                cmd.Parameters.Add("@CodTCl", SqlDbType.Int).Value = codTCl;
                cmd.Parameters.Add("@NumOrd", SqlDbType.Int).Value = numOrd;

                var dep = new SqlDependency(cmd);
                dep.OnChange += async (_, args) =>
                {
                    _logger.LogInformation("🔔 SqlDependency cambio detectado. Info: Type={Type} | Source={Source} | Info={Info}",
                                            args.Type, args.Source, args.Info);

                    try
                    {
                        await onChangeResubscribe();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error en OnChange de SqlDependency");
                    }
                    finally
                    {
                        // Pequeño delay para evitar loops si hay errores
                        await Task.Delay(400);
                        Suscribir(key, codEmp, codSed, codTCl, numOrd, onChangeResubscribe);
                    }
                };

                cn.Open();

                // ⚠️ IMPORTANTE: NO usar using; guardar el reader para que la suscripción quede viva
                var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                _subs[key] = new ActiveSub
                {
                    Connection = cn,
                    Command = cmd,
                    Reader = reader,
                    Dependency = dep
                };

                _logger.LogInformation("👂 Suscrito SqlDependency para clave {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando suscripción SqlDependency para clave {Key}", key);
                // Intentar reintentar después de un breve tiempo
                _ = Task.Run(async () =>
                {
                    await Task.Delay(1500);
                    Suscribir(key, codEmp, codSed, codTCl, numOrd, onChangeResubscribe);
                });
            }


        }

    }
}
