using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpinionsAnalytics.Domain.Entities.Dwh.Dimensions
{
    public class DimFuente
    {
        public int FuenteKey { get; set; }

        public string IdFuente { get; set; }

        public string TipoFuente { get; set; }

        public string Canal { get; set; }

        public DateOnly? FechaCarga { get; set; }

        public string Descripcion { get; set; }

        public bool? RegistroActivo { get; set; }
    }
}
