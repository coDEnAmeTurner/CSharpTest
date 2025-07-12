using System;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;

ConnectionStringSettings settings =
            ConfigurationManager.ConnectionStrings["TripBooking"];

string connectionString = settings.ConnectionString;

string queryString = "Select * from [Route]";

using (SqlConnection connection =
    new(connectionString))
{
    SqlDataAdapter adapter = new SqlDataAdapter(queryString, connection);
    adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
    DataSet routes = new DataSet();
    adapter.Fill(routes, "Routes");
    var table = routes.Tables["Routes"];
    foreach (DataRow row in table.Rows)
    {
        Console.WriteLine($"id: {row["id"]}, routeDescription: {row["routeDescription"]}, [dateCreated]: {row["dateCreated"]}, , [dateModified]: {row["dateModified"]}");
    }

    //update
    adapter.UpdateCommand = new SqlCommand("Update [Route] set routeDescription = @RouteDescription where id = @Id", connection);
    table.Rows[1]["routeDescription"] = "Hanoi to Quang Binh";
    var parameterD = new SqlParameter()
    {
        ParameterName = "@RouteDescription",
        SqlDbType = SqlDbType.NVarChar,
        Size = 300,
        SourceColumn = "routeDescription",
        SourceVersion = DataRowVersion.Current
    };
    adapter.UpdateCommand.Parameters.Add(parameterD);

    var parameter = new SqlParameter()
    {
        ParameterName = "@Id",
        SqlDbType = SqlDbType.Int,
        SourceColumn = "id",
        SourceVersion = DataRowVersion.Original
    };
    adapter.UpdateCommand.Parameters.Add(parameter);
    adapter.UpdateCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;

    //insert
    //the ids for insert can be anything, would be neglected all the same
    table.Rows.Add(new object[] {
         40, "Saigon to Tra Vinh", DateTime.Now, DateTime.Now
    });
    table.Rows.Add(new object[] {
         45, "Saigon to Vung Tau", DateTime.Now, DateTime.Now
    });
    adapter.InsertCommand = new SqlCommand("InsertRoutes", connection) { CommandType = CommandType.StoredProcedure };
    adapter.InsertCommand.Parameters.Add(new SqlParameter("@RouteDescription", SqlDbType.NVarChar, 300, "routeDescription"));
    adapter.InsertCommand.Parameters.Add(new SqlParameter("@DateCreated", SqlDbType.DateTime, 0, "dateCreated"));
    adapter.InsertCommand.Parameters.Add(new SqlParameter("@DateModified", SqlDbType.DateTime, 0, "dateModified"));
    //By default, @Identity takes value from id, but UpdateRowSource is set to OutputParameters, so id now takes value from @Identity
    adapter.InsertCommand.Parameters.Add(new SqlParameter("@Identity", SqlDbType.BigInt, 0,"id") { Direction = ParameterDirection.Output });
    adapter.InsertCommand.UpdatedRowSource = UpdateRowSource.OutputParameters;


    //delete
    table.Rows[2].Delete();
    table.Rows[3].Delete();
    adapter.DeleteCommand = new SqlCommand("Delete from [Route] where id = @Id", connection) { CommandType = CommandType.Text };
    var parameterId = new SqlParameter()
    {
        ParameterName = "@Id",
        SqlDbType = SqlDbType.BigInt,
        SourceColumn = "id",
        SourceVersion = DataRowVersion.Current
    };
    adapter.DeleteCommand.Parameters.Add(parameterId);
    //must set so that the changes in db (which are what matter) are reflected in the DataSet
    adapter.AcceptChangesDuringUpdate = true;
    adapter.Update(routes, "Routes");

    Console.WriteLine("After insert and update and delete:");
    foreach (DataRow row in table.Rows)
    {
        Console.WriteLine($"id: {row["id"]}, routeDescription: {row["routeDescription"]}, [dateCreated]: {row["dateCreated"]}, , [dateModified]: {row["dateModified"]}");
    }
}
