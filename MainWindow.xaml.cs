using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Configuration;
using System.CodeDom.Compiler;
using System.Security.Cryptography;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace FlightReservationProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SqlConnection conn;
        public MainWindow()
        {
            InitializeComponent();
            string connString = ConfigurationManager.ConnectionStrings["FlightReservationProject.Properties.Settings.FlightReservationConnectionString"].ConnectionString;
            conn = new SqlConnection(connString);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT userId, UserType FROM Users WHERE Username=@username AND Password=@password", conn);
            cmd.Parameters.AddWithValue("@username", UsernameTextBox.Text);
            cmd.Parameters.AddWithValue("@password", PasswordBox.Password);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int userID = (int)reader["UserID"];
                int userType = (int)reader["UserType"];

                if (userType == 1)
                {
                    AddFlightWindow window1 = new AddFlightWindow();
                    window1.Show();
                    conn.Close();
                    this.Close();
                }
                else if (userType == 2)
                {
                    ChoseFlightWindow window2 = new ChoseFlightWindow(userID);
                    window2.Show();
                    conn.Close();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Invalid username or password.");
                conn.Close();
            }
            
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            SignUpWindow window3 = new SignUpWindow();
            window3.Show();
        }
    }
}
