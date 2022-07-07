using MemoryMetrics.Utils;
using System.Diagnostics;
using System.Runtime.InteropServices;
namespace PoiskIT.Andromeda.Metrics
{
    public struct MemoryMetrics
    {
        public double Total;
        public double Used;
        public double Free;
        public double Percent
        {
            get { return (Used * 100) / Total; }
        }
    }

    public class MemoryMetricsClient : IDisposable
    {
        private static readonly object Lock = new object();
        private bool _disposed;

        List<MemoryMetrics> metrics;
        List<double> cpuMetrics;
        private System.Timers.Timer t;
        private const int MetricsLimit = 128;
        private const int TimeLimit = 5000;
        private const int DigitsInResult = 2;
        private static long totalMemoryInKb;
        private bool _isLinux;
        private System.Timers.ElapsedEventHandler timerHandler;
        public event EventHandler? OnMeasurementsCompleted;
        public MemoryMetricsClient()
        {
            metrics = new List<MemoryMetrics>();
            cpuMetrics = new List<double>();
            _isLinux = IsLinux();
            timerHandler = new System.Timers.ElapsedEventHandler(OnTimedEvent);
            t = new System.Timers.Timer(TimeLimit);
            t.AutoReset = true;
            t.Elapsed += timerHandler;
        }

        public void Start()
        {
            timerHandler.Invoke(t, null);
            if (!t.Enabled)
                t.Start();
        }

        public void Stop()
        {
            if (t.Enabled)
            {
                t.Stop();
                OnMeasurementsCompleted?.Invoke(this, EventArgs.Empty);
            }
        }

        protected async void OnTimedEvent(object? source, System.Timers.ElapsedEventArgs e)
        {
            if (metrics.Count > MetricsLimit)
            {
                t.Stop();
                OnMeasurementsCompleted?.Invoke(this, EventArgs.Empty);
                return;
            }
            await Task.Run(() => metrics.Add(GetMetrics()));
            await Task.Run(() => cpuMetrics.Add(GetOverallCpuUsagePercentage()));
        }

        public MemoryMetrics GetMetrics()
        {
            return _isLinux ? GetLinuxMetrics() : GetWindowsMetrics();
        }
        private bool IsLinux()
        {
            var isUnix = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                         RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            return isUnix;
            //return Environment.OSVersion.Platform == PlatformID.Unix;
        }

        private MemoryMetrics GetWindowsMetrics()
        {
            string output = String.Empty;
            var info = new ProcessStartInfo();
            info.FileName = "wmic";
            info.Arguments = "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value";
            info.RedirectStandardOutput = true;
            using (var process = Process.Start(info))
            {
                if (process == null)
                    throw new NullReferenceException();
                output = process.StandardOutput.ReadToEnd();
            }
            var lines = output.Trim().Split("\n");
            var freeMemoryParts = lines[0].Split("=", StringSplitOptions.RemoveEmptyEntries);
            var totalMemoryParts = lines[1].Split("=", StringSplitOptions.RemoveEmptyEntries);

            var metrics = new MemoryMetrics();
            metrics.Total = Math.Round(double.Parse(totalMemoryParts[1]) / 1024, 0);
            metrics.Free = Math.Round(double.Parse(freeMemoryParts[1]) / 1024, 0);
            metrics.Used = metrics.Total - metrics.Free;

            return metrics;
        }

        private MemoryMetrics GetLinuxMetrics()
        {
            var output = "";

            var info = new ProcessStartInfo("free -m");
            info.FileName = "/bin/bash";
            info.Arguments = "-c \"free -m\"";
            info.RedirectStandardOutput = true;

            using (var process = Process.Start(info))
            {
                if (process == null)
                    throw new NullReferenceException();
                output = process.StandardOutput.ReadToEnd();
                Console.WriteLine(output);
            }

            var lines = output.Split("\n");
            var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

            var metrics = new MemoryMetrics();
            metrics.Total = double.Parse(memory[1]);
            metrics.Used = double.Parse(memory[2]);
            metrics.Free = double.Parse(memory[3]);

            return metrics;
        }

        /// <summary>
        /// Get the system overall CPU usage percentage.
        /// </summary>
        /// <returns>The percentange value with the '%' sign. e.g. if the usage is 30.1234 %,
        /// then it will return 30.12.</returns>
        public double GetOverallCpuUsagePercentage()
        {
            var startTime = DateTime.UtcNow;
            var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            Thread.Sleep(500);

            var endTime = DateTime.UtcNow;
            var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            return cpuUsageTotal * 100;
        }

        /// <summary>
        /// Get the system overall memory usage percentage.
        /// </summary>
        /// <returns>The percentange value with the '%' sign. e.g. if the usage is 30.1234 %,
        /// then it will return 30.12.</returns>
        public MemoryMetrics GetOccupiedMemoryPercentage()
        {
            var metrics = new MemoryMetrics();
            metrics.Total = GetTotalMemoryInKb();
            metrics.Used = GetUsedMemoryForAllProcessesInKb();
            metrics.Free = metrics.Total - metrics.Used;
            return metrics;
        }

        private double GetUsedMemoryForAllProcessesInKb()
        {
            var totalAllocatedMemoryInBytes = Process.GetProcesses().Sum(a => a.PrivateMemorySize64);
            return totalAllocatedMemoryInBytes / 1024.0;
        }

        private long GetTotalMemoryInKb()
        {
            string path = "/proc/meminfo";
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"File not found: {path}");
            }

            using (var reader = new StreamReader(path))
            {
                string line = string.Empty;
                while (!string.IsNullOrWhiteSpace(line = reader.ReadLine()))
                {
                    if (line.Contains("MemTotal", StringComparison.OrdinalIgnoreCase))
                    {
                        // e.g. MemTotal:       26170404 kB
                        var parts = line.Split(':');
                        var valuePart = parts[1].Trim();
                        parts = valuePart.Split(' ');
                        var numberString = parts[0].Trim();

                        var result = long.TryParse(numberString, out totalMemoryInKb);
                        return result ? totalMemoryInKb : throw new Exception($"Cannot parse 'MemTotal' value from the file {path}.");
                    }
                }

                throw new Exception($"Cannot find the 'MemTotal' property from the file {path}.");
            }
        }
        public override string ToString()
        {
            if (metrics.Count <= 0 && cpuMetrics.Count <= 0)
                return "Metrics is empty";
            var fistMemory = metrics.First();
            var table = new Table();
            table.SetHeaders("", "MemoryTotal", "MemoryUsed", "MemoryFree", "Memory%", "CPU Usage");
            table.AddRow("Begin", fistMemory.Total.ToString(), fistMemory.Used.ToString(), fistMemory.Free.ToString(), fistMemory.Percent.ToString(), cpuMetrics.First().ToString());
            table.AddRow("Average", metrics.Average(x => x.Total).ToString(), metrics.Average(x => x.Used).ToString(), metrics.Average(x => x.Free).ToString(), metrics.Average(x => x.Percent).ToString(), cpuMetrics.Average().ToString());
            table.AddRow("Max", metrics.Max(x => x.Total).ToString(), metrics.Max(x => x.Used).ToString(), metrics.Max(x => x.Free).ToString(), metrics.Max(x => x.Percent).ToString(), cpuMetrics.Max().ToString());
            return table.ToString();
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            metrics.Clear();
            cpuMetrics.Clear();
            if (t.Enabled)
                t.Stop();
            t.Dispose();
            _disposed = true;
        }
    }
}