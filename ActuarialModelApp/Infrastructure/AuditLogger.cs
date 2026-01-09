using System;
using System.IO;

namespace ActuarialModelApp.Infrastructure
{
    public static class AuditLogger
    {
        public static void Log(string message)
        {
            File.AppendAllText(
                "audit.log",
                $"{DateTime.Now:u} - {message}{Environment.NewLine}");
        }
    }
}
