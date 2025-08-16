using Clinic.Route.Application.Interfaces;
using Clinic.Route.Contracts;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Route.Infrastructure.Repositories
{
    public class SiglaRepository : ISiglaRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<SiglaRepository> _logger;

        public SiglaRepository(IConfiguration configuration, ILogger<SiglaRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("SiglaDb");
            _logger = logger;
        }

        public async Task<IEnumerable<ExamenPacienteDto>> ListarExamenesPacienteAsync(
            int codEmp, int codSed, int codTCl, int numOrd)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                var parametros = new DynamicParameters();
                parametros.Add("@CodEmp", codEmp, DbType.Int32);
                parametros.Add("@CodSed", codSed, DbType.Int32);
                parametros.Add("@CodTCl", codTCl, DbType.Int32);
                parametros.Add("@NumOrd", numOrd, DbType.Int32);
              

                var result = await connection.QueryAsync<ExamenPacienteDto>(
                    "SP_LISTAR_EXAMENES_PACIENTES",
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
    }
}
