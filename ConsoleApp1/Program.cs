using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

ConnectionStringSettings settings =
            ConfigurationManager.ConnectionStrings["TripBooking"];

string connectionString = settings.ConnectionString;

string queryString = "Select * from [User]";

using (SqlConnection connection =
    new(connectionString))
{
    SqlDataAdapter adapter = new SqlDataAdapter(queryString, connection);
    DataSet users = new DataSet();
    adapter.Fill(users, "Users");  
    var table = users.Tables["Users"];
    foreach (DataRow row in table.Rows)
    {
        Console.WriteLine($"UserId: {row["id"]}, UserName: {row["userName"]}");
    }
}
