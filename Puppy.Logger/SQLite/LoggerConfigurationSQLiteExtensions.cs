using Puppy.Core.StringUtils;
using Serilog;
using Serilog.Configuration;
using Serilog.Debugging;
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
        /// <param name="loggerConfiguration"> The logger configuration. </param>
        /// <param name="app">                </param>
        /// <exception cref="ArgumentNullException"> A required parameter is null. </exception>
        public static LoggerConfiguration SQLite(this LoggerSinkConfiguration loggerConfiguration)
        {
            if (string.IsNullOrWhiteSpace(LoggerConfig.SQLiteFilePath))
            {
                SelfLog.WriteLine($"Invalid {LoggerConfig.SQLiteFilePath}");
                throw new ArgumentNullException(nameof(LoggerConfig.SQLiteFilePath));
            }

            try
            {
                // Make sure folder is created
                var sqliteDbFile = new FileInfo(LoggerConfig.SQLiteFilePath.GetFullPath());
                sqliteDbFile.Directory?.Create();

                return loggerConfiguration.Sink(new SQLiteSink(), LoggerConfig.SQLiteLogMinimumLevelEnum);
            }
            catch (Exception ex)
            {
                SelfLog.WriteLine(ex.Message);
                throw;
            }
        }
    }
}