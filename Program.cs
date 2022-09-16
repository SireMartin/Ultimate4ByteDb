using AbiParser;
using System.Text.Json;

//StreamReader streamReader = new StreamReader("contract1.json");
//Root rootObject = JsonSerializer.Deserialize<Root>(streamReader.ReadToEnd());
//foreach (var iter in rootObject.output.abi)
//{
//    Console.WriteLine($"{iter.name} is a {iter.type}");
//}
//Console.ReadLine();
//streamReader = new StreamReader("contract2.json");
//rootObject = JsonSerializer.Deserialize<Root>(streamReader.ReadToEnd());
//foreach (var iter in rootObject.output.abi)
//{
//    Console.WriteLine($"{iter.name} is a {iter.type}");
//}
//Console.ReadLine();
//streamReader = new StreamReader("contract3.json");
//rootObject = JsonSerializer.Deserialize<Root>(streamReader.ReadToEnd());
//foreach (var iter in rootObject.output.abi)
//{
//    Console.WriteLine($"{iter.name} is a {iter.type}");
//}
//Console.ReadLine();
//streamReader = new StreamReader("contract4.json");
//rootObject = JsonSerializer.Deserialize<Root>(streamReader.ReadToEnd());
//foreach (var iter in rootObject.output.abi)
//{
//    Console.WriteLine($"{iter.name} is a {iter.type}");
//}
//Console.ReadLine();

AbiManager parser = new();
parser.ProcessData();

Console.ReadLine();
