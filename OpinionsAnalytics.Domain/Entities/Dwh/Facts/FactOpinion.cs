using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpinionsAnalytics.Domain.Entities.Dwh.Facts
{
    [Table("FactOpinion", Schema = "dbo")]
    public class FactOpinion
    {
        public long OpinionKey { get; set; }
        public int? ClienteKey { get; set; }
        public int ProductoKey { get; set; }
        public int FechaKey { get; set; }
        public int FuenteKey { get; set; }
        public int? ClasificacionKey { get; set; }
        public string? IdOpinionOriginal { get; set; } 
        public decimal? Rating { get; set; }
        public short? PuntajeSatisfaccion { get; set; }
        public string? Comentario { get; set; } 
        public int? LongitudComentario { get; set; }
        public bool? TieneComentario { get; set; }
        public DateTime? FechaCargaDw { get; set; }
        public string? FuenteSistema { get; set; }
    }
}