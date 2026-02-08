using System.Diagnostics;

namespace MaximizeToVirtualDesktop;

static class Program
{
    private static Mutex? _mutex;

    [STAThread]
    static void Main()
    {
        // Single instance enforcement
        const string mutexName = "Global\\MaximizeToVirtualDesktop_SingleInstance";
        _mutex = new Mutex(true, mutexName, out bool createdNew);

        if (!createdNew)
        {
            // Another instance is already running
            return;
        }

        try
        {
            var logPath = Path.Combine(AppContext.BaseDirectory, "debug.log");
            Trace.Listeners.Add(new TextWriterTraceListener(logPath) { TraceOutputOptions = TraceOptions.DateTime });
            Trace.AutoFlush = true;
            Trace.WriteLine("Program: Starting...");

            ApplicationConfiguration.Initialize();
            Application.Run(new TrayApplication());
        }
        finally
        {
            _mutex.ReleaseMutex();
            _mutex.Dispose();
        }
    }
}