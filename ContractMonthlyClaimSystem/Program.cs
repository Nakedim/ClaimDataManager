
using ContractMonthlyClaimSystem_;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
namespace ClaimApp
{
    class Program
    {

        private static readonly string logfile = "ClaimSummary.txt";
        private static readonly string connectionString = "Data Source=claim.db";
        static void Main(string[] args)
        {
            Console.WriteLine("Initializing database...");
            InitialiseDabase();
            using var db = new ClaimContext();
            db.Database.EnsureCreated();

            bool isCreated = db.Database.EnsureCreated();

            if (isCreated)
            {
                Console.WriteLine("Database and tables successfully created!");
            }
            else
            {
                Console.WriteLine("Database already exists. No structural changes were made.");
            }

            //var allClaims = db.Claims.ToList();
            bool running = true;

            while (running)
            {
                Console.WriteLine("\n--- Claim Record Operations ---");
                Console.WriteLine("1. Add Claim");
                Console.WriteLine("2. Update Claim");
                Console.WriteLine("3. View Claims");
                Console.WriteLine("4. Delete Claim");
                Console.WriteLine("5. Press 5 to Exit");
                Console.WriteLine("6 ViewAllClaims");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddClaim(db);
                        break;
                    case "2":
                        UpdateClaim(db);
                        break;
                    case "3":
                        ViewClaim(db);
                        break;
                    case "4":
                        DeleteClaim(db);
                        break;
                    case "6":
                        ViewAllClaims();
                        break;
                    case "5":
                        running = false;
                        WriteToFile("log file written ");
                        break;
                    default:
                        Console.WriteLine("Invalid option");
                        break;
                }
            }
            if (db.Database.CanConnect())
            {
                Console.WriteLine("Database exists and is accessible!");
            }
            else
            {
                Console.WriteLine("Database does not exist yet.");
            }

        }

        static void WriteToFile(string file)
        {
            try
            {
                using var write = new StreamWriter(logfile, append: true);
                write.WriteLine($"[{DateTime.Now}] {file}");

            }
            catch (Exception e)
            {
                Console.WriteLine("error writing to file" + e.Message);
            }
        }

        static void AddClaim(ClaimContext db)
        {
            Console.WriteLine("\n--- Add a New Claim ---");
            Console.Write("Enter Lecturer Name: ");
            string lecturerName = Console.ReadLine();
            string claimStatus = "Pending";

            string moduleCode = "";

            while (string.IsNullOrWhiteSpace(moduleCode))
            {
                Console.Write("Enter Module Code (Required): ");
                moduleCode = Console.ReadLine();
            }
            string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            string SelectedMonth = "";

            while (string.IsNullOrEmpty(SelectedMonth))
            {
                Console.WriteLine("Select Claim Month:");
                for (int i = 0; i < months.Length; i++)
                    Console.WriteLine($"{i + 1}.{months[i]}");
                Console.Write("Enter choice (1-12): ");
                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= 12)
                    SelectedMonth = months[choice - 1];
            }
            {

            }


            Console.Write("Enter hours worked: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal hoursWorked))
            {
                Console.Write("Enter Hourly Rate: R");
                if (decimal.TryParse(Console.ReadLine(), out decimal hourlyRate))
                {
                    var newClaim = new ContractMonthlyClaimSystem_.Claim
                    {
                        LecturerName = lecturerName,
                        ModuleCode = moduleCode,
                        HoursWorked = hoursWorked,
                        HourlyRate = hourlyRate,
                        Status = claimStatus,
                        //ClaimMonth = DateTime.Now.ToString("MMMM")
                        ClaimMonth = SelectedMonth
                    };

                    using var conn = new SqliteConnection(connectionString);
                    conn.Open();
                    string insertSql = @"
                        INSERT INTO Claims (LecturerName, ModuleCode, HoursWorked, HourlyRate, Status, ClaimMonth)
                        VALUES (@LecturerName, @ModuleCode, @HoursWorked, @HourlyRate, @Status, @ClaimMonth);
                        SELECT last_insert_rowid();"; // Retrieve generated ID directly

                    using var command = new SqliteCommand(insertSql, conn);
                    command.Parameters.AddWithValue("@LecturerName", lecturerName);
                    command.Parameters.AddWithValue("@ModuleCode", moduleCode);
                    command.Parameters.AddWithValue("@HoursWorked", hoursWorked);
                    command.Parameters.AddWithValue("@HourlyRate", hourlyRate);
                    command.Parameters.AddWithValue("@Status", claimStatus);
                    command.Parameters.AddWithValue("@ClaimMonth", SelectedMonth);

                    long newId = (long)command.ExecuteScalar();
                    db.Claims.Add(newClaim);
                    db.SaveChanges();

                    Console.WriteLine("\nClaim successfully saved!");

                    // StreamWriter logging the event
                    WriteToFile($"ADD SUCCESS - Claim ID: {newClaim.ClaimId} | Lecturer: {lecturerName} | Module: {moduleCode} | Total: R{newClaim.TotalAmount:N2} Claim Month:{newClaim.ClaimMonth}");
                    conn.Close();

                }
                else
                {
                    Console.WriteLine("Error: Invalid Hourly Rate value.");
                }

            }
        }


