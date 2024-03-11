﻿using MakeProfits.Backend.Models;
using MakeProfits.Backend.Models.AdvisorRequests;
using MakeProfits.Models;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
namespace MakeProfits.Repository
{
    public class UserDataAccess
    {
        private readonly string connectionstring;
          public UserDataAccess(string connectionstring)
          {
            this.connectionstring = connectionstring;
            }
         public void RegisterUser(User user)
        {
            using(SqlConnection connection = new SqlConnection(connectionstring))
            { 
                connection.Open();
                using (SqlCommand command = new SqlCommand("InsertUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Parameters
                    command.Parameters.AddWithValue("@UserID", user.UserID);
                    command.Parameters.AddWithValue("@RoleID",user.RoleID);
                    command.Parameters.AddWithValue("@AddressLine",user.AddressLine);
                    command.Parameters.AddWithValue("@City",user.City);
                    command.Parameters.AddWithValue("@State",user.State);
                    command.Parameters.AddWithValue("@EmailAddress",user.EmailAddress);
                    command.Parameters.AddWithValue("@AdvisorID",user.AdvisorID);
                    command.Parameters.AddWithValue("@AgentID",user.AgentID);
                    command.Parameters.AddWithValue("@FirstName",user.FirstName);
                    command.Parameters.AddWithValue("@LastName",user.LastName);
                    command.Parameters.AddWithValue("@Company","<>");

                    command.Parameters.AddWithValue("@UserName",user.UserName);
                    command.Parameters.AddWithValue("@Password",user.Password);
                    command.Parameters.AddWithValue("@PhoneNumber",user.PhoneNumber);


                    command.ExecuteNonQuery();
                }
            }
        }
        public UserDTO getUserByUserName(string UserName)
        {
            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                conn.Open();

                //TODO : Add Log Depicting Connection established and 
                using (SqlCommand command = new SqlCommand("GetUserByName", conn))
                {

                    //TODO : Add Log Depicting Command Created
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserName", UserName);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // TODO : Add Log depicting command executed
                        UserDTO user = new UserDTO();
                        if (reader.Read())
                        {
                            user.FirstName = reader.GetString(0);
                            user.LastName = reader.GetString(1);
                            user.UserName = reader.GetString(2);
                            user.PhoneNumber = reader.GetString(3);
                            user.AddressLine = reader.GetString(4);
                            user.City = reader.GetString(5);
                            user.State = reader.GetString(6);
                            user.EmailAddress = reader.GetString(7);
                            user.AdvisorName = reader.GetString(8); 
                            user.AgentName = reader.GetString(9);
                            user.Role = reader.GetString(11);
                            

                            return user;
                        }
                        return null;
                    }
                }

                conn.Close();
            }
        }
        public  UserDTO GetUserById(int UserID)
        {
            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                conn.Open();

                //TODO : Add Log Depicting Connection established and 
                using (SqlCommand command = new SqlCommand("GetUserByID", conn))
                {

                    //TODO : Add Log Depicting Command Created
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID",UserID);

                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        // TODO : Add Log depicting command executed
                        if(reader.Read())
                        {
                            return new UserDTO();
                        }
                        return new UserDTO();
                    }
                }

                conn.Close();
            }
        }
        public bool ValidateUser(String Username,String Password)
        {
            using(SqlConnection conn = new SqlConnection(connectionstring))
            {
                conn.Open();
                using(SqlCommand command = new SqlCommand("ValidateUser",conn))
                {
                    command.CommandType= CommandType.StoredProcedure;
                    SqlParameter pr1=new    SqlParameter("@Username",Username);
                    SqlParameter pr2 = new SqlParameter("@Password", Password);
                    command.Parameters.Add(pr1 );
                    command.Parameters.Add(pr2 );
                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            if(reader.GetInt32(0)==1)
                            { return true; }
                            else { return false; }
                        }
                        return false;
                    }

                }
            }
        }

        public bool RequestAdvisory(AdvisoryRequest advisoryRequest)
        {
            advisoryRequest.RequestBY = "C";
            try
            {
                SqlConnection conn = new SqlConnection(connectionstring);
                conn.Open();
                try
                {
                    SqlCommand command = new SqlCommand("client_request_advisor",conn);
                    command.CommandType= CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@clientID",advisoryRequest.ClientID);
                    command.Parameters.AddWithValue("@advisorID",advisoryRequest.AdvisorID);
                    command.Parameters.AddWithValue("@stratergyID", advisoryRequest.StratergyID);
                    command.Parameters.AddWithValue("@Message",advisoryRequest.Message);
                    Console.WriteLine("Created and Parametrized  command");

                    try
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        
                        if(reader.Read() && reader.HasRows)
                        {
                            Console.WriteLine($"RESULT {reader.GetString(0)}");
                            reader.Close();
                            conn.Close();
                            return true;
                        }
                        reader.Close();
                        conn.Close();
                        return false;
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("Failed to read  result, Exception raised in DB");
                        conn.Close();
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to read  result, Exception raised");
                        conn.Close();
                        return true;
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Failed to Create  Command, Exception raised in DB");
                    conn.Close();
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to Create  Command, Exception raised");
                    conn.Close();
                    return true;
                }
            }
            catch(SqlException ex) {
                Console.WriteLine("Failed to Establish Connection, Exception raised in DB");
                return false;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Failed to Establish Connection, Exception raised");
                return true;
            }
        }
        public IEnumerable<Notification> ReadNotifications(int UserID)

        {
            Console.WriteLine("Request to retieve all the advisors");
            try
            {
                //string conn = _configuration.GetConnectionString("DBConnection");
                SqlConnection connection = new SqlConnection(connectionstring);
                connection.Open();
                try
                {
                    SqlCommand command = new SqlCommand("read_notifications", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID",UserID);

                    Console.WriteLine("Created and Parametrized the Comamnd");
                    try
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        Console.WriteLine("Command Executing retrieving results");
                        List<Notification> notifications = new List<Notification>();
                        Notification notification;
                        while (reader.Read())
                        {
                            notification = new Notification();
                            notification.FromUserName = reader.GetString(0);
                            notification.FromID = reader.GetInt32(1);
                            notification.ToUserName = reader.GetString(2);
                            notification.ToID = reader.GetInt32(3);
                            notification.Message = reader.GetString(4);

                            //Console.WriteLine("Read Notification Received from {FromName} to {ToName}", notification.FromUserName,notification.ToUserName);
                            Console.WriteLine($"Read Notification Received from {notification.FromUserName} to {notification.ToUserName}");

                            notifications.Add(notification);
                        }
                        //Console.WriteLine("Retrieved {count} records", notifications.Count);
                        Console.WriteLine($"Retrieved {notifications.Count} records" );
                        reader.Close();
                        connection.Close();
                        return notifications;
                    }
                    catch (SqlException ex)
                    {
                       // Console.WriteLine(ex, "Failed to Execute SP_{storedProcedure}, Exception  raised due to DBContext", "GetAdvisorsInfo");
                       Console.WriteLine("Failed to Execute SP_read_notifications, Exception  raised due to DBContext");
                        connection.Close();
                        return null;
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine(ex, "Failed to Execure SP_{storedProcedure}, Exception raised in genetal context", "GetAdvisorsInfo");
                        Console.WriteLine("Failed to Execute SP_read_notifications, Exception  raised due to general context");
                        connection.Close();
                        return null;
                    }

                }
                catch (SqlException ex)
                {
                    //Console.WriteLine(ex, "Failed to Establish a command, Exception  raised due to DBContext");
                    Console.WriteLine( "Failed to Establish a command, Exception  raised due to DBContext");
                    connection.Close();
                    return null;
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex, "Failed to Establish a command, Exception raised in genetal context");
                    Console.WriteLine("Failed to Establish a command, Exception  raised due general context");
                    connection.Close();

                    return null;
                }
            }
            catch (SqlException ex)
            {
                //Console.WriteLine(ex, "Failed to Establish a connection, Exception  raised due to DBContext");
                Console.WriteLine("Failed to Establish a connection, Exception  raised due to DBContext");
                return null;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex, "Failed to Establish a connection, Exception raised in genetal context");
                Console.WriteLine("Failed to Establish a connection, Exception raised in genetal context");
                return null;
            }
        }
    }
}
