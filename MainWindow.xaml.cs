using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Configuration;

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

            SqlCommand cmd = new SqlCommand("SELECT UserType FROM Users WHERE Username=@username AND Password=@password", conn);
            cmd.Parameters.AddWithValue("@username", UsernameTextBox.Text);
            cmd.Parameters.AddWithValue("@password", PasswordBox.Password);

            int userType = (int?)cmd.ExecuteScalar() ?? 0;

            if (userType == 1)
            {
                AddFlightWindow window1 = new AddFlightWindow();
                window1.Show();
                conn.Close();
                this.Close();
            }
            else if (userType == 2)
            {
                ChoseFlightWindow window2 = new ChoseFlightWindow();
                window2.Show();
                conn.Close();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.");
                conn.Close();
            }
        }
    }
}
