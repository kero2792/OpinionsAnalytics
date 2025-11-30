using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpinionsAnalytics.Domain.Entities.Dwh.Dimensions
{
    public class DimFecha
    {
        public int FechaKey { get; set; }

        public DateOnly Fecha { get; set; }

        public short Anio { get; set; }

        public byte Trimestre { get; set; }

        public byte Mes { get; set; }

        public string NombreMes { get; set; }

        public byte Semana { get; set; }

        public byte DiaMes { get; set; }

        public byte DiaSemana { get; set; }

        public string NombreDiaSemana { get; set; }

        public bool? EsFinDeSemana { get; set; }

        public bool? EsFeriado { get; set; }
    }
}
