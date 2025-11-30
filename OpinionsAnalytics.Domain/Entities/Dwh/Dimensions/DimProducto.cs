using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpinionsAnalytics.Domain.Entities.Dwh.Dimensions
{
    public class DimProducto
    {
        public int ProductoKey { get; set; }

        public int IdProducto { get; set; }

        public string NombreProducto { get; set; }

        public string Categoria { get; set; }

        public DateOnly? FechaCreacion { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public bool? RegistroActivo { get; set; }
    }
}
