using Microsoft.Data.Sqlite;
using Puppy.Logger.Core.Models;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Puppy.Logger.SQLite
{
    internal class SQLiteSink : BatchProvider, ILogEventSink
    {
        private readonly string _connString;

        public SQLiteSink(string sqlLiteDbPath)
        {
            _connString = CreateConnectionString(sqlLiteDbPath);
            InitializeDatabase();
        }

        #region ILogEvent implementation

        public void Emit(LogEvent logEvent)
        {
            PushEvent(logEvent);
        }

        #endregion ILogEvent implementation

        private static string CreateConnectionString(string dbPath)
        {
            return new SqliteConnectionStringBuilder { DataSource = dbPath }.ConnectionString;
        }

        private void InitializeDatabase()
        {
            using (var conn = GetSqLiteConnection())
            {
                CreateSqlTable(conn);
            }
        }

        private SqliteConnection GetSqLiteConnection()
        {
            var sqlConnection = new SqliteConnection(_connString);
            sqlConnection.Open();
            return sqlConnection;
        }

        private void CreateSqlTable(SqliteConnection sqlConnection)
        {
            var colDefs = $"{nameof(LogInfo.Id)} TEXT PRIMARY KEY NOT NULL,";
            colDefs += $"{nameof(LogInfo.CallerMemberName)} TEXT NULL,";
            colDefs += $"{nameof(LogInfo.CallerFilePath)} TEXT NULL,";
            colDefs += $"{nameof(LogInfo.CallerRelativePath)} TEXT NULL,";
            colDefs += $"{nameof(LogInfo.CallerLineNumber)} INT NULL,";
            colDefs += $"{nameof(LogInfo.CreatedOn)} DATETIME NOT NULL,";
            colDefs += $"{nameof(LogInfo.Level)} VARCHAR(10) NOT NULL,";
            colDefs += $"{nameof(LogInfo.Message)} TEXT NULL,";
            colDefs += $"{nameof(LogInfo.ExceptionInfo)} TEXT NULL,";
            colDefs += $"{nameof(LogInfo.HttpContextInfo)} TEXT NULL";

            var sqlCreateText = $"CREATE TABLE IF NOT EXISTS {nameof(LogInfo)} ({colDefs})";
            var sqlCommand = new SqliteCommand(sqlCreateText, sqlConnection);
            sqlCommand.ExecuteNonQuery();
        }

        private SqliteCommand CreateSqlInsertCommand(SqliteConnection connection)
        {
            var sqlInsertText =
                $"INSERT INTO {nameof(LogInfo)}" +
                $" (" +
                $"{nameof(LogInfo.Id)}" +
                $", {nameof(LogInfo.CallerMemberName)}" +
                $", {nameof(LogInfo.CallerFilePath)}" +
                $", {nameof(LogInfo.CallerRelativePath)}" +
                $", {nameof(LogInfo.CallerLineNumber)}" +
                $", {nameof(LogInfo.CreatedOn)}" +
                $", {nameof(LogInfo.Level)}" +
                $", {nameof(LogInfo.Message)}" +
                $", {nameof(LogInfo.ExceptionInfo)}" +
                $", {nameof(LogInfo.HttpContextInfo)}" +
                $")";

            sqlInsertText += $" VALUES" +
                             $" (" +
                             $"@{nameof(LogInfo.Id)}" +
                             $", @{nameof(LogInfo.CallerMemberName)}" +
                             $", @{nameof(LogInfo.CallerFilePath)}" +
                             $", @{nameof(LogInfo.CallerRelativePath)}" +
                             $", @{nameof(LogInfo.CallerLineNumber)}" +
                             $", @{nameof(LogInfo.CreatedOn)}" +
                             $", @{nameof(LogInfo.Level)}" +
                             $", @{nameof(LogInfo.Message)}" +
                             $", @{nameof(LogInfo.ExceptionInfo)}" +
                             $", @{nameof(LogInfo.HttpContextInfo)}" +
                             $")";

            var sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = sqlInsertText;
            sqlCommand.CommandType = CommandType.Text;

            sqlCommand.Parameters.Add(new SqliteParameter($"@{nameof(LogInfo.Id)}", DbType.String));
            sqlCommand.Parameters.Add(new SqliteParameter($"@{nameof(LogInfo.CallerMemberName)}", DbType.String));
            sqlCommand.Parameters.Add(new SqliteParameter($"@{nameof(LogInfo.CallerFilePath)}", DbType.String));
            sqlCommand.Parameters.Add(new SqliteParameter($"@{nameof(LogInfo.CallerRelativePath)}", DbType.String));
            sqlCommand.Parameters.Add(new SqliteParameter($"@{nameof(LogInfo.CallerLineNumber)}", DbType.String));
            sqlCommand.Parameters.Add(new SqliteParameter($"@{nameof(LogInfo.CreatedOn)}", DbType.DateTimeOffset));
            sqlCommand.Parameters.Add(new SqliteParameter($"@{nameof(LogInfo.Level)}", DbType.String));
            sqlCommand.Parameters.Add(new SqliteParameter($"@{nameof(LogInfo.Message)}", DbType.String));
            sqlCommand.Parameters.Add(new SqliteParameter($"@{nameof(LogInfo.ExceptionInfo)}", DbType.String));
            sqlCommand.Parameters.Add(new SqliteParameter($"@{nameof(LogInfo.HttpContextInfo)}", DbType.String));

            return sqlCommand;
        }

        protected override void WriteLogEvent(ICollection<LogEvent> logEventsBatch)
        {
            if (logEventsBatch?.Any() != true)
            {
                return;
            }

            try
            {
                using (var sqlConnection = GetSqLiteConnection())
                {
                    using (var transaction = sqlConnection.BeginTransaction())
                    {
                        using (var sqlCommand = CreateSqlInsertCommand(sqlConnection))
                        {
                            sqlCommand.Transaction = transaction;

                            foreach (var logEvent in logEventsBatch)
                            {
                                if (logEvent == null || !Core.LoggerHelper.TryParseLogInfo(logEvent.MessageTemplate.Text, out LogInfo logInfo))
                                {
                                    return;
                                };

                                sqlCommand.Parameters[$"@{nameof(LogInfo.Id)}"].Value = logInfo.Id;
                                sqlCommand.Parameters[$"@{nameof(LogInfo.CallerMemberName)}"].Value = logInfo.CallerMemberName ?? string.Empty;
                                sqlCommand.Parameters[$"@{nameof(LogInfo.CallerFilePath)}"].Value = logInfo.CallerFilePath ?? string.Empty;
                                sqlCommand.Parameters[$"@{nameof(LogInfo.CallerRelativePath)}"].Value = logInfo.CallerRelativePath ?? string.Empty;
                                sqlCommand.Parameters[$"@{nameof(LogInfo.CallerLineNumber)}"].Value = logInfo.CallerLineNumber;
                                sqlCommand.Parameters[$"@{nameof(LogInfo.CreatedOn)}"].Value = logInfo.CreatedOn;
                                sqlCommand.Parameters[$"@{nameof(LogInfo.Level)}"].Value = logInfo.Level.ToString();
                                sqlCommand.Parameters[$"@{nameof(LogInfo.Message)}"].Value = logInfo.Message ?? string.Empty;
                                sqlCommand.Parameters[$"@{nameof(LogInfo.ExceptionInfo)}"].Value = logInfo.ExceptionInfo.ToString() ?? string.Empty;
                                sqlCommand.Parameters[$"@{nameof(LogInfo.HttpContextInfo)}"].Value = logInfo.HttpContextInfo.ToString() ?? string.Empty;
                                sqlCommand.ExecuteNonQuery();
                            }
                            transaction.Commit();
                        }
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception e)
            {
                SelfLog.WriteLine(e.Message);
            }
        }
    }
}