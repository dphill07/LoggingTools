using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace LoggingTools
{
    public static class ReleaseNotes
    {
        private const string ConnectionString = "Server=bosql;Database=SoftwareAccess;User ID=softwareaccess;Password=G!v3M3@ccess";
        private static readonly int ProgramId = Convert.ToInt32(ConfigurationManager.AppSettings["programId"]);
        public static void Initialize()
        {
            var major = Properties.Settings.Default.Major;
            var minor = Properties.Settings.Default.Minor;
            var build = Properties.Settings.Default.Build;
            var revision = Properties.Settings.Default.Revision;

            var version = GetPublishedVersion();

            if (version.Major != major || version.Minor != minor || version.Build != build || version.Revision != revision)
            {
                ShowReleaseNotes();
            }

            Properties.Settings.Default.Major = version.Major;
            Properties.Settings.Default.Minor = version.Minor;
            Properties.Settings.Default.Build = version.Build;
            Properties.Settings.Default.Revision = version.Revision;
            Properties.Settings.Default.Save();
        }
        public static void ShowReleaseNotes()
        {
            var message = string.Empty;
            var hasNotes = false;
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT * FROM ReleaseNotes WHERE ProgramId = @ProgramId ORDER BY [Date] DESC")
                {
                    CommandType = CommandType.Text,
                    Connection = conn
                };
                cmd.Parameters.AddWithValue("@ProgramId", ProgramId);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    hasNotes = true;
                    message += "~~~";
                    message += reader["Date"] != DBNull.Value ? Convert.ToDateTime(reader["Date"]).Date.ToShortDateString() : DateTime.MinValue.ToShortDateString();
                    message += "~~~";
                    message += Environment.NewLine;
                    message += reader["Notes"] + Environment.NewLine;
                    message += "---------------------------------------" + Environment.NewLine;
                }
            }
            if (hasNotes)
            {
                MessageBox.Show(message, "Release Notes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        public static Version GetPublishedVersion()
        {
            var xmlDoc = new XmlDocument();
            var asmCurrent = Assembly.GetEntryAssembly();
            var executePath = new Uri(asmCurrent.GetName().CodeBase).LocalPath;

            xmlDoc.Load(executePath + ".manifest");
            var retval = string.Empty;
            if (!xmlDoc.HasChildNodes)
                return new Version(retval);
            var xmlAttributeCollection = xmlDoc.ChildNodes[1].ChildNodes[0].Attributes;
            if (xmlAttributeCollection != null)
                retval = xmlAttributeCollection.GetNamedItem("version").Value;
            return new Version(retval);
        }
    }
}