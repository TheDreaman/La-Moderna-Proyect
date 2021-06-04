using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;


namespace La_Moderna_Proyect.Models
{
    public class InventarioModel
    {
        public string Producto { get; set; }
        public string Marca { get; set; }
        public string Total { get; set; }

    }
    public class InventarioCon
    {
        public string ID { get; set; }
        public string Producto { get; set; }
        public string Marca { get; set; }
        public string Entradas { get; set; }
        public string Ventas { get; set; }
        public string Salida_Caducidad { get; set; }
        public string Existencias { get; set; }

    }
}