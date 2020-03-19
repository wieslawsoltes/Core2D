using System.Reflection;
using static System.Console;

string codeBase = Assembly.GetEntryAssembly().CodeBase;
string location = Assembly.GetEntryAssembly().Location;
string path = System.IO.Path.GetDirectoryName(location);
string baseDirectory = AppContext.BaseDirectory;

WriteLine($"codeBase: {codeBase}");
WriteLine($"location: {location}");
WriteLine($"path: {path}");
WriteLine($"baseDirectory: {baseDirectory}");
