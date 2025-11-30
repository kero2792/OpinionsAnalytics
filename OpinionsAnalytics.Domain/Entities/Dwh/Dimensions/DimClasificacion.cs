using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpinionsAnalytics.Domain.Entities.Dwh.Dimensions
{
    public class DimClasificacion
    {
        public int ClasificacionKey { get; set; }

        public string Clasificacion { get; set; }

        public string Descripcion { get; set; }

        public byte? ValorNumerico { get; set; }
    }
}
