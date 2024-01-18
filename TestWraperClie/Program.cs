using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Diagnostics.PerformanceData;
using System.Text;


class Program
{
    static void Main()
    {
        Console.WriteLine("Podaj ścieżkę do folderu:");
        string userInput = Console.ReadLine();
        string [] output = ExecutePowerShellCommand($"(ls '{userInput}' -Directory).FullName");
        List<string> directories = new List<string>();
        
        for (int i = 0; i < output.Length-1; i++)
        {
            string command = $"foreach ($group in ((get-acl {output[i]}).access | Select-Object -ExpandProperty IdentityReference).Value | Out-String -Stream ) {{$group; write-host \"\\n\" }}";
            directories = directories.Union(ExecutePowerShellCommand2(command)).ToList();
        }
        for (int i = 0; i < directories.Count; i++)
        {
            Console.WriteLine($"indeks {i} = {directories[i]}");
        }
        string[] groups = Groups(directories);

        foreach (var c in output)
        {
            foreach (var d in groups)
            {
                ExecutePowerShellCommand2($"icacls {c} /inheritance:d");
                ExecutePowerShellCommand2($"(get-acl {c}).access | where-object {{ $_.IdentityReference.Value -eq '{d}' }} | foreach-object {{ (get-acl {c}).RemoveAccessRule($_) }}; set-acl -path {c} -AclObject (get-acl {c})");
            }
        }

        Console.WriteLine("Naciśnij dowolny klawisz, aby zakończyć...");
        Console.ReadKey();
    }
    static string[] ExecutePowerShellCommand(string command)
    {
        using (Process process = new Process())
        {
            process.StartInfo.FileName = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
            process.StartInfo.Arguments = $"-Command \"{command}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string[] results = output.Split(Environment.NewLine);
            process.WaitForExit();
            return results;
        }
    }
    static List<string> ExecutePowerShellCommand2(string command)
    {
        using (Process process = new Process())
        {
            process.StartInfo.FileName = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
            process.StartInfo.Arguments = $"-Command \"{command}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            List<string> list = new List<string>(); 
            foreach (string x in output.Split("\n"))
            {
                if (x != "\\n" && CzyZawieraZnakiSpecjalneLubCyfryLubLitery(x))
                {
                    list.Add(x.Trim().ToLower());
                }
            }
            process.WaitForExit();
            return list;
        }
    }
    public static bool Inheritance()
    {
        Console.WriteLine("Would you like to remove access of certain group to a directory? (yes/no)");
        string answer = Console.ReadLine();
        while (answer != "Yes" && answer != "No" && answer != "yes" && answer != "no")
        {
            Console.WriteLine("Insert yes or no");
            answer = Console.ReadLine();
        }
        Console.WriteLine($"You Chose: {answer}");
        return (answer == "yes" || answer == "Yes");
    }
    public static string[] Groups(List<string> directories)
    {
        int helper = 1;
        string group = "";
        while (helper == 1)
        {
            Console.WriteLine("Insert name/names of group/groups that you would like to remove access\nPlease separate them with space");
            group = Console.ReadLine();
            while (group == null)
            {
                Console.WriteLine("Please provide at least one name group");
                group = Console.ReadLine();
            }
            for (int i = 0; i < group.Split().Length; i++)
            {
                if (!directories.Contains(group.Split()[i]))
                {
                    Console.WriteLine("group that you have inserted isn't on grouplist assigned for directory that you have previously inserted");
                    helper = 1;
                    break;
                }
                helper = 0;
            }
        }
        string[] groups = new string[group.Split(" ").Length];
        for (int i = 0; i < groups.Length; i++)
        {
            groups[i] = group.Split(" ")[i];
        }
        return groups;
    }
    static bool CzyZawieraZnakiSpecjalneLubCyfryLubLitery(string input) => input.Any(c => char.IsLetter(c) || char.IsDigit(c) || char.IsSymbol(c) || char.IsPunctuation(c));
}
