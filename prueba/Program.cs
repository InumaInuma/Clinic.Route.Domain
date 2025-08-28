using Microsoft.Data.SqlClient;
using System;
using System.Data.SqlClient;

class Program
{
    static void Main()
    {
        string cs = "Server=MCNT-78;Database=cita;User Id=sa1;Password=Med1234;TrustServerCertificate=True;";

        SqlDependency.Start(cs);

        using (var cn = new SqlConnection(cs))
        using (var cmd = new SqlCommand("SELECT Id, IdTicked, Estado, FechaCambio FROM dbo.NotificacionTrayecto", cn))
        {
            var dep = new SqlDependency(cmd);
            dep.OnChange += (_, e) =>
            {
                Console.WriteLine($"🔔 Cambio detectado: Type={e.Type}, Source={e.Source}, Info={e.Info}");
            };

            cn.Open();
            using (var rdr = cmd.ExecuteReader())
            {
                Console.WriteLine("👂 Escuchando cambios... Inserta o actualiza en NotificacionTrayecto para probar.");
                Console.ReadLine();
            }
        }

        SqlDependency.Stop(cs);
    }
}
