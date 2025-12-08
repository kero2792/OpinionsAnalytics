using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpinionsAnalytics.Domain.Entities.Dwh.Dimensions
{
    [Table("DimProducto", Schema = "dbo")]
    public class DimProducto
    {
        public int ProductoKey { get; set; }
        public int IdProducto { get; set; }
        public string? NombreProducto { get; set; }
        public string? Categoria { get; set; }
        public DateOnly? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public bool? RegistroActivo { get; set; }
    }
}