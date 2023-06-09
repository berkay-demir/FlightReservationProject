﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace FlightReservationProject
{
    public partial class AddFlightWindow : Window
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["FlightReservationProject.Properties.Settings.FlightReservationConnectionString"].ConnectionString;
        
        public AddFlightWindow()
        {
            InitializeComponent();
            LoadCities();
            LoadPlaneTypes();
        }

        private void LoadCities()
        {
            string query = "SELECT Id, City FROM Cities ORDER BY City";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    DepartureCityComboBox.Items.Add((string)reader["City"]);
                    DestinationCityComboBox.Items.Add((string)reader["City"]);
                }
                reader.Close();
            }
        }

        private void LoadPlaneTypes()
        {
            string query = "SELECT Id, Plane_Type, Quota FROM Planes ORDER BY Plane_Type";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    PlaneTypeComboBox.Items.Add((string)reader["Plane_Type"]);
                }
                reader.Close();
            }
        }

        private void Add_Flight_ClickButton(object sender, RoutedEventArgs e)
        {
            string departureCity = DepartureCityComboBox.SelectedItem.ToString();
            string destinationCity = DestinationCityComboBox.SelectedItem.ToString();
            if (departureCity == destinationCity)
            {
                MessageBox.Show("Departure city and destination city cannot be the same.");
                return;
            }
            string planeType = PlaneTypeComboBox.SelectedItem.ToString();
            DateTime departureDate = DepartureDatePicker.SelectedDate.Value;
            TimeSpan departureTime;
            if (!TimeSpan.TryParse(DepartureTimeTextBox.Text, out departureTime))
            {
                MessageBox.Show("Invalid departure time format. Please enter the time in the format HH:mm.");
                return;
            }
            float flightLength;
            if (!float.TryParse(FlightLengthTextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out flightLength))
            {
                MessageBox.Show("Invalid flight length. Please enter a valid number of hours.");
                return;
            }
            DateTime arrivalTime = departureDate.Add(departureTime).AddHours(flightLength);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Flights (DepartureCity, DestinationCity, Quota, DepartureTime, ArrivalTime,Plane_Id) " +
                               "VALUES ( @departureCity, @destinationCity, @quota, @departureTime, @arrivalTime,@planeId)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    

                    command.Parameters.AddWithValue("@departureCity", departureCity);
                    command.Parameters.AddWithValue("@destinationCity", destinationCity);
                    command.Parameters.AddWithValue("@quota", GetPlaneQuota(planeType));
                    command.Parameters.AddWithValue("@departureTime", departureDate.Add(departureTime));
                    command.Parameters.AddWithValue("@arrivalTime", arrivalTime);
                    command.Parameters.AddWithValue("@planeId", GetPlaneId(planeType));
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Flight added successfully.");
            

        }

        private int GetPlaneQuota(string planeType)
        {
            int quota = 0;
            string query = "SELECT Quota FROM Planes WHERE Plane_Type = @planeType";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@planeType", planeType);
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    quota = (int)result;
                }
            }
            return quota;
        }
        private int GetPlaneId(string planeType)
        {
            int plane_id=0;
            string query = "SELECT Id FROM Planes WHERE Plane_Type = @planeType";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@planeType", planeType);
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    plane_id = (int)result;
                }
            }
            return plane_id;
        }

        private void List_Flights_Window_ClickButton(object sender, RoutedEventArgs e)
        {
            
            ListFlightsAdminWindow listFlightsAdmin = new ListFlightsAdminWindow();
            listFlightsAdmin.Show();
            this.Close();

        }

    }
}