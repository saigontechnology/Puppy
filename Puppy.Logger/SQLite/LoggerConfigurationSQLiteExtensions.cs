using Serilog;
using Serilog.Configuration;
using Serilog.Debugging;
using Serilog.Events;
using System;
using System.IO;

namespace Puppy.Logger.SQLite
{
    /// <summary>
    ///     Adds the WriteTo.SQLite() extension method to <see cref="LoggerConfiguration" />. 
    /// </summary>
    public static class LoggerConfigurationSQLiteExtensions
    {
        /// <summary>
        ///     Adds a sink that writes log events to a SQLite database. 
        /// </summary>
        /// <param name="loggerConfiguration">      The logger configuration. </param>
        /// <param name="sqliteDbPath">             The path of SQLite db. </param>
        /// <param name="restrictedToMinimumLevel">
        ///     The minimum log event level required in order to write an event to the sink.
        /// </param>
        /// <exception cref="ArgumentNullException"> A required parameter is null. </exception>
        public static LoggerConfiguration SQLite(this LoggerSinkConfiguration loggerConfiguration, string sqliteDbPath, LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            if (loggerConfiguration == null)
            {
                SelfLog.WriteLine("Logger configuration is null");
                throw new ArgumentNullException(nameof(loggerConfiguration));
            }

            if (string.IsNullOrEmpty(sqliteDbPath))
            {
                SelfLog.WriteLine("Invalid sqliteDbPath");
                throw new ArgumentNullException(nameof(sqliteDbPath));
            }

            sqliteDbPath = sqliteDbPath.GetFullPath();

            try
            {
                var sqliteDbFile = new FileInfo(sqliteDbPath);
                sqliteDbFile.Directory?.Create();
                return loggerConfiguration.Sink(new SQLiteSink(sqliteDbFile.FullName), restrictedToMinimumLevel);
            }
            catch (Exception ex)
            {
                SelfLog.WriteLine(ex.Message);
                throw;
            }
        }
    }
}