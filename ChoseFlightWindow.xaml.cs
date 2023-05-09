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
    /// Interaction logic for ChoseFlightWindow.xaml
    /// </summary>
    public partial class ChoseFlightWindow : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["FlightReservationProject.Properties.Settings.FlightReservationConnectionString"].ConnectionString;
        private List<Flight> flights = new List<Flight>();
        private int _userId;
        public ChoseFlightWindow(int userId)
        {
            InitializeComponent();
            LoadFlights();
            _userId = userId;
            DataContext = this;
            comboBox1.ItemsSource = Flights.Select(f => f.DepartureCity).Distinct();
            comboBox2.ItemsSource = Flights.Select(f => f.DestinationCity).Distinct();
        }

        public List<Flight> Flights
        {
            get { return flights; }
            set { flights = value; }
        }

        private void LoadFlights()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT f.PNR, f.DepartureCity, f.DestinationCity, p.Plane_Type, f.Quota, f.DepartureTime, f.ArrivalTime " +
                                   "FROM Flights f " +
                                   "INNER JOIN Planes p ON f.Plane_Id = p.Id";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Flight flight = new Flight
                        {
                            PNR = (int)reader["PNR"],
                            DepartureCity = reader["DepartureCity"].ToString(),
                            DestinationCity = reader["DestinationCity"].ToString(),
                            PlaneType = reader["Plane_Type"].ToString(),
                            Quota = (int)reader["Quota"],
                            DepartureTime = (DateTime)reader["DepartureTime"],
                            ArrivalTime = (DateTime)reader["ArrivalTime"]
                        };
                        flights.Add(flight);
                    }
                    reader.Close();

                    // Bind the Flights list to the DataGrid
                    FlightsDataGrid.ItemsSource = flights;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Sort(object sender, RoutedEventArgs e)
        {
            var dep = comboBox1.Text;
            var des = comboBox2.Text;
            var date = datePicker1.SelectedDate;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT f.PNR, f.DepartureCity, f.DestinationCity, p.Plane_Type, f.Quota, f.DepartureTime, f.ArrivalTime " +
                    "FROM Flights f " +
                    "INNER JOIN Planes p ON f.Plane_Id = p.Id " +
                    "WHERE f.DepartureCity=@dep AND f.DestinationCity=@des AND CAST(f.DepartureTime AS DATE) = @date AND f.Quota > 0";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@dep", dep);
                    command.Parameters.AddWithValue("@des", des);
                    command.Parameters.AddWithValue("@date", date?.Date);
                    SqlDataReader reader = command.ExecuteReader();
                    List<Flight> filteredFlights = new List<Flight>();

                    while (reader.Read())
                    {
                        Flight flight = new Flight
                        {
                            PNR = (int)reader["PNR"],
                            DepartureCity = reader["DepartureCity"].ToString(),
                            DestinationCity = reader["DestinationCity"].ToString(),
                            PlaneType = reader["Plane_Type"].ToString(),
                            Quota = (int)reader["Quota"],
                            DepartureTime = (DateTime)reader["DepartureTime"],
                            ArrivalTime = (DateTime)reader["ArrivalTime"]
                        };
                        filteredFlights.Add(flight);
                    }
                    reader.Close();
                    FlightsDataGrid.ItemsSource = null;
                    FlightsDataGrid.ItemsSource = filteredFlights;
                    string query2 = "SELECT COUNT(*) FROM Flights WHERE DepartureCity=@dep AND DestinationCity=@des AND CAST(DepartureTime AS DATE) = @date";
                    SqlCommand command2 = new SqlCommand(query2, connection);
                    command2.Parameters.AddWithValue("@dep", dep);
                    command2.Parameters.AddWithValue("@des", des);
                    command2.Parameters.AddWithValue("@date", date?.Date);
                    int count = (int)command2.ExecuteScalar();
                    MessageBox.Show(count + " flights found between " + dep + " and " + des);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void chooseButton_Click(object sender, RoutedEventArgs e)
        {
            
                try
                {
                    Flight selectedFlight = (Flight)FlightsDataGrid.SelectedItem;
                    if (selectedFlight != null)
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            // Check if the user has already chosen the selected flight
                            string checkUserFlightQuery = "SELECT COUNT(*) FROM User_Flights WHERE userId=@userId AND pnr=@pnr";
                            SqlCommand checkUserFlightCommand = new SqlCommand(checkUserFlightQuery, connection);
                            checkUserFlightCommand.Parameters.AddWithValue("@userId", _userId);
                            checkUserFlightCommand.Parameters.AddWithValue("@pnr", selectedFlight.PNR);
                            int count = (int)checkUserFlightCommand.ExecuteScalar();
                            if (count > 0)
                                {
                                    MessageBox.Show("You have a reservation for this flight", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }
                            selectedFlight.Quota--;
                            string query = "UPDATE Flights SET Quota=@quota WHERE PNR=@pnr";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@quota", selectedFlight.Quota);
                            command.Parameters.AddWithValue("@pnr", selectedFlight.PNR);
                            command.ExecuteNonQuery();
                            string insertUserFlight = "INSERT INTO User_Flights (userId, pnr) VALUES (@userId, @pnr)";
                            SqlCommand command2 = new SqlCommand(insertUserFlight, connection);
                            command2.Parameters.AddWithValue("@userId", _userId);
                            command2.Parameters.AddWithValue("@pnr", selectedFlight.PNR);
                            command2.ExecuteNonQuery();
                            Flight updatedFlight = Flights.FirstOrDefault(f => f.PNR == selectedFlight.PNR);
                            if (updatedFlight != null)
                                {
                                    updatedFlight.Quota = selectedFlight.Quota;
                                }
                            FlightsDataGrid.ItemsSource = null;
                            FlightsDataGrid.ItemsSource = Flights;


                            MessageBox.Show("Reservation Confirmed!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
            
        }

    }
    public class Flights
    {
        public int PNR { get; set; }
        public string DepartureCity { get; set; }
        public string DestinationCity { get; set; }
        public int Plane_id { get; set; }
        public string PlaneType { get; set; }
        public int Quota { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
}

