// See https://aka.ms/new-console-template for more information

using data_safet.console.Utils;

Console.WriteLine("Enter message for workbench: ");
var message = Console.ReadLine().Trim();

Console.WriteLine("\n--------------------------------------------- RSA ------------------------------------------------\n");

RSAWorkbench.MeasureTime(message);
Console.WriteLine("\n--------------------------------------------- RC5 ------------------------------------------------\n");
RC5Workbench.MeasureTime(message);