namespace OpinionsAnalytics.Domain.Entities.Csv
{
    public class EncuestasInternas
    {
        public int IdOpinion { get; set; }
        public int IdCliente { get; set; }
        public int IdProducto { get; set; }
        public DateTime Fecha { get; set; }
        public string Comentario { get; set; }
        public string Clasificacion { get; set; }
        public int PuntajeSatisfaccion { get; set; }
        public string Fuente { get; set; }

        public EncuestasInternas()
        {

        }

        public EncuestasInternas(int idOpinion, int idCliente, int idProducto, DateTime fecha,
                       string comentario, string clasificacion, int puntajeSatisfaccion, string fuente)
        {
            IdOpinion = idOpinion;
            IdCliente = idCliente;
            IdProducto = idProducto;
            Fecha = fecha;
            Comentario = comentario;
            Clasificacion = clasificacion;
            PuntajeSatisfaccion = puntajeSatisfaccion;
            Fuente = fuente;
        }

        public override string ToString()
        {
            return $"{IdOpinion},{IdCliente},{IdProducto},{Fecha:yyyy-MM-dd},\"{Comentario}\",{Clasificacion},{PuntajeSatisfaccion},{Fuente}";
        }
    }
}
