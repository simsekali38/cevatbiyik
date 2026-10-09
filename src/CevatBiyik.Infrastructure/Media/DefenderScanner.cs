using System.Diagnostics;
using CevatBiyik.Application.Interfaces;

namespace CevatBiyik.Infrastructure.Media;
public sealed class DefenderScanner(string? executable) : IMalwareScanner, IDisposable
{
    public void Dispose() => gate.Dispose();
    private readonly SemaphoreSlim gate = new(1);
    public async Task<bool> IsCleanAsync(string fullPath, CancellationToken ct = default)
    {
        if (!OperatingSystem.IsWindows() || string.IsNullOrWhiteSpace(executable) || !Path.IsPathFullyQualified(executable)
            || !File.Exists(executable) || !string.Equals(Path.GetFileName(executable), "MpCmdRun.exe", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        await gate.WaitAsync(ct);
        try
        {
            SafeFilePaths.NoLinks(executable);
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeout.CancelAfter(TimeSpan.FromSeconds(60));
            var info = new ProcessStartInfo(executable) { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
            foreach (var argument in new[] { "-Scan", "-ScanType", "3", "-File", fullPath, "-DisableRemediation" })
            {
                info.ArgumentList.Add(argument);
            }

            using var process = Process.Start(info);
            if (process is null)
            {
                return false;
            }
            // Drain rather than retain antivirus output: it can contain private paths and names.
            var output = process.StandardOutput.BaseStream.CopyToAsync(Stream.Null, timeout.Token);
            var error = process.StandardError.BaseStream.CopyToAsync(Stream.Null, timeout.Token);
            try { await process.WaitForExitAsync(timeout.Token); await Task.WhenAll(output, error); return process.ExitCode == 0; }
            catch (OperationCanceledException)
            {
                if (!process.HasExited)
                {
                    process.Kill(true);
                }

                try { await Task.WhenAll(output, error); } catch (OperationCanceledException) { }
                ct.ThrowIfCancellationRequested(); return false;
            }
        }
        catch (System.ComponentModel.Win32Exception) { return false; }
        finally { gate.Release(); }
    }
}
