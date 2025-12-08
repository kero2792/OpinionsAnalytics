using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpinionsAnalytics.Domain.Entities.Dwh.Dimensions
{
    [Table("DimClasificacion", Schema = "dbo")]
    public class DimClasificacion
    {
        public int ClasificacionKey { get; set; }
        public string? Clasificacion { get; set; }
        public string? Descripcion { get; set; }
        public byte? ValorNumerico { get; set; }
    }
}