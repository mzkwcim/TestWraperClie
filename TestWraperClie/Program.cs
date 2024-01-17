using System;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        Console.WriteLine("Podaj ścieżkę do folderu:");

        // Odczytaj dane od użytkownika
        string userInput = Console.ReadLine();

        // Wywołaj PowerShell
        string output = ExecutePowerShellCommand($"$data = '{userInput}'; $dir = (ls $data).FullName");
        foreach( string line in output.Split())
        {
            ExecutePowerShellCommand($"get-acl {line} | Select-object -ExpandProperty access | Select-Object -ExpandProperty IdentityReference | Format-Table -HideTableHeaders");
        }

        Console.WriteLine("Naciśnij dowolny klawisz, aby zakończyć...");
        Console.ReadKey();
    }

    static string ExecutePowerShellCommand(string command)
    {
        using (Process process = new Process())
        {
            process.StartInfo.FileName = "powershell";
            process.StartInfo.Arguments = $"-Command {command}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            // Pobierz wynik wykonania polecenia
            string output = process.StandardOutput.ReadToEnd();
            Console.WriteLine(output);

            process.WaitForExit();
            return output;
        }
    }
}
