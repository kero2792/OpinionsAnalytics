namespace OpinionsAnalytics.Domain.Entities.Db
{
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("webReviews", Schema = "opinionesClientes")]
    public class Resenas
    {
        public int IdReview { get; set; }

        public int IdCliente { get; set; }

        public int IdProducto { get; set; }

        public DateOnly Fecha { get; set; }

        public string Comentario { get; set; }

        public int Rating { get; set; }
    }
}
