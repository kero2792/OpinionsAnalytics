using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpinionsAnalytics.Domain.Entities.Db
{
    [Table("webReviews", Schema = "dbo")]
    public class Resenas
    {
        [Key]
        [Column("idReview")]
        public int IdReview { get; set; }

        [Column("idCliente")]
        public int IdCliente { get; set; }

        [Column("idProducto")]
        public int IdProducto { get; set; }

        [Column("fecha")]
        public DateOnly Fecha { get; set; }

        [Column("comentario")]
        public string? Comentario { get; set; }

        [Column("rating")]
        public int Rating { get; set; }
    }
}