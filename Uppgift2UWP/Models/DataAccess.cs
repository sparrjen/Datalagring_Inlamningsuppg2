using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uppgift2UWP.Models;
using System.Data.SqlClient;
using Windows.Storage;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Uppgift2UWP.Models
{
    public class DataAccess 
    {

        public static Settings settings = new Settings(); 

        private static string connectionString = "Data Source=serveruppg2.database.windows.net;Initial Catalog=Dbuppg2;Persist Security Info=True;User ID=sparrjen;Password=###";

        public static async void ReadSettingsAsync()
        {
            try
            {
                //Hämtar json-filen ifrån Documents
                StorageFolder storageFolder = KnownFolders.DocumentsLibrary; 
                StorageFile settingFile = await storageFolder.GetFileAsync(@"settings.json");
                string json = await FileIO.ReadTextAsync(settingFile);
                settings = JsonConvert.DeserializeObject<Settings>(json);
            }
            catch { }
        }

        public static async Task AddAsync(Customer customer, Ticket ticket)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                //Kontrollerar om kunden med personnumret redan finns i databasen
                using (SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) from customers where SSNo = @SSNo", conn))
                {
                    conn.Open();
                    sqlCommand.Parameters.AddWithValue("@SSNo", customer.SSNo);

                    int customerInDb = (int)sqlCommand.ExecuteScalar();// hämtar kund

                    // if kunden finns, lägg endast till ärende
                    if (customerInDb != 0)
                    {
                        var query = @"INSERT INTO Tickets (CustomerId,  Created, Status, Title, Category, Description)
                        VALUES(@CustomerId, @Created, @Status, @Title, @Category, @Description) ";

                        SqlCommand cmd = new SqlCommand(query, conn);

                        cmd.Parameters.AddWithValue("@CustomerId", customer.SSNo);
                        cmd.Parameters.AddWithValue("@Created", ticket.Created);
                        cmd.Parameters.AddWithValue("@Status", ticket.Status);
                        cmd.Parameters.AddWithValue("@Title", ticket.Title);
                        cmd.Parameters.AddWithValue("@Category", ticket.Category);
                        cmd.Parameters.AddWithValue("@Description", ticket.Description);

                        await cmd.ExecuteReaderAsync();
                        conn.Close();
                    }
                    // om kund inte finns lägg till kund och ärende
                    else
                    {
                        var query = @"INSERT INTO Customers (SSNo, FirstName, LastName, PhoneNo, Email) 
                        VALUES(@SSNo, @FirstName, @LastName, @PhoneNo, @Email)
                        INSERT INTO Tickets (CustomerId, Created, Status, Title, Category, Description)
                        VALUES(@CustomerId, @Created, @Status, @Title, @Category, @Description) ";

                        SqlCommand cmd = new SqlCommand(query, conn);


                        cmd.Parameters.AddWithValue("@SSNo", customer.SSNo);
                        cmd.Parameters.AddWithValue("@FirstName", customer.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", customer.LastName);
                        cmd.Parameters.AddWithValue("@PhoneNo", customer.PhoneNo);
                        cmd.Parameters.AddWithValue("@Email", customer.Email);

                        cmd.Parameters.AddWithValue("@CustomerId", customer.SSNo);
                        cmd.Parameters.AddWithValue("@Created", ticket.Created);
                        cmd.Parameters.AddWithValue("@Status", ticket.Status);
                        cmd.Parameters.AddWithValue("@Title", ticket.Title);
                        cmd.Parameters.AddWithValue("@Category", ticket.Category);
                        cmd.Parameters.AddWithValue("@Description", ticket.Description);


                        await cmd.ExecuteReaderAsync();
                        conn.Close();
                    }
                }
            }

        }
        public static ObservableCollection<Ticket> GetAll(string status)
        {
            var customerList = new List<Customer>();
            var ticketList = new ObservableCollection<Ticket>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Hämta alla tickets utifrån status
                var query = "SELECT * FROM Customers INNER JOIN Tickets ON Customers.SSNo = Tickets.CustomerId WHERE Tickets.Status = @Status";
                // Hämta alla utifrån kundid
                //var query = "SELECT * FROM Customers, Tickets WHERE Customers.SSNo = Tickets.CustomerId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Status", status);

                var result = cmd.ExecuteReader();

                while (result.Read())
                {
                    long SSNo = result.GetInt64(0);
                    string FirstName = result.GetString(1);
                    string LastName = result.GetString(2);
                    long PhoneNo = result.GetInt64(3); //Endast Phone?
                    string Email = result.GetString(4);

                    int TicketId = result.GetInt32(5);
                    long CustomerId = result.GetInt64(6);
                    DateTime Created = result.GetDateTime(7);
                    string Status = result.GetString(8);
                    string Title = result.GetString(9);
                    string Category = result.GetString(10);
                    string Description = result.GetString(11);


                    Customer customer = new Customer(SSNo, FirstName, LastName, PhoneNo, Email); 
                    customerList.Add(customer);
                    ticketList.Add(new Ticket(TicketId, CustomerId, Created, Status, Title, Category, Description, customer));

                    //Innan Customer lades till i ticket=
                    //customerList.Add(new Customer(SSNo, FirstName, LastName, PhoneNo, Email));
                    //ticketList.Add(new Ticket(TicketId, CustomerId, Created, Status, Title, Category, Description ));


                }

                conn.Close();
                return ticketList;
            }
        }
        public static ObservableCollection<Ticket> GetAllActive()
        {
            var customerList = new List<Customer>();
            var ticketList = new ObservableCollection<Ticket>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                //Hämta de sex senaste ticketsen som är pending/active utifrån ticked id
                var query = "SELECT TOP (@Take) * FROM Customers INNER JOIN Tickets ON Customers.SSNo = Tickets.CustomerId WHERE Tickets.Status = @Status1 OR Tickets.Status = @Status2 ORDER BY Tickets.TicketId DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Take", settings.take);
                cmd.Parameters.AddWithValue("@Status1", settings.Status[0]);
                cmd.Parameters.AddWithValue("@Status2", settings.Status[1]);

                var result = cmd.ExecuteReader();

                while (result.Read())
                {
                    long SSNo = result.GetInt64(0);
                    string FirstName = result.GetString(1);
                    string LastName = result.GetString(2);
                    long PhoneNo = result.GetInt64(3);        
                    string Email = result.GetString(4);

                    int TicketId = result.GetInt32(5);
                    long CustomerId = result.GetInt64(6);
                    DateTime Created = result.GetDateTime(7);
                    string Status = result.GetString(8);
                    string Title = result.GetString(9);
                    string Category = result.GetString(10);
                    string Description = result.GetString(11);



                    Customer customer = new Customer(SSNo, FirstName, LastName, PhoneNo, Email); 
                    customerList.Add(customer);
                    ticketList.Add(new Ticket(TicketId, CustomerId, Created, Status, Title, Category, Description, customer));


                }

                conn.Close();
                return ticketList;
            }
        }
        public static ObservableCollection<Ticket> GetAllClosed()
        {
            var customerList = new List<Customer>();
            var ticketList = new ObservableCollection<Ticket>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Hämta alla Closed tickets utifrån senaste skapta ärende id
                var query = "SELECT TOP(@Take) * FROM Customers INNER JOIN Tickets ON Customers.SSNo = Tickets.CustomerId WHERE Tickets.Status = @Status ORDER BY Tickets.TicketId DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Take", settings.take);
                cmd.Parameters.AddWithValue("@Status", settings.Status[2]);

                var result = cmd.ExecuteReader();

                while (result.Read())
                {
                    long SSNo = result.GetInt64(0);
                    string FirstName = result.GetString(1);
                    string LastName = result.GetString(2);
                    long PhoneNo = result.GetInt64(3);
                    string Email = result.GetString(4);

                    int TicketId = result.GetInt32(5);
                    long CustomerId = result.GetInt64(6);
                    DateTime Created = result.GetDateTime(7);
                    string Status = result.GetString(8);
                    string Title = result.GetString(9);
                    string Category = result.GetString(10);
                    string Description = result.GetString(11);


                    Customer customer = new Customer(SSNo, FirstName, LastName, PhoneNo, Email);
                    customerList.Add(customer);
                    ticketList.Add(new Ticket(TicketId, CustomerId, Created, Status, Title, Category, Description, customer));

                    //Innan vi lade till customer:
                    //customerList.Add(new Customer(SSNo, FirstName, LastName, PhoneNo, Email));
                    //ticketList.Add(new Ticket(TicketId, CustomerId, Created, Status, Title, Category, Description));


                }

                conn.Close();
                return ticketList;
            }
        }
        public static async Task UpdateAsync(int ticketId, string status)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                //   //Uppdaterar ärendets status utifrån ticket-id
                var query = "UPDATE Tickets SET Status = @Status WHERE TicketId = @TicketId;";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TicketId", ticketId);
                cmd.Parameters.AddWithValue("@Status", status);

                await cmd.ExecuteReaderAsync();
                conn.Close();
            }
        }

    } 
}