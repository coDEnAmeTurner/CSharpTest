using System;
using System.Data.SqlClient;

const string connectionString =
            "Persist Security Info=False;User ID=sa;Password=Admin@123;Initial Catalog=TripBooking;Server=ADMIN\\DOTNETTEST";

// Provide the query string with a parameter placeholder.
const string queryString =
    "SELECT * from dbo.[User]"
        + " WHERE userName like @name+'%'";

// Specify the parameter value.
const string name = "Mai";

// Create and open the connection in a using block. This
// ensures that all resources will be closed and disposed
// when the code exits.
using (SqlConnection connection =
    new(connectionString))
{
    // Create the Command and Parameter objects.
    SqlCommand command = new(queryString, connection);
    command.Parameters.AddWithValue("@name", name);

    // Open the connection in a try/catch block.
    // Create and execute the DataReader, writing the result
    // set to the console window.
    try
    {
        connection.Open();
        SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            Console.WriteLine($"\t{reader[0]}\t{reader[1]}\t{reader[2]}");
        }
        reader.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
    Console.ReadLine();
}
