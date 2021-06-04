using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace La_Moderna_Proyect.Models
{
    public class VentasModel
    {
        public string ID { get; set; }
        public string Producto { get; set; }
        public string Marca { get; set; }
        public string Fecha { get; set; }
        public string Cantidad { get; set; }
        public string PVenta { get; set; }
        public string PEntrada { get; set; }
        public string Ganancia { get; set; }
        public string Cliente { get; set; }
    }
}