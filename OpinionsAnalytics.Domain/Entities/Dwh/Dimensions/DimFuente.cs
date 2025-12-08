using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpinionsAnalytics.Domain.Entities.Dwh.Dimensions
{
    [Table("DimFuente", Schema = "dbo")]
    public class DimFuente
    {
        public int FuenteKey { get; set; }
        public string? IdFuente { get; set; }
        public string? TipoFuente { get; set; }
        public string? Canal { get; set; }
        public DateOnly? FechaCarga { get; set; }
        public string? Descripcion { get; set; }
        public bool? RegistroActivo { get; set; }
    }
}