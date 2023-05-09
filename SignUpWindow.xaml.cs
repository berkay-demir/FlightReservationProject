using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FlightReservationProject
{
    /// <summary>
    /// Interaction logic for SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        private SqlConnection conn;
        public SignUpWindow()
        {
            InitializeComponent();
            string connString = ConfigurationManager.ConnectionStrings["FlightReservationProject.Properties.Settings.FlightReservationConnectionString"].ConnectionString;
            conn = new SqlConnection(connString);
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the user's input from the text boxes
            string name = NameTextBox.Text.Trim();
            string surname = SurnameTextBox.Text.Trim();
            string username = UserameTextBox.Text.Trim();
            string password = PasswordTextBox.Password.Trim();

            // Check that all fields have been filled in
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Fill all fields with right information.");
                return;
            }
            
           

            // Insert the new user record into the database
            try
            {

                conn.Open();
                SqlCommand cmdCheck = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @Username", conn);
                cmdCheck.Parameters.AddWithValue("@Username", username);
                int count = Convert.ToInt32(cmdCheck.ExecuteScalar());
                if (count > 0)
                {
                    MessageBox.Show("Username already exists. Choose a different username.");
                    return;
                }
                SqlCommand cmd = new SqlCommand("INSERT INTO Users (Name, Surname, Username, Password, UserType) VALUES (@Name, @Surname, @Username, @Password, 2)", conn);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Surname", surname);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("User created successfully.");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("User cannot created.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
