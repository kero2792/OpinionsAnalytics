using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpinionsAnalytics.Api.Entities
{
    [Table("comentarios", Schema = "dbo")]
    public class ViewComentarios
    {
        [Key]
        [Column("idReview")]
        public int IdReview { get; set; }

        [Column("idCliente")]
        public int IdCliente { get; set; }

        [Column("nombreCliente")]
        [MaxLength(255)]
        public string NombreCliente { get; set; } = string.Empty;

        [Column("idProducto")]
        public int IdProducto { get; set; }

        [Column("nombreProducto")]
        [MaxLength(255)]
        public string NombreProducto { get; set; } = string.Empty;

        [Column("comentario")]
        public string? Comentario { get; set; }

        [Column("rating")]
        public int Rating { get; set; }
    }
}