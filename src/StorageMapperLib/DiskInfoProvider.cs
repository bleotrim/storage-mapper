using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;

namespace StorageMapperLib;

public class DiskInfoProvider
{
    public static async Task<LsblkResult?> GetDataAsync()
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "lsblk",
            Arguments = "-b -J -o NAME,SIZE,MODEL,SERIAL,TYPE,UUID,PARTUUID,MOUNTPOINT,FSTYPE",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using Process process = Process.Start(psi);
            if (process == null)
                throw new Exception("Unable to start the lsblk process");

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            // Check if lsblk exited successfully
            if (process.ExitCode != 0)
            {
                Console.Error.WriteLine($"lsblk exited with code {process.ExitCode}: {error}");
                return null;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Deserialize JSON into strongly-typed objects
            LsblkResult? result = JsonSerializer.Deserialize<LsblkResult>(output, options);
            return result;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error while executing lsblk: {ex.Message}");
            return null;
        }
    }

    public static LsblkResult? TestWithJsonFile(string jsonFile)
    {
        if (!File.Exists(jsonFile))
            throw new FileNotFoundException("File JSON non trovato", jsonFile);

        try
        {
            string jsonContent = File.ReadAllText(jsonFile);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            LsblkResult? result = JsonSerializer.Deserialize<LsblkResult>(jsonContent, options);

            return result;

        }
        catch (JsonException ex)
        {
            Console.Error.WriteLine($"Errore nel parsing del JSON: {ex.Message}");
            return null;
        }
    }

    public static DiskInfo? FindByPartUuid(LsblkResult result, string partUuid)
    {
        if (result?.DiskInfo == null || string.IsNullOrWhiteSpace(partUuid))
            return null;

        foreach (var disk in result.DiskInfo)
        {
            var found = FindByPartUuidRecursive(disk, partUuid);
            if (found != null)
                return found;
        }

        return null;
    }

    private static DiskInfo? FindByPartUuidRecursive(DiskInfo node, string partUuid)
    {
        if (string.Equals(node.PartUuid, partUuid, StringComparison.OrdinalIgnoreCase))
            return node;

        if (node.Children != null)
        {
            foreach (var child in node.Children)
            {
                var found = FindByPartUuidRecursive(child, partUuid);
                if (found != null)
                    return found;
            }
        }

        return null;
    }
}