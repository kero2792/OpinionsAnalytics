using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpinionsAnalytics.Domain.Entities.Api
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Comentarios
    {
        public int idReview { get; set; }
        public int idCliente { get; set; }
        public string nombreCliente { get; set; }
        public int idProducto { get; set; }
        public string nombreProducto { get; set; }
        public string comentario { get; set; }
        public int rating { get; set; }

    }
}
