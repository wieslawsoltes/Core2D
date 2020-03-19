using System.Reflection;

string codeBase = Assembly.GetEntryAssembly().CodeBase;
string location = Assembly.GetEntryAssembly().Location;
string path = System.IO.Path.GetDirectoryName(location);
string baseDirectory = AppContext.BaseDirectory;

Console.WriteLine($"codeBase: {codeBase}");
Console.WriteLine($"location: {location}");
Console.WriteLine($"path: {path}");
Console.WriteLine($"baseDirectory: {baseDirectory}");
