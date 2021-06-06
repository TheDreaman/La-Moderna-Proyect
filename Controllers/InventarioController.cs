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
            return View("Inventario");
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
                    Mar = lec["Marca"].ToString();
                    Tot = lec["Total"].ToString();
                    InventarioModel invMod = new InventarioModel
                    {
                        Producto = Prod,
                        Marca = Mar,
                        Total = Tot,                        
                    };
                    lst.Add(invMod);
                    
                    if (searchValue != "")
                    {
                        if (Prod.Contains(searchValue) || Mar.Contains(searchValue) || Tot.Contains(searchValue))
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
        public ActionResult Consultas(/*string inProdTipoCon, string inProdCon*/)
        {
            //Cache cache = new Models.Cache();
            //Models.Cache cache = new Cache();
            string inTipo = Cache.inProdTipoCon;
            string inCon = Cache.inProdCon;
            /*if (inProdCon == "")
            {
                return View("Inventario");
            }*/
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
            catch //(Exception ex)
            {
                return Content("Inventario");
                //return Content("F" + ex.ToString());
            }

        }
    }
}
