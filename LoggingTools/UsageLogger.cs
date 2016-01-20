using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingTools
{
    public static class UsageLogger
    {
        private static readonly string _connectionString = "Server=bosql;Database=SoftwareAccess;User ID=softwareaccess;Password=G!v3M3@ccess";
        private static readonly int programId = Convert.ToInt32(ConfigurationManager.AppSettings["programId"]);
        private static string _filePath;
        private static string _programId;
        private static string _userAction;
        private static string _information;
        private static string _sqlCommand;
        private static SqlCommand _command;
        /// <summary>
        /// Method for logging any information the developer wants.
        /// </summary>
        /// <param name="information">What the developer wants to know.</param>
        public static async void LogThis(string information)
        {
            _programId = programId.ToString();
            _information = information;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand(@"INSERT INTO ProgramLogs
                                                            (ProgramId, ComputerName, UserName, UserDomainName, UserAction, Information, SQLCommand)
                                                          VALUES
                                                            (@ProgramId, @ComputerName, @UserName, @UserDomainName, @UserAction, @Information, @SQLCommand)")
                    {
                        CommandType = CommandType.Text,
                        Connection = conn
                    };
                    cmd.Parameters.AddWithValue("@ProgramId", programId);
                    cmd.Parameters.AddWithValue("@ComputerName", Environment.MachineName ?? "");
                    cmd.Parameters.AddWithValue("@UserName", Environment.UserName ?? "");
                    cmd.Parameters.AddWithValue("@UserDomainName", Environment.UserDomainName ?? "");
                    cmd.Parameters.AddWithValue("@UserAction", string.Empty);
                    cmd.Parameters.AddWithValue("@Information", _information);
                    cmd.Parameters.AddWithValue("@SQLCommand", string.Empty);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                try
                {
                    var date = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;

                    var datetime = DateTime.Now.Ticks;

                    var path = @"C:\temp\UsageLogs\" + date;

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    _filePath = @"C:\temp\UsageLogs\" + date + @"\" + datetime + ".txt";

                    await Task.Factory.StartNew(() =>
                    {
                        File.Create(_filePath);
                    });
                    TextWriter tw = new StreamWriter(_filePath);
                    tw.WriteLine("Program Id: " + _programId);
                    tw.WriteLine("User Name: " + Environment.UserName);
                    tw.WriteLine("User Domain Name: " + Environment.UserDomainName);
                    tw.WriteLine("Computer Name: " + Environment.MachineName);
                    tw.WriteLine("User Action: " + _userAction);
                    tw.WriteLine("Information: " + _information);
                    tw.WriteLine(_command.CommandText);
                    tw.Close();

                }
                catch (Exception)
                {
                    //Ignore it if it gets to this point, there is no hope.
                }
            }
        }
        /// <summary>
        /// Method that pulls apart an SqlCommand and logs what it was doing.
        /// </summary>
        /// <param name="command">The executed SqlCommand</param>
        public static async void LogThis(SqlCommand command)
        {
            _programId = programId.ToString();
            _command = command;
            try
            {
                var parameters = command.Parameters.Cast<SqlParameter>().Aggregate("", (current, p) => current + (p.Value + "\n"));

                _information = parameters;

                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand(@"INSERT INTO ProgramLogs
                                                            (ProgramId, ComputerName, UserName, UserDomainName, Information, SQLCommand)
                                                          VALUES
                                                            (@ProgramId, @ComputerName, @UserName, @UserDomainName, @Information, @SQLCommand)")
                    {
                        CommandType = CommandType.Text,
                        Connection = conn
                    };
                    cmd.Parameters.AddWithValue("@ProgramId", programId);
                    cmd.Parameters.AddWithValue("@ComputerName", Environment.MachineName != null ? Environment.MachineName : "");
                    cmd.Parameters.AddWithValue("@UserName", Environment.UserName != null ? Environment.UserName : "");
                    cmd.Parameters.AddWithValue("@UserDomainName", Environment.UserDomainName != null ? Environment.UserDomainName : "");
                    cmd.Parameters.AddWithValue("@Information", _information);
                    cmd.Parameters.AddWithValue("@SQLCommand", command.CommandText ?? "");
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                try
                {
                    var date = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;

                    var datetime = DateTime.Now.Ticks;

                    var path = @"C:\temp\UsageLogs\" + date;

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    _filePath = @"C:\temp\UsageLogs\" + date + @"\" + datetime + ".txt";

                    await Task.Factory.StartNew(() =>
                    {
                        File.Create(_filePath);
                    });
                    TextWriter tw = new StreamWriter(_filePath);
                    tw.WriteLine("Program Id: " + _programId);
                    tw.WriteLine("User Name: " + Environment.UserName);
                    tw.WriteLine("User Domain Name: " + Environment.UserDomainName);
                    tw.WriteLine("Computer Name: " + Environment.MachineName);
                    tw.WriteLine("User Action: " + _userAction);
                    tw.WriteLine("Information: " + _information);
                    tw.WriteLine(_command.CommandText);
                    tw.Close();
                }
                catch (Exception)
                {
                    //Ignore it if it gets to this point, there is no hope.
                }
            }
        }
        /// <summary>
        /// Method that pulls apart an SqlCommand and logs it, along with the action that the user took that caused the command to be executed.
        /// </summary>
        /// <param name="userAction">What the user did.</param>
        /// <param name="command">The executed SqlCommand</param>
        public static async void LogThis(string userAction, SqlCommand command)
        {
            _programId = programId.ToString();
            _userAction = userAction;
            _command = command;
            try
            {
                var parameters = command.Parameters.Cast<SqlParameter>().Aggregate("", (current, p) => current + (p.Value + "\n"));

                _information = parameters;

                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand(@"INSERT INTO ProgramLogs
                                                            (ProgramId, ComputerName, UserName, UserDomainName, UserAction, Information, SQLCommand)
                                                          VALUES
                                                            (@ProgramId, @ComputerName, @UserName, @UserDomainName, @UserAction, @Information, @SQLCommand)")
                    {
                        CommandType = CommandType.Text,
                        Connection = conn
                    };
                    cmd.Parameters.AddWithValue("@ProgramId", programId);
                    cmd.Parameters.AddWithValue("@ComputerName", Environment.MachineName != null ? Environment.MachineName : "");
                    cmd.Parameters.AddWithValue("@UserName", Environment.UserName != null ? Environment.UserName : "");
                    cmd.Parameters.AddWithValue("@UserDomainName", Environment.UserDomainName != null ? Environment.UserDomainName : "");
                    cmd.Parameters.AddWithValue("@UserAction", userAction);
                    cmd.Parameters.AddWithValue("@Information", _information);
                    cmd.Parameters.AddWithValue("@SQLCommand", command.CommandText ?? "");
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                try
                {
                    var date = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;

                    var datetime = DateTime.Now.Ticks;

                    var path = @"C:\temp\UsageLogs\" + date;

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    _filePath = @"C:\temp\UsageLogs\" + date + @"\" + datetime + ".txt";

                    await Task.Factory.StartNew(() =>
                    {
                        File.Create(_filePath);
                    });
                    TextWriter tw = new StreamWriter(_filePath);
                    tw.WriteLine("Program Id: " + _programId);
                    tw.WriteLine("User Name: " + Environment.UserName);
                    tw.WriteLine("User Domain Name: " + Environment.UserDomainName);
                    tw.WriteLine("Computer Name: " + Environment.MachineName);
                    tw.WriteLine("User Action: " + _userAction);
                    tw.WriteLine("Information: " + _information);
                    tw.WriteLine(_command.CommandText);
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