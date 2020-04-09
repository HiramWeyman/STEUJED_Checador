using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace STEUJED_Checador
{
    public partial class Form2 : Form
    {
        private string connectionString = "data source=65.99.252.110;initial catalog=steujedo_sindicato;persist security info=True;user id=steujedo_sindicato;password=Sindicato#1586;MultipleActiveResultSets=True;";
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string count_users = "SELECT count(*) from Usuarios where matricula=" + textBox1.Text + " and role_id=1";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command_count_user = new SqlCommand(count_users, connection);

                try
                {
                    connection.Open();
                    int count = (int)command_count_user.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show("Ingreso Correctamente");
                        Form3 frm = new Form3();
                        frm.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Usuario o Clabe Incorrectos");
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
