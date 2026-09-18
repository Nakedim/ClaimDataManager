
using ContractMonthlyClaimSystem_;
using Microsoft.EntityFrameworkCore;
using System;
namespace ClaimApp
{
    class Program
    {

        static void Main(string[] args)
        {
            using var db = new ClaimContext();
            db.Database.EnsureCreated();

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
                        //DeleteClaim(db);
                        break;
                    case "5":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option");
                        break;
                }
            }

        }

        static void AddClaim(ClaimContext db)
        {
            Console.WriteLine(" Welcome to Claim system");
            Console.Write("Enter lecture Name: ");
            string lecturename = Console.ReadLine();
            Console.Write("Enter hours worked : ");
            if (decimal.TryParse(Console.ReadLine(), out decimal hoursworked))
            { 
                var newClaim = new Claim
                {
               
                   LecturerName = lecturename,
                    HoursWorked = hoursworked
                };  
                  db.Claims.Add(newClaim);
                //db.SaveChanges();
                Console.WriteLine("Claim added successfully!");

                Console.WriteLine($"Generated Claim ID: {newClaim.ClaimId}");
            }
     
  
    }
        static void UpdateClaim(ClaimContext db)
        {
            Console.WriteLine("Update Claim");
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
                Console.WriteLine($"ID:          {targetClaim.ClaimId}");
                Console.WriteLine($"Amount:      R{targetClaim.TotalAmount:N2}");


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
                    }
                    else
                    {
                        Console.WriteLine($"{ClaimId} not found");
                    }
                }

            }

        }
    }
