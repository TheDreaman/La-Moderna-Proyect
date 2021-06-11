using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using La_Moderna_Proyect.Models;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

namespace La_Moderna_Proyect.Controllers
{
    public class InventarioController : Controller
    {
        
        public string draw = "";
        public string start = "";
        public string lenght = "";
        public string sortColumn = "";
        public string sortColumnDir = "";
        public string searchValue = "";
        public int pageSize, skip, recordsTotal;

        // GET: Inventario
        public ActionResult Inventario()
        {
            SqlConnection con;
            SqlDataReader lec, lec2;
            SqlCommand comQry;
            SqlDataAdapter ada;

            var cadConexion = "Data Source=.; Initial Catalog=doncuco;Integrated Security=SSPI;" + "MultipleActiveResultSets = true;";
            con = new SqlConnection(cadConexion);
            try
            {
                con.Open();
            }
            catch (Exception ex)
            {
                return Content("F" + ex.ToString());
            }

            try
            {
                string IdEnt = "";
                string IdProd = "";
                string Caduca = "";
                string Cant = "";

                comQry = new SqlCommand("sp_productos_caducados", con); //------------------------Checa y quita los productos caducos
                comQry.CommandType = CommandType.StoredProcedure;
                ada = new SqlDataAdapter(comQry);
                lec = comQry.ExecuteReader();

                while (lec.Read())
                {
                    IdEnt = lec["IdEntrada"].ToString();
                    IdProd = lec["IdProducto"].ToString();
                    Caduca = lec["Caduca"].ToString();
                    Cant = lec["Cantidad"].ToString();

                    if (IdEnt != "" && IdEnt != null)
                    {
                        comQry = new SqlCommand("sp_actualiza_caducidad", con);
                        comQry.CommandType = CommandType.StoredProcedure;
                        comQry.Parameters.AddWithValue("@id_caducidad", 1);
                        comQry.Parameters.AddWithValue("@producto_caducidad", IdProd);
                        comQry.Parameters.AddWithValue("@fecha_caducidad", Convert.ToDateTime(Caduca));
                        comQry.Parameters.AddWithValue("@cantidad_caducidad", Cant);
                        comQry.Parameters.AddWithValue("@operacion", 1);
                        ada = new SqlDataAdapter(comQry);
                        lec2 = comQry.ExecuteReader();
                        lec2.Close();
                    }
                }
                lec.Close();
                con.Close();
                return View("Inventario");
            }
            catch (Exception ex)
            {
                return Content("F" + ex.ToString());
            }
        }
        public ActionResult VerConsultas()
        {
            return View("VerConsultas");
        }
        [HttpPost]
        public ActionResult Test(string inProdTipoCon, string inProdCon)
        {
            if(inProdCon != null)
            {                
                Cache.inProdCon = inProdCon;
                Cache.inProdTipoCon = inProdTipoCon;
                return Content("1");
            }
            SqlConnection con;
            SqlDataReader lec;
            SqlCommand comQry;
            SqlDataAdapter ada;

            var cadConexion = "Data Source=.; Initial Catalog=doncuco;Integrated Security=SSPI;";
            con = new SqlConnection(cadConexion);
            try
            {
                con.Open();
            }
            catch (Exception ex)
            {
                return Content("F" + ex.ToString());
            }

            try
            {
                string Prod = "";
                string Tip = "";
                string Mar = "";
                string Tot = "";
                List<InventarioModel> lst = new List<InventarioModel>();
                List<InventarioModel> src = new List<InventarioModel>();

                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                pageSize = length != null ? Convert.ToInt32(length) : 0;
                skip = start != null ? Convert.ToInt32(start) : 0;
                recordsTotal = 0;

                comQry = new SqlCommand("sp_reportes", con);
                comQry.CommandType = CommandType.StoredProcedure;
                comQry.Parameters.AddWithValue("@operacion", 5);

                ada = new SqlDataAdapter(comQry);
                lec = comQry.ExecuteReader();

                while (lec.Read())
                {
                    Prod = lec["Producto"].ToString();
                    Tip = lec["Tipo"].ToString();
                    Mar = lec["Marca"].ToString();
                    Tot = lec["Total"].ToString();
                    InventarioModel invMod = new InventarioModel
                    {
                        Producto = Prod,
                        Tipo = Tip,
                        Marca = Mar,
                        Total = Tot,                        
                    };
                    lst.Add(invMod);
                    
                    if (searchValue != "")
                    {
                        if (Prod.Contains(searchValue) || Tip.Contains(searchValue) || Mar.Contains(searchValue) || Tot.Contains(searchValue))
                        {
                            src.Add(invMod);
                        }
                    }
                }
                if (searchValue != "")
                {
                    recordsTotal = src.Count();
                    src = src.Skip(skip).Take(pageSize).ToList();
                    lec.Close();
                    con.Close();
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = src });
                }
                recordsTotal = lst.Count();
                lst = lst.Skip(skip).Take(pageSize).ToList();
                lec.Close();
                con.Close();
                
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = lst });
            }
            catch (Exception ex)
            {
                return Content("F" + ex.ToString());
            }
            
        }
        [HttpPost]
        public ActionResult Consultas()
        {
            string inTipo = Cache.inProdTipoCon;
            string inCon = Cache.inProdCon;
            SqlConnection con;
            SqlDataReader lec;
            SqlCommand comQry;
            SqlDataAdapter ada;

            var cadConexion = "Data Source=.; Initial Catalog=doncuco;Integrated Security=SSPI;";
            con = new SqlConnection(cadConexion);
            try
            {
                con.Open();
            }
            catch (Exception ex)
            {
                return Content("F" + ex.ToString());
            }

            try
            {
                string Id = "";
                string Prod = "";
                string Mar = "";
                string Ent = "";
                string Ven = "";
                string SalCad = "";
                string Exi = "";
                List<InventarioCon> lst = new List<InventarioCon>();    

                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                pageSize = length != null ? Convert.ToInt32(length) : 0;
                skip = start != null ? Convert.ToInt32(start) : 0;
                recordsTotal = 0;

                if(inTipo == "ID")
                {
                    comQry = new SqlCommand("sp_producto_id", con);
                    comQry.CommandType = CommandType.StoredProcedure;
                    comQry.Parameters.AddWithValue("@id_producto", inCon);
                }
                else if(inTipo == "Marca")
                {
                    comQry = new SqlCommand("sp_producto_marca", con);
                    comQry.CommandType = CommandType.StoredProcedure;
                    comQry.Parameters.AddWithValue("@marca_producto", inCon);
                }
                else if (inTipo == "Tipo")
                {
                    comQry = new SqlCommand("sp_producto_tipo", con);
                    comQry.CommandType = CommandType.StoredProcedure;
                    comQry.Parameters.AddWithValue("@tipo_producto", inCon);
                }
                else
                {
                    return View("Inventario");
                }
                
                ada = new SqlDataAdapter(comQry);
                lec = comQry.ExecuteReader();

                while (lec.Read())
                {
                    Id = lec["ID"].ToString();
                    Prod = lec["Producto"].ToString();
                    Mar = lec["Marca"].ToString();
                    Ent = lec["Entradas"].ToString();
                    Ven = lec["Ventas"].ToString();
                    if (Ven == null)
                    {
                        Ven = " ";
                    }
                    SalCad = lec["Salida_Caducidad"].ToString();
                    if(SalCad==null)
                    {
                        SalCad = " ";
                    }
                    Exi = lec["Existencias"].ToString();
                    InventarioCon invCon = new InventarioCon
                    {
                        ID = Id,
                        Producto = Prod,
                        Marca = Mar,
                        Entradas = Ent,
                        Ventas = Ven,
                        Salida_Caducidad = SalCad,
                        Existencias = Exi,
                    };
                    lst.Add(invCon);
                }                
                recordsTotal = lst.Count();
                lst = lst.Skip(skip).Take(pageSize).ToList();
                lec.Close();
                con.Close();
                if (lst.Count() != 0)
                {
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = lst });
                }
                else
                {
                    return View("Inventario");
                }
            }
            catch 
            {
                return Content("Inventario");
            }

        }
        [HttpPost]
        public ActionResult VerificaEntrada(string modIdProd, string modTipo, string modMarca, string modCantEntry, string modDateIn, string modCad, string modIdProveedor, string modNameEmpresa,
                                            string modNameProveedor, string modApeProveedor, string modTelProveedor, string modPrecioIn)
        {            
            SqlConnection con;
            SqlDataReader lec;
            SqlCommand comQry;
            SqlDataAdapter ada;            

            var cadConexion = "Data Source=.; Initial Catalog=doncuco;Integrated Security=SSPI;";
            con = new SqlConnection(cadConexion);
            try
            {
                con.Open();
            }
            catch (Exception ex)
            {
                return Content("F" + ex.ToString());
            }

            try
            {
                if (modIdProd != null && modIdProd != "")
                {
                    comQry = new SqlCommand("sp_producto_id", con);
                    comQry.CommandType = CommandType.StoredProcedure;
                    comQry.Parameters.AddWithValue("@id_producto", modIdProd);
                    ada = new SqlDataAdapter(comQry);
                    lec = comQry.ExecuteReader();
                    lec.Read();
                    var rellena = new Rellena
                    {
                        Tipo = lec["Producto"].ToString(),
                        Marca = lec["Marca"].ToString(),
                    };                                        
                    lec.Close();                    
                    return Json(rellena);
                }
                else if((modIdProd == "" || modIdProd == null) && (modTipo != "" && modTipo != null || modMarca != "" && modMarca != null))
                {
                    return Content("1");
                }
                else
                {
                    return Content("0");
                }
            }
            catch
            {
                return Content("Inventario");
            }
        }
        [HttpPost]
        public ActionResult VerificaEntrada2(string modIdProd, string modTipo, string modMarca, string modCantEntry, string modDateIn, string modCad, string modIdProveedor, string modNameEmpresa,
                                            string modNameProveedor, string modApeProveedor, string modTelProveedor, string modPrecioIn)
        {
            SqlConnection con;
            SqlDataReader lec;
            SqlCommand comQry;
            SqlDataAdapter ada;

            var cadConexion = "Data Source=.; Initial Catalog=doncuco;Integrated Security=SSPI;";
            con = new SqlConnection(cadConexion);
            try
            {
                con.Open();
            }
            catch (Exception ex)
            {
                return Content("F" + ex.ToString());
            }

            try
            {
                if (modIdProveedor != null && modIdProveedor != "")
                {
                    comQry = new SqlCommand("sp_busca_proveedor_ID", con);
                    comQry.CommandType = CommandType.StoredProcedure;
                    comQry.Parameters.AddWithValue("@id_proveedor", modIdProveedor);
                    ada = new SqlDataAdapter(comQry);
                    lec = comQry.ExecuteReader();
                    lec.Read();
                    var rellena2 = new Rellena2
                    {
                        NameEmpresa = lec["NameEmpresa"].ToString(),
                        NameProveedor = lec["NameProveedor"].ToString(),
                        ApeProveedor = lec["ApeProveedor"].ToString(),
                        TelProveedor = lec["TelProveedor"].ToString()
                    };
                    lec.Close();
                    return Json(rellena2);
                }
                else if ((modIdProveedor == "" || modIdProveedor == null) && (modNameEmpresa != null && modNameEmpresa != "" ||
                    modNameProveedor != null && modNameProveedor != "" || modApeProveedor != null && modApeProveedor!= "" ||
                    modTelProveedor != null && modTelProveedor != ""))
                {
                    return Content("1");
                }
                else
                {
                    return Content("0");
                }
            }
            catch
            {
                return Content("Inventario");
            }
        }
        [HttpPost]
        public ActionResult InsertaEntrada(string modIdProd, string modTipo, string modMarca, string modCantEntry, string modDateIn, string modCad, string modIdProveedor, string modNameEmpresa,
                                            string modNameProveedor, string modApeProveedor, string modTelProveedor, string modPrecioIn)
        {
            SqlConnection con;
            SqlDataReader lec;
            SqlCommand comQry;
            SqlDataAdapter ada;
            List<Autofill> lst = new List<Autofill>();
            List<SrcTipoProd> lst2 = new List<SrcTipoProd>();

            var cadConexion = "Data Source=.; Initial Catalog=doncuco;Integrated Security=SSPI;";
            con = new SqlConnection(cadConexion);
            try
            {
                con.Open();
            }
            catch (Exception ex)
            {
                return Content("F" + ex.ToString());
            }

            try
            {
                if ((modIdProd != "" && modIdProd != null) && (modIdProveedor != "" && modIdProveedor != null)) //ID producto existe, ID Proveedor existe
                {
                    comQry = new SqlCommand("sp_actualiza_entradas_existentes", con);
                    comQry.CommandType = CommandType.StoredProcedure;
                    comQry.Parameters.AddWithValue("@id_producto", modIdProd);
                    comQry.Parameters.AddWithValue("@cantidad_entrada", modCantEntry);
                    comQry.Parameters.AddWithValue("@id_proveedor", modIdProveedor);
                    comQry.Parameters.AddWithValue("@precio_entrada", modPrecioIn);
                    comQry.Parameters.AddWithValue("@caducidad_entrada", modCad);
                    ada = new SqlDataAdapter(comQry);
                    lec = comQry.ExecuteReader();
                    lec.Close();
                    con.Close();
                }
                else if ((modIdProd != "" && modIdProd != null) && (modIdProveedor == "" || modIdProveedor == null)) //ID producto existe, ID Proveedor no existe
                {
                    
                    comQry = new SqlCommand("sp_muestra_empresas", con);
                    comQry.CommandType = CommandType.StoredProcedure;

                    ada = new SqlDataAdapter(comQry);
                    lec = comQry.ExecuteReader();

                    while (lec.Read())
                    {
                        Autofill autoName = new Autofill
                        {
                            NombreEmpresa = lec["NombreEmpresa"].ToString()
                        };
                        lst.Add(autoName);
                    }
                    lec.Close();
                    var src = lst.Where(x => x.NombreEmpresa.Contains(modNameEmpresa)).Select(y => y.NombreEmpresa).ToList();
                    if (src.Count != 0) //ID producto existe, ID Proveedor no existe, Nombre de empresa existe
                    {
                        comQry = new SqlCommand("sp_actualiza_entradas_por_id_producto_datos_proveedor", con);
                    }
                    else //ID producto existe, ID Proveedor no existe, Nombre de empresa no existe
                    {
                        comQry = new SqlCommand("sp_actualiza_entradas_por_id_producto", con);                        
                    }
                    comQry.CommandType = CommandType.StoredProcedure;
                    comQry.Parameters.AddWithValue("@id_producto", modIdProd);
                    comQry.Parameters.AddWithValue("@cantidad_entrada", modCantEntry);
                    comQry.Parameters.AddWithValue("@nombre_empresa", modNameEmpresa);
                    comQry.Parameters.AddWithValue("@nombre_proveedor", modNameProveedor);
                    comQry.Parameters.AddWithValue("@apellido_proveedor", modApeProveedor);
                    comQry.Parameters.AddWithValue("@telefono_proveeedor", modTelProveedor);
                    comQry.Parameters.AddWithValue("@precio_entrada", modPrecioIn);
                    comQry.Parameters.AddWithValue("@caducidad_entrada", modCad);
                    ada = new SqlDataAdapter(comQry);
                    lec = comQry.ExecuteReader();
                    lec.Close();
                    con.Close();
                }
                else if ((modIdProd == "" || modIdProd == null) && (modIdProveedor != "" && modIdProveedor != null)) //ID producto no existe, ID Proveedor existe
                {
                    comQry = new SqlCommand("sp_muestra_tipo_productos", con);
                    comQry.CommandType = CommandType.StoredProcedure;

                    ada = new SqlDataAdapter(comQry);
                    lec = comQry.ExecuteReader();

                    while (lec.Read())
                    {
                        SrcTipoProd autoTipProd = new SrcTipoProd
                        {
                            TipoProducto = lec["TipoProducto"].ToString()
                        };
                        lst2.Add(autoTipProd);
                    }
                    lec.Close();
                    var src2 = lst2.Where(x => x.TipoProducto.Contains(modTipo)).Select(y => y.TipoProducto).ToList();
                    if (src2.Count != 0) //ID producto no existe, ID Proveedor existe, Tipo de producto existe
                    {
                        comQry = new SqlCommand("sp_actualiza_entradas_por_id_proveedor_nueva_marca", con);
                    }
                    else //ID producto no existe, ID Proveedor existe, Tipo de producto no existe
                    {
                        comQry = new SqlCommand("sp_actualiza_entradas_por_id_proveedor", con);
                    }
                    comQry.CommandType = CommandType.StoredProcedure;
                    comQry.Parameters.AddWithValue("@tipo_producto", modTipo);
                    comQry.Parameters.AddWithValue("@marca", modMarca);
                    comQry.Parameters.AddWithValue("@cantidad_entrada", modCantEntry);
                    comQry.Parameters.AddWithValue("@id_proveedor", modIdProveedor);
                    comQry.Parameters.AddWithValue("@precio_entrada", modPrecioIn);
                    comQry.Parameters.AddWithValue("@caducidad_entrada", modCad);
                    ada = new SqlDataAdapter(comQry);
                    lec = comQry.ExecuteReader();
                    lec.Close();
                    con.Close();
                     
                }
                else if ((modIdProd == "" || modIdProd == null) && (modIdProveedor == "" || modIdProveedor == null))
                {
                    comQry = new SqlCommand("sp_actualiza_entradas_no_existen", con);
                    comQry.CommandType = CommandType.StoredProcedure;
                    comQry.Parameters.AddWithValue("@tipo_producto", modTipo);
                    comQry.Parameters.AddWithValue("@marca", modMarca);
                    comQry.Parameters.AddWithValue("@cantidad_entrada", modCantEntry);
                    comQry.Parameters.AddWithValue("@nombre_empresa", modNameEmpresa);
                    comQry.Parameters.AddWithValue("@nombre_proveedor", modNameProveedor);
                    comQry.Parameters.AddWithValue("@apellido_proveedor", modApeProveedor);
                    comQry.Parameters.AddWithValue("@telefono_proveeedor", modTelProveedor);
                    comQry.Parameters.AddWithValue("@precio_entrada", modPrecioIn);
                    comQry.Parameters.AddWithValue("@caducidad_entrada", modCad);
                    ada = new SqlDataAdapter(comQry);
                    lec = comQry.ExecuteReader();
                    lec.Close();
                    con.Close();
                }
                else
                {
                    return Content("0");
                }
                return Content("1");
            }
            catch (Exception ex)
            {
                return Content("F" + ex.ToString());
            }
        }

        public JsonResult AutofillNameEmp(string modIdProd, string modTipo, string modMarca, string modCantEntry, string modDateIn, string modCad, string modIdProveedor, string modNameEmpresa,
                                            string modNameProveedor, string modApeProveedor, string modTelProveedor, string modPrecioIn, string term)
        {
            SqlConnection con;
            SqlDataReader lec;
            SqlCommand comQry;
            SqlDataAdapter ada;

            var cadConexion = "Data Source=.; Initial Catalog=doncuco;Integrated Security=SSPI;";
            con = new SqlConnection(cadConexion);
            try
            {
                con.Open();
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }

            try
            {
                List<Autofill> lst = new List<Autofill>();
                comQry = new SqlCommand("sp_muestra_empresas", con);
                comQry.CommandType = CommandType.StoredProcedure;

                ada = new SqlDataAdapter(comQry);
                lec = comQry.ExecuteReader();

                while (lec.Read())
                {
                    Autofill autoName = new Autofill
                    {
                        NombreEmpresa = lec["NombreEmpresa"].ToString()
                    };
                    lst.Add(autoName);
                }
                lec.Close();
                con.Close();
                var fill = lst.Where(x => x.NombreEmpresa.Contains(term)).Select(y => y.NombreEmpresa)/*.Take(5)*/.ToList();

                return Json(fill, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult AutofillTipoProd(string modIdProd, string modTipo, string modMarca, string modCantEntry, string modDateIn, string modCad, string modIdProveedor, string modNameEmpresa,
                                            string modNameProveedor, string modApeProveedor, string modTelProveedor, string modPrecioIn, string term)
        {
            SqlConnection con;
            SqlDataReader lec;
            SqlCommand comQry;
            SqlDataAdapter ada;

            var cadConexion = "Data Source=.; Initial Catalog=doncuco;Integrated Security=SSPI;";
            con = new SqlConnection(cadConexion);
            try
            {
                con.Open();
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }

            try
            {
                List<SrcTipoProd> lst2 = new List<SrcTipoProd>();

                comQry = new SqlCommand("sp_muestra_tipo_productos", con);
                comQry.CommandType = CommandType.StoredProcedure;

                ada = new SqlDataAdapter(comQry);
                lec = comQry.ExecuteReader();

                while (lec.Read())
                {
                    SrcTipoProd autoTipProd = new SrcTipoProd
                    {
                        TipoProducto = lec["TipoProducto"].ToString()
                    };
                    lst2.Add(autoTipProd);
                }
                lec.Close();
                var src2 = lst2.Where(x => x.TipoProducto.Contains(term)).Select(y => y.TipoProducto).ToList();
                return Json(src2, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
