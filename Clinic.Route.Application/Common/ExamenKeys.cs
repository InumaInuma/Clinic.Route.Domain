using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Route.Application.Helpers
{
    public static class ExamenKeys
    {
        //public static string BuildGroupKey(int codEmp, int codSed, int codTCl, int numOrd)
        //    => $"{codEmp}-{codSed}-{codTCl}-{numOrd}";
        /// <summary>
        /// Clase auxiliar para construir claves únicas de grupo
        /// que identifican los exámenes de un paciente en un contexto específico.
        /// </summary>
        public static class GroupKeyBuilder
        {
            /// <summary>
            /// Construye una clave única a partir de los parámetros principales.
            /// Ejemplo: "1-2-3-456"
            /// </summary>
            public static string Build(int codEmp, int codSed, int codTCl, int numOrd)
                => $"{codEmp}-{codSed}-{codTCl}-{numOrd}";
        }
    }
}
