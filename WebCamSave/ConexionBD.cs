using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace WebCamSave
{
    class ConexionBD
    {
        string cadena = "data source=localhost;initial catalog=steujedo_sindicato;persist security info=True;user id="";password="";MultipleActiveResultSets=True;";
        public SqlConnection conectarBD= new SqlConnection();

        public ConexionBD() 
        {
            conectarBD.ConnectionString = cadena;
        }

        public void abrir() {

            try {

                conectarBD.Open();
                Console.WriteLine("Conexion Abrierta");
            
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Error al abrir la conexion " + ex.Message.ToString());
            }
        
        }
        public void cerrar() {

            try {

                conectarBD.Close();
                Console.WriteLine("Concion Cerrada");
            } 
            catch (Exception ex) 
            {
                Console.WriteLine("Error al cerrar la conexion "+ex.Message.ToString());
            }
        
        }
    }
}