        static void ViewClaim(ClaimContext db)
        {
            Console.WriteLine("\n--- list of Claims ---");
            Console.Write("Enter the Claim ID to view: ");

            if (int.TryParse(Console.ReadLine(), out int ClaimId))
            {

                var targetClaim = db.Claims.Find(ClaimId);

                if (targetClaim == null)
                {
                    Console.WriteLine($"Claim id: {ClaimId} not found");
                    return;
                }
                Console.WriteLine("\n--- Claim Details ---");
                Console.WriteLine($"ID:          {targetClaim.ClaimId + 100}");
                Console.WriteLine($"Amount:      R{targetClaim.TotalAmount:N2}");
                Console.WriteLine($"Lecture Name:      {targetClaim.LecturerName}");
                Console.WriteLine($"Module Code:      {targetClaim.ModuleCode}");
                Console.WriteLine($"Month:      {targetClaim.ClaimMonth}");
                Console.WriteLine($"Amount:      {targetClaim.Status}");


            }

            else
            {
                Console.WriteLine("invalid ID format");
            }


        }


        static void DeleteClaim(ClaimContext db)
        {
            Console.WriteLine("\n--- Delete a Claim ---");
            Console.Write("Enter the ID of the claim to delete: ");

            if (int.TryParse(Console.ReadLine(), out int ClaimId))
            {
                var claim = db.Claims.Find(ClaimId);
                if (claim != null)
                {
                    db.Claims.Remove(claim);
                    db.SaveChanges();
                    Console.WriteLine("Claim removed successfully");
                    WriteToFile($"update: removal success - Claim ID: {claim.ClaimId}");
                }
                else
                {
                    Console.WriteLine($"{ClaimId} not found");
                }
            }


        }
        static void UpdateClaim(ClaimContext db)
        {
            Console.WriteLine("\n--- update a Claim ---");
            Console.Write("Enter the ID of the claim to update: ");

            if (int.TryParse(Console.ReadLine(), out int ClaimId))
            {
                var targetClaim = db.Claims.Find(ClaimId);

                if (targetClaim != null)

                {

                    Console.WriteLine($"\nCurrent Lecturer Name: {targetClaim.Status}");
                    Console.Write("Enter new claimn status (or press Enter to keep current): ");
                    string status = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(status))
                    {
                        targetClaim.LecturerName = status;
                    }


                    db.SaveChanges();
                    Console.WriteLine("Claim updated successfully!");
                    Console.WriteLine("\n--- Claim Details ---");
                    Console.WriteLine($"ID:          {targetClaim.ClaimId + 100}");
                    Console.WriteLine($"Amount:      R{targetClaim.TotalAmount:N2}");
                    Console.WriteLine($"Lecture Name:      {targetClaim.LecturerName}");
                    Console.WriteLine($"Module Code:      {targetClaim.ModuleCode:N2}");
                    Console.WriteLine($"Amount:      {targetClaim.ClaimMonth}");
                    Console.WriteLine($"Status:      {targetClaim.Status}");
                    WriteToFile($"update: success - Claim ID: {targetClaim.ClaimId}");
                }
            }


            else
            {
                Console.WriteLine("Error: Invalid ID format.");
            }

        }




        static void InitialiseDabase()
        {
            var connect = new SqliteConnection(connectionString);
            connect.Open();

            string stringQuery = @"
                CREATE TABLE IF NOT EXISTS Claims (
                    ClaimId INTEGER PRIMARY KEY AUTOINCREMENT,
                    LecturerName TEXT,
                    ModuleCode TEXT NOT NULL,
                    HoursWorked DECIMAL NOT NULL,
                    HourlyRate DECIMAL NOT NULL,
                    Status TEXT,
                    ClaimMonth TEXT,
                    TotalAmount DECIMAL GENERATED ALWAYS AS (HoursWorked * HourlyRate) STORED
                );";

            using var comm = new SqliteCommand(stringQuery, connect);
            comm.ExecuteNonQuery();
            Console.WriteLine("database written successfully");
        }
    

    static void ViewAllClaims()
        {
            Console.WriteLine("\n--- Complete List of Database Claims ---");

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            // 1. Write the multi-row SELECT query
            string query = "SELECT ClaimId, LecturerName, ModuleCode, ClaimMonth, TotalAmount, Status FROM Claims";

            using var command = new SqliteCommand(query, connection);

            // 2. Open an active streaming reader cursor
            using var reader = command.ExecuteReader();

            if (!reader.HasRows)
            {
                Console.WriteLine("The database table is currently empty.");
                return;
            }

            // Neat grid layout layout headers
            Console.WriteLine("\nID   | Lecturer Name        | Module   | Month     | Total Amount | Status");
            Console.WriteLine("----------------------------------------------------------------------------");

            // 3. Loop through every row matching the query rules sequentially
            while (reader.Read())
            {
                Console.WriteLine(
                    $"{reader["ClaimId"],-4} | " +
                    $"{reader["LecturerName"],-20} | " +
                    $"{reader["ModuleCode"],-8} | " +
                    $"{reader["ClaimMonth"],-9} | " +
                    $"R{Convert.ToDecimal(reader["TotalAmount"]),-11:N2} | " +
                    $"{reader["Status"]}"
                );
            }
        }
    }
    }
