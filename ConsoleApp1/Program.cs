using System;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Collections.Generic;

ConnectionStringSettings settings =
            ConfigurationManager.ConnectionStrings["TripBooking"];

string connectionString = settings.ConnectionString;

using (SqlConnection connection =
    new(connectionString))
{
    SqlDataAdapter customerBookTripAdapter = GetSqlDataAdapter(connection, "GetCustomerBookTrip", "InsertCustomerBookTrip", new List<SqlParameter>(){
                new SqlParameter("@CustomerBookTrip_customerId", SqlDbType.BigInt) { SourceColumn = "customerId" },
                new SqlParameter("@CustomerBookTrip_tripId", SqlDbType.BigInt) { SourceColumn = "tripId" },
                new SqlParameter("@CustomerBookTrip_placeNumber", SqlDbType.Int) { SourceColumn = "placeNumber" },
                new SqlParameter("@CustomerBookTrip_dateCreated", SqlDbType.DateTime) { SourceColumn = "dateCreated" },
                new SqlParameter("@CustomerBookTrip_dateModified", SqlDbType.DateTime) { SourceColumn = "dateModified" },
                new SqlParameter("@CustomerBookTrip_id", SqlDbType.BigInt) { SourceColumn = "id",Direction= ParameterDirection.Output },
            });

    SqlDataAdapter ticketAdapter = GetSqlDataAdapter(connection, "GetTicket", "InsertTicket", new List<SqlParameter>(){
                new SqlParameter("@Ticket_customerBookTripId", SqlDbType.BigInt) { SourceColumn = "customerBookTripId" },
                new SqlParameter("@Ticket_price", SqlDbType.Money) { SourceColumn = "price" },
                new SqlParameter("@Ticket_sellerCode", SqlDbType.VarChar, 200) { SourceColumn = "sellerCode" },
                new SqlParameter("@Ticket_dateCreated", SqlDbType.DateTime) { SourceColumn = "dateCreated" },
                new SqlParameter("@Ticket_dateModified", SqlDbType.DateTime) { SourceColumn = "dateModified" },
                new SqlParameter("@Ticket_id", SqlDbType.BigInt) { SourceColumn = "id",Direction= ParameterDirection.Output },
            });

    DataSet dataSet = new DataSet();
    customerBookTripAdapter.Fill(dataSet);

    var customerBookTrips = dataSet.Tables[0];

    customerBookTrips.Rows.Add(
        new object[]
        {
            customerBookTrips.Rows.Count,
            4,
            1,
            5,
            DateTime.Now,
            DateTime.Now,
        });
    customerBookTripAdapter.Update(dataSet);

    var cbtId = Int64.Parse(customerBookTrips.Rows[customerBookTrips.Rows.Count - 1]["id"].ToString());

    DataSet tdataSet = new DataSet();
    ticketAdapter.Fill(tdataSet);

    var tickets = tdataSet.Tables[0];
    tickets.Rows.Add(
        new object[]
        {
            cbtId,
            20000,
            "SellerCode123",
            DateTime.Now,
            DateTime.Now.AddDays(5),
        });
    ticketAdapter.Update(tdataSet);
    
    foreach (DataRow row in tickets.Rows)
    {
        Console.WriteLine($"Ticket ID: {row["customerBookTripId"]}, Price: {row["price"]}, Seller Code: {row["sellerCode"]}");
    }
}

static SqlDataAdapter GetSqlDataAdapter(SqlConnection connection, string SelectProcedure, string InsertProcedure, List<SqlParameter> insertParameters)
{
    var adapter = new SqlDataAdapter()
    {
        SelectCommand = new SqlCommand()
        {
            CommandText = SelectProcedure,
            Connection = connection,
            CommandType = CommandType.StoredProcedure,
        },
        InsertCommand = new SqlCommand()
        {
            CommandText = InsertProcedure,
            Connection = connection,
            CommandType = CommandType.StoredProcedure,
            UpdatedRowSource = UpdateRowSource.OutputParameters
        },

        AcceptChangesDuringUpdate = true,
    };

    adapter.InsertCommand.Parameters.AddRange(insertParameters.ToArray());
    
    return adapter;
}