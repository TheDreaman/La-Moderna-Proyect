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
    public class VentasController : Controller
    {
        public string draw = "";
        public string start = "";
        public string lenght = "";
        public string sortColumn = "";
        public string sortColumnDir = "";
        public string searchValue = "";
        public int pageSize, skip, recordsTotal;

        // GET: Caja
        public ActionResult Ventas()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Json()
        {
            SqlConnection con;
            SqlDataReader lec;
            SqlCommand comQry;
            SqlDataAdapter ada;

            var cadConexion = "Data Source=.; Initial Catalog=doncuco;Integrated Security=SSPI;" + "MultipleActiveResultSets = true;" ;
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
                string ID = "";
                string Prod = "";
                string Mar = "";
                string Fec = "";
                string Can = "";
                string Ven = "";
                string Ent = "";
                string Gan = "";
                string Clie = "";
                List<VentasModel> lst = new List<VentasModel>();
                List<VentasModel> src = new List<VentasModel>();
                DataTable prodName = new DataTable();
                DataTable clieName = new DataTable();

                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                pageSize = length != null ? Convert.ToInt32(length) : 0;
                skip = start != null ? Convert.ToInt32(start) : 0;
                recordsTotal = 0;

                comQry = new SqlCommand("sp_ganancias", con);
                comQry.CommandType = CommandType.StoredProcedure;
                comQry.Parameters.AddWithValue("@operacion", 1);

                ada = new SqlDataAdapter(comQry);
                lec = comQry.ExecuteReader();

                while (lec.Read())
                {
                    ID = lec["ID"].ToString();
                    Prod = lec["Producto"].ToString();
                    SqlDataAdapter datos = new SqlDataAdapter("sp_producto_id", con);
                    datos.SelectCommand.CommandType = CommandType.StoredProcedure;
                    datos.SelectCommand.Parameters.AddWithValue("@id_producto", Convert.ToInt32(Prod));
                    datos.Fill(prodName);
                    object pName = prodName.Rows[0][1];
                    Prod = pName.ToString();
                    Mar = lec["Marca"].ToString();
                    Fec = lec["Fecha"].ToString();
                    Can = lec["Cantidad"].ToString();
                    Ven = lec["PVenta"].ToString();
                    Ent = lec["PEntrada"].ToString();
                    Gan = lec["Ganancia"].ToString();
                    Clie = lec["Cliente"].ToString();
                    if(Clie!="")
                    {
                        SqlDataAdapter clie = new SqlDataAdapter("sp_cliente_id", con);
                        clie.SelectCommand.CommandType = CommandType.StoredProcedure;
                        clie.SelectCommand.Parameters.AddWithValue("@id_cliente", Convert.ToInt32(Clie));
                        clie.Fill(clieName);
                        object pClie = clieName.Rows[0][1];
                        Clie = pClie.ToString();
                    }                    
                    VentasModel venMod = new VentasModel
                    {
                        ID = ID,
                        Producto = Prod,
                        Marca = Mar,
                        Fecha = Fec,
                        Cantidad = Can,
                        PVenta = Ven,
                        PEntrada = Ent,
                        Ganancia = Gan,
                        Cliente = Clie,
                    };
                    lst.Add(venMod);

                    if (searchValue != "")
                    {
                        if (ID.Contains(searchValue) || Prod.Contains(searchValue) || Mar.Contains(searchValue)
                            || Fec.Contains(searchValue) || Can.Contains(searchValue) || Ven.Contains(searchValue)
                            || Ent.Contains(searchValue) || Gan.Contains(searchValue) || Clie.Contains(searchValue))
                        {
                            src.Add(venMod);
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
    }
}