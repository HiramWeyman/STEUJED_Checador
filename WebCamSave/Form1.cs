using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;
using STEUJED_Checador;

namespace WebCamSave
{
    public partial class Form1 : Form
    {
        private string Path = @"C:\WebCamSave\";
        private bool HayDispositivos;
        private FilterInfoCollection MisDispositivios;
        private VideoCaptureDevice MiWebCam;
        private string connectionString = "data source=65.99.252.110;initial catalog=steujedo_sindicato;persist security info=True;user id=steujedo_sindicato;password=Sindicato#1586;MultipleActiveResultSets=True;";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CargaDispositivos();
            if (!(System.IO.Directory.Exists(Path)))
            {
                System.IO.Directory.CreateDirectory(Path);
            }


        }

        public void CargaDispositivos()
        {
            MisDispositivios = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (MisDispositivios.Count > 0)
            {
                HayDispositivos = true;
                for (int i = 0; i < MisDispositivios.Count; i++)
                    comboBox1.Items.Add(MisDispositivios[i].Name.ToString());
                comboBox1.Text = MisDispositivios[0].Name.ToString();
            }
            else
                HayDispositivos = false;
            
        }

        private void CerrarWebCam()
        {
            if (MiWebCam!=null && MiWebCam.IsRunning)
            {
                MiWebCam.SignalToStop();
                MiWebCam = null;
            }
        }

        public class AutoClosingMessageBox
        {
            System.Threading.Timer _timeoutTimer;
            string _caption;
            AutoClosingMessageBox(string text, string caption, int timeout)
            {
                _caption = caption;
                _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
                    null, timeout, System.Threading.Timeout.Infinite);
                MessageBox.Show(text, caption);
            }
            public static void Show(string text, string caption, int timeout)
            {
                new AutoClosingMessageBox(text, caption, timeout);
            }
            void OnTimerElapsed(object state)
            {
                IntPtr mbWnd = FindWindow(null, _caption);
                if (mbWnd != IntPtr.Zero)
                    SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                _timeoutTimer.Dispose();
            }
            const int WM_CLOSE = 0x0010;
            [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
            static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
            [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
            static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CerrarWebCam();
            int i = comboBox1.SelectedIndex;
            string NombreVideo = MisDispositivios[i].MonikerString;
            MiWebCam = new VideoCaptureDevice(NombreVideo);
            MiWebCam.NewFrame += new NewFrameEventHandler(Capturando);
            MiWebCam.Start();
        }

        private void Capturando(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap Imagen = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = Imagen;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            CerrarWebCam();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (MiWebCam != null && MiWebCam.IsRunning)
            {
                if (textBox1.TextLength.Equals(6))
                {
                    try
                    {
                        //SqlConnection conn = new SqlConnection(@"data source=65.99.252.110;initial catalog=steujedo_sindicato;persist security info=True;user id=steujedo_sindicato;password=Sindicato#1586;MultipleActiveResultSets=True;");
                        string comp_user = "SELECT count(*) from Lista_Asistencia,Usuarios where usuario_id=Usuarios.id  and matricula=" + textBox1.Text + " and fecha='" + DateTime.Now.ToString("yyyy-MM-dd") + "'";
                        string count_users = "SELECT count(*) from Usuarios where matricula=" + textBox1.Text;
                        string get_id_user = "SELECT id from Usuarios where matricula=" + textBox1.Text;
                        string Insert = "INSERT INTO Lista_Asistencia(usuario_id, fecha,ruta_img,entrada) VALUES(@usuario_id, @fecha,@ruta_img,@entrada)";
                        
                        string entrada = "";
                        string ruta_img = "";
                        Random rdn = new Random();
                        int a = rdn.Next(1000, 9000);
                        ruta_img = Path + a;
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            SqlCommand command_comp_asist = new SqlCommand(comp_user, connection);
                            SqlCommand command_count_user = new SqlCommand(count_users, connection);
                            SqlCommand command_id = new SqlCommand(get_id_user, connection);

                            try
                            {
                                connection.Open();
                                int count = (int)command_comp_asist.ExecuteScalar();
                                if (count > 0)
                                {
                                    AutoClosingMessageBox.Show("El trabajador ya se registro el dia de hoy", "Caption", 2000);
                                }
                                else
                                {

                                    int count_user = (int)command_count_user.ExecuteScalar();
                                    Console.WriteLine("Count is: {0}", count);
                                    if (count_user > 0)
                                    {
                                        SqlDataReader reader = command_id.ExecuteReader();
                                        try
                                        {
                                            while (reader.Read())
                                            {
                                                DateTime localDate = DateTime.Now;
                                                String[] cultureNames = { "es-MX" };

                                                foreach (var cultureName in cultureNames)
                                                {
                                                    var culture = new CultureInfo(cultureName);
                                                    Console.WriteLine("{0}: {1}", cultureName, localDate.ToString(culture));
                                                    entrada = localDate.ToString(culture);
                                                }

                                                Console.WriteLine(String.Format("{0}", reader["id"]));// etc
                                                string id = String.Format("{0}", reader["id"]);
                                                SqlCommand cmd = new SqlCommand(Insert, connection);
                                                cmd.Parameters.AddWithValue("@usuario_id", id);
                                                cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
                                                cmd.Parameters.AddWithValue("@ruta_img", "file:"+ruta_img + ".jpg");
                                                cmd.Parameters.AddWithValue("@entrada", entrada);

                                                //cmd.Parameters.AddWithValue("@entrada", DateTime.Now.AddMinutes(55).ToString("dd'/'MM'/'yyyy HH:mm:ss"));
                                                int res = cmd.ExecuteNonQuery();
                                                if (res > 0)
                                                {

                                                    AutoClosingMessageBox.Show("Se registro Correctamente", "Caption", 2000);
                                                    if (MiWebCam != null && MiWebCam.IsRunning)
                                                    {
                                                        
                                                        pictureBox2.Image = pictureBox1.Image;
                                                        pictureBox2.Image.Save(ruta_img+".jpg", ImageFormat.Jpeg);
                                                        timer1.Enabled = true;
                                                    }
                                                   
                                                }


                                            }
                                        }
                                        finally
                                        {
                                            // Always call Close when done reading.
                                            reader.Close();
                                        }

                                    }
                                    else
                                    {
                                        MessageBox.Show("Su matricula es incorrecta");
                                    }
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
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());

                    }
                }
            }
            else {

                MessageBox.Show("Por favor encienda la camara");
            }
               
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox1.Text = null;
            pictureBox2.Image = null;
            timer1.Enabled = false;
        }

        private void reporteDeAsistenciaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.Show();
        }
    }
}
