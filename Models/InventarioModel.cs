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
        public string Tipo { get; set; }
        public string Marca { get; set; }
        public string Total { get; set; }

    }
    public class Rellena
    {        
        public string Tipo { get; set; }
        public string Marca { get; set; }

    }
    public class Rellena2
    {
        public string NameEmpresa { get; set; }
        public string NameProveedor { get; set; }
        public string ApeProveedor { get; set; }
        public string TelProveedor { get; set; }

    }
    public class Caducos
    {
        public string IdEntrada { get; set; }
        public string IdProducto { get; set; }
        public string Caduca { get; set; }
        public string Cantidad { get; set; }

    }
    public class Autofill
    {
        public string NombreEmpresa { get; set; }
    }
    public class SrcTipoProd
    {
        public string TipoProducto { get; set; }
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

    public class Cache
    {
        public static string inProdTipoCon { get; set; }
        public static string inProdCon { get; set; }
        public static string modIdProd { get; set; }
        public static string modTipo { get; set; }
        public string modTipo2 { get; set; } 
        public static string modMarca { get; set; }
        public static string modCantEntry { get; set; }
        public static string modDateIn { get; set; }
        public static string modCad { get; set; }
        public static string modIdProveedor { get; set; }
        public static string modNameEmpresa { get; set; }
        public static string modNameProveedor { get; set; }
        public static string modApeProveedor { get; set; }
        public static string modTelProveedor { get; set; }
        public static string modPrecioIn { get; set; }
    }
}