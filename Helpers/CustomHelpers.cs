using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using La_Moderna_Proyect.Models;
using System.Data.SqlClient;
using System.Data;

namespace La_Moderna_Proyect.Helpers
{
    public static class CustomHelpers
    {
        public static MvcHtmlString Alertas1(this HtmlHelper Helper)
        {
            string Text2 = "";
            string html = "";
            SqlConnection con;
            SqlDataReader lec;
            SqlCommand comQry;
            SqlDataAdapter ada;
            
            List<string> lst = new List<string>();

            var cadConexion = "Data Source=.; Initial Catalog=doncuco;Integrated Security=SSPI;" + "MultipleActiveResultSets = true;";
            con = new SqlConnection(cadConexion);
            try
            {
                con.Open();
            }
            catch (Exception ex)
            {
                return new MvcHtmlString(html + ex);
            }

            try
            {
                comQry = new SqlCommand("sp_alertas", con); //------------------------Checa si hay alertas de caducidad
                comQry.CommandType = CommandType.StoredProcedure;
                comQry.Parameters.AddWithValue("@operacion", 1);
                ada = new SqlDataAdapter(comQry);
                lec = comQry.ExecuteReader();

                while (lec.Read())
                {                    
                    Text2 = ", Producto: " + lec["Producto"].ToString() + 
                            ", Marca: " + lec["Marca"].ToString() + 
                            ", Cantidad: " + lec["Cantidad"].ToString() + 
                            ", Caducidad: " + lec["Caducidad"].ToString();                    
                    html = "<div class='" + "alert alert-danger alert-dismissible fade show" + "' role='"
                        + "alert" + "'><strong>" + "Por caducar!" + "</strong>" + Text2 + "</div>";

                    lst.Add(html);
                }
                Cache.alerta1 = lst.Count();
                string htmlfinal = string.Join("", lst);

                lec.Close();
                con.Close();
                if (htmlfinal != null && htmlfinal != "")
                {
                    return new MvcHtmlString(htmlfinal);
                }
                else
                {
                    return new MvcHtmlString("");
                }
                
            }
            catch (Exception ex)
            {
                return new MvcHtmlString(html);
            }
        }
        public static MvcHtmlString Alertas2(this HtmlHelper Helper)
        {
            string Text2 = "";
            string html = "";
            SqlConnection con;
            SqlDataReader lec;
            SqlCommand comQry;
            SqlDataAdapter ada;
            List<string> lst = new List<string>();

            var cadConexion = "Data Source=.; Initial Catalog=doncuco;Integrated Security=SSPI;" + "MultipleActiveResultSets = true;";
            con = new SqlConnection(cadConexion);
            try
            {
                con.Open();
            }
            catch (Exception ex)
            {
                return new MvcHtmlString(html + ex);
            }

            try
            {
                comQry = new SqlCommand("sp_alertas", con); //------------------------Checa si hay alertas de inventario <5
                comQry.CommandType = CommandType.StoredProcedure;
                comQry.Parameters.AddWithValue("@operacion", 2);
                ada = new SqlDataAdapter(comQry);
                lec = comQry.ExecuteReader();

                while (lec.Read())
                {
                    Text2 = ", Producto: " + lec["Producto"].ToString() +
                            ", Marca: " + lec["Marca"].ToString() +
                            ", Total: " + lec["Total"].ToString();
                    html = "<div class='" + "alert alert-danger alert-dismissible fade show" + "' role='"
                        + "alert" + "'><strong>" + "Pocas unidades!" + "</strong>" + Text2 + "</div>";

                    lst.Add(html);
                }
                Cache.alerta2 = lst.Count();
                string htmlfinal = string.Join("", lst);

                lec.Close();
                con.Close();
                if (htmlfinal != null && htmlfinal != "")
                {
                    return new MvcHtmlString(htmlfinal);
                }
                else
                {
                    return new MvcHtmlString("");
                }
            }
            catch (Exception ex)
            {
                return new MvcHtmlString(html);
            }
        }
        public static MvcHtmlString Alertas3(this HtmlHelper Helper)
        {
            int res = Cache.alerta1 + Cache.alerta2;
            return new MvcHtmlString(res.ToString());
        }
    }
}