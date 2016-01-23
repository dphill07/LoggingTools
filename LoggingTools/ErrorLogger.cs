using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace LoggingTools
{
    public static class ErrorLogger
    {
        private static string _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Derek\Desktop\Git\ProgramAdmin\ProgramAdmin\ProgramAdmin\LoggingTools.mdf;Integrated Security=True;";
        private static int programId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["programId"]);
        private static Exception exception;
        private static string filePath;
        /// <summary>
        /// Method that pulls apart an Exception object and logs it for easy access by the developer.
        /// </summary>
        /// <param name="ex">The Exception.</param>
        public static async void LogThis(Exception ex)
        {
            exception = ex;
            try
            {
                if (ex == null || programId == 0)
                    return;

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    var command = new SqlCommand(@"INSERT INTO Errors
                                                            (TimeStamp, ProgramId, ComputerName, UserName, UserDomainName, Data, HelpLink, HResult, InnerException, Message,
                                                                Source, StackTrace, TargetSite)
                                                          VALUES
                                                            (GETDATE(), @ProgramId, @ComputerName, @UserName, @UserDomainName, @Data, @HelpLink, @HResult, @InnerException, @Message,
                                                                @Source, @StackTrace, @TargetSite)")
                    {
                        CommandType = CommandType.Text,
                        Connection = conn
                    };
                    command.Parameters.AddWithValue("@ProgramId", programId);
                    command.Parameters.AddWithValue("@ComputerName", Environment.MachineName != null ? Environment.MachineName : "");
                    command.Parameters.AddWithValue("@UserName", Environment.UserName != null ? Environment.UserName : "");
                    command.Parameters.AddWithValue("@UserDomainName", Environment.UserDomainName != null ? Environment.UserDomainName : "");
                    command.Parameters.AddWithValue("@Data", ex.Data != null ? ex.Data.ToString() : "");
                    command.Parameters.AddWithValue("@HelpLink", ex.HelpLink ?? "");
                    command.Parameters.AddWithValue("@HResult", ex.HelpLink != null ? ex.HResult.ToString() : "");
                    command.Parameters.AddWithValue("@InnerException", ex.InnerException?.ToString() ?? "");
                    command.Parameters.AddWithValue("@Message", ex.Message != null ? ex.Message : "");
                    command.Parameters.AddWithValue("@Source", ex.Source ?? "");
                    command.Parameters.AddWithValue("@StackTrace", ex.StackTrace ?? "");
                    command.Parameters.AddWithValue("@TargetSite", ex.TargetSite?.ToString() ?? "");

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                try
                {
                    var date = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;

                    var datetime = DateTime.Now.Ticks;

                    var path = @"C:\temp\ErrorLogs\" + date;

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    filePath = @"C:\temp\ErrorLogs\" + date + @"\" + datetime + ".txt";

                    await Task.Factory.StartNew(() =>
                    {
                        File.Create(filePath);
                    });
                    TextWriter tw = new StreamWriter(filePath);
                    tw.Write(exception);
                    tw.Close();
                }
                catch (Exception)
                {
                    //Ignore it if it gets to this point, there is no hope.
                }
            }
        }
    }
}