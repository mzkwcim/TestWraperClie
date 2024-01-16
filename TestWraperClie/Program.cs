using CliWrap;
using CliWrap.Buffered;

var powerShellResults = await Cli.Wrap("Powershell")
    .WithArguments("foreach($dir in (ls C:\\Users\\Laptop\\Desktop\\UdaloSie).FullName ) \r\n{ \r\n    $dir\r\n    (get-acl $dir).Access | Select-Object -expandproperty Identityreference | Format-Table -HideTableHeaders  \r\n}")
    .ExecuteBufferedAsync();

Console.WriteLine(powerShellResults.StandardOutput);