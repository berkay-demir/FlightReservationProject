using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Windows.Media.Media3D;
using System;
using System.Configuration;



namespace FlightReservationProject
{
    
    public partial class ListFlightsAdminWindow : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["FlightReservationProject.Properties.Settings.FlightReservationConnectionString"].ConnectionString;
        private List<Flight> flights = new List<Flight>();

        public ListFlightsAdminWindow()
        {
            InitializeComponent();
            LoadFlights();
            DataContext = this;
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (FlightsDataGrid.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this flight?", "Delete Flight", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Flight flight = (Flight)FlightsDataGrid.SelectedItem;
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            // Check if the quota of the flight matches the quota of the corresponding plane in the Planes table
                            string checkQuotaQuery = "SELECT p.Quota FROM Flights f INNER JOIN Planes p ON f.Plane_Id = p.Id WHERE f.PNR=@PNR";
                            SqlCommand checkQuotaCommand = new SqlCommand(checkQuotaQuery, connection);
                            checkQuotaCommand.Parameters.AddWithValue("@PNR", flight.PNR);
                            int planeQuota = (int)checkQuotaCommand.ExecuteScalar();
                            if (planeQuota != flight.Quota)
                            {
                                MessageBox.Show("A user has a ticket for this flight. You can not delete it.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            // Delete the flight from the Flights table
                            string deleteQuery = "DELETE FROM Flights WHERE PNR=@PNR";
                            SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);
                            deleteCommand.Parameters.AddWithValue("@PNR", flight.PNR);
                            int rowsAffected = deleteCommand.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                flights.Remove(flight);
                                FlightsDataGrid.Items.Refresh();
                                MessageBox.Show("Flight deleted successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Flight could not be deleted", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a flight to delete", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Add_Flights_Window_ClickButton(object sender, RoutedEventArgs e)
        {

            AddFlightWindow addFlightsAdmin = new AddFlightWindow();
            addFlightsAdmin.Show();
            this.Close();

        }
    }
    public class Flight
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