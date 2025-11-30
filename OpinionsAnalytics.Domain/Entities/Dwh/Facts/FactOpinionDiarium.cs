using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpinionsAnalytics.Domain.Entities.Dwh.Facts
{
    public class FactOpinionDiarium
    {
        public long ResumenKey { get; set; }

        public int ProductoKey { get; set; }

        public int FechaKey { get; set; }

        public int FuenteKey { get; set; }

        public int? ClasificacionKey { get; set; }

        public int? CantidadOpiniones { get; set; }

        public decimal? PromedioRating { get; set; }

        public decimal? PromedioSatisfaccion { get; set; }

        public int? TotalPositivas { get; set; }

        public int? TotalNegativas { get; set; }

        public int? TotalNeutras { get; set; }

        public int? ClientesUnicos { get; set; }

        public DateTime? FechaCargaDw { get; set; }
    }
}
