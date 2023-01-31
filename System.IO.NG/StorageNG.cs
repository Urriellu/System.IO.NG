using System.Diagnostics;
using System.Reflection;
using System.Threading;
using StatsNET;

namespace System.IO.NG
{
    public class StorageNG
    {
        public static void RecordStatistics(TimeSpan elapsed)
        {
            StackTrace trace1 = new StackTrace();
            MethodBase caller = trace1.GetFrame(1).GetMethod();
            string callerMethod = $"{caller.DeclaringType.Name}.{caller.Name}";
            StackTrace trace2 = new StackTrace(2);
            string stackTrace = trace2.ToString().Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\r", "\n").Replace("\n", ", ");
            int threadId = Thread.CurrentThread.ManagedThreadId;

            // per System.IO.NG method
            Statistics.IncreaseValue($"StrgUse/Mthd/{callerMethod}/Calls");
            Statistics.IncreaseValue($"StrgUse/Mthd/{callerMethod}/TotalTimeMs", elapsed.Milliseconds);

            // pre thread
            Statistics.IncreaseValue($"StrgUse/Thread/{threadId}/Calls");
            Statistics.IncreaseValue($"StrgUse/Thread/{threadId}/TotalTimeMs", elapsed.Milliseconds);

            // per stack
            Statistics.IncreaseValue($"StrgUse/Stack/{stackTrace}/Calls");
            Statistics.IncreaseValue($"StrgUse/Stack/{stackTrace}/TotalTimeMs", elapsed.Milliseconds);
        }

        internal static ProcessPriorityClass ProcessPriority => _processPriority ??= Process.GetCurrentProcess().PriorityClass;
        static ProcessPriorityClass? _processPriority;
    }
}
