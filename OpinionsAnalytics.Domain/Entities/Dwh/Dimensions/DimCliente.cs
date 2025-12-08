using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpinionsAnalytics.Domain.Entities.Dwh.Dimensions
{
    [Table("DimCliente", Schema = "dbo")]
    public class DimCliente
    {
        public int ClienteKey { get; set; }
        public int IdCliente { get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public DateOnly? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public bool? RegistroActivo { get; set; }
    }
}