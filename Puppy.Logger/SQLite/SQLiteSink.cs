using Microsoft.EntityFrameworkCore;
using Puppy.Logger.Core.Entities;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Puppy.Logger.SQLite
{
    internal class SQLiteSink : BatchProvider, ILogEventSink
    {
        public SQLiteSink()
        {
            InitializeDatabase();
        }

        private static void InitializeDatabase()
        {
            using (var db = new SqliteDbContext())
            {
                db.Database.EnsureCreated();
                db.Database.Migrate();
            }
        }

        // Implementation Sink
        public void Emit(LogEvent logEvent)
        {
            PushEvent(logEvent);
        }

        protected override void WriteLogEvent(ICollection<LogEvent> logEventsBatch)
        {
            if (logEventsBatch?.Any() != true)
            {
                return;
            }

            try
            {
                using (var db = new SqliteDbContext())
                {
                    foreach (var logEvent in logEventsBatch)
                    {
                        if (logEvent == null || !Core.LoggerHelper.TryParseLogInfo(logEvent.MessageTemplate.Text, out LogEntity logEntity))
                        {
                            return;
                        }

                        db.Logs.Add(logEntity);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                SelfLog.WriteLine(e.Message);
            }
        }
    }
}