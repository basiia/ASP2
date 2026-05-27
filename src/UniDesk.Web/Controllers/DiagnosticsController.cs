using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace UniDesk.Web.Controllers
{
    [Route("diagnostics")]
    public class DiagnosticsController : Controller
    {
        private readonly ILogger<DiagnosticsController> _logger;

        public DiagnosticsController(ILogger<DiagnosticsController> logger)
        {
            _logger = logger;
        }

        [HttpGet("process")]
        public IActionResult ProcessMetrics()
        {
            var process = Process.GetCurrentProcess();

            var memoryBytes = process.WorkingSet64;
            var memoryMb = Math.Round(memoryBytes / 1024.0 / 1024.0, 2);
            var cpuTimeMs = Math.Round(process.TotalProcessorTime.TotalMilliseconds, 2);

            _logger.LogInformation(
                "Process diagnostics collected with memory {MemoryBytes} bytes, memory {MemoryMb} MB and CPU time {CpuTimeMs} ms for process {ProcessId}",
                memoryBytes,
                memoryMb,
                cpuTimeMs,
                process.Id);

            return Ok(new
            {
                processId = process.Id,
                memoryBytes,
                memoryMb,
                cpuTimeMs
            });
        }
    }
}