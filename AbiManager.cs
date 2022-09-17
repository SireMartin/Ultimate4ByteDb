using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace AbiParser
{
    internal class AbiManager
    {
        //4bytecode data based on parsed contract abi
        StatCounter rootStatCounter = new StatCounter("root", null);
        //collections to permutating
        private HashSet<string> functionNameColl = new();
        private HashSet<string[]> functionArgumentColl = new();
        int processedContractCounter = 0;
        int processedPermutationCounter = 0;
        private IDatabase _db;
        private ConnectionMultiplexer _redis;

        public void ProcessData()
        {
            _redis = ConnectionMultiplexer.Connect($"localhost:30073,abortConnect=false,ssl=false,allowAdmin=true,password=mypassword");
            _db = _redis.GetDatabase();

            foreach (var iterFilePath in Directory.EnumerateFiles("/data/sourcify/full_match/1", "metadata.json", SearchOption.AllDirectories))
            {
                /*Console.WriteLine($"{iterFilePath} will be processed");
                string[] splittedFilePath = iterFilePath.Split(Path.DirectorySeparatorChar);
                string chainId = splittedFilePath[splittedFilePath.Count() - 3];
                string contractAddress = splittedFilePath[splittedFilePath.Count() - 2];*/

            //foreach (var iterFilePath in Directory.EnumerateFiles(".", "contract*.json", SearchOption.AllDirectories))
            //{
            //    Console.WriteLine($"{iterFilePath} will be processed");

                StreamReader streamReader = new StreamReader(iterFilePath);
                RootAbi? iterAbi = JsonSerializer.Deserialize<RootAbi>(streamReader.ReadToEnd());

                foreach (var iterFunction in iterAbi.output.abi.Where(x => x.type == "function"))
                {
                    //internalType (solidity) are translate to types (abi)
                    string arguments = $"({string.Join(",", iterFunction.inputs.Select(x => x.type))})";
                    string functionSignature = iterFunction.name + arguments;
                    functionNameColl.Add(iterFunction.name);
                    functionArgumentColl.Add(iterFunction.inputs.Select(x => x.internalType).ToArray());
                    string fourbyteFunctionSignature = new Nethereum.Util.Sha3Keccack().CalculateHash(functionSignature).Substring(0, 8);
                    //Console.WriteLine($"4byte code for function {functionSignature} is {fourbyteFunctionSignature}");

                    //add the redundant 4byte code for referencing purposes to the container
                    StatCounter? fourByteStatCounter = rootStatCounter.AddChild(fourbyteFunctionSignature, fourbyteFunctionSignature, rootStatCounter);

                    StatCounter? functionStatCounter = fourByteStatCounter.AddChild(functionSignature, functionSignature, fourByteStatCounter);

                    //remark: if only the order of arguments differs for an equally named function, they will belong to a different 4byte code
                    foreach (var iterFunctionInputVariable in iterFunction.inputs.Select((x, y) => new { seq = y, obj = x }))
                    {
                        //we add a prefix to garantee uniqueness of the key
                        string key = $"{iterFunctionInputVariable.seq}_{iterFunctionInputVariable.obj.type}";
                        StatCounter inputVariableStatCounter = functionStatCounter.AddChild(key, iterFunctionInputVariable.obj.type, functionStatCounter);

                        inputVariableStatCounter.AddChild(iterFunctionInputVariable.obj.name, iterFunctionInputVariable.obj.name, inputVariableStatCounter);
                    }
                }
                if(++processedContractCounter % 1000 == 0)
                {
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss}: {processedContractCounter} contracts processed");
                }
            }

            Console.WriteLine($"{DateTime.Now:HH:mm:ss}: start permutating");
            foreach (string iterFunctionName in functionNameColl)
            {
                foreach (string[] iterArgumentTypeColl in functionArgumentColl)
                {
                    string functionSignature = $"{iterFunctionName}({string.Join(",", iterArgumentTypeColl)})";
                    string fourbyteFunctionSignature = new Nethereum.Util.Sha3Keccack().CalculateHash(functionSignature).Substring(0, 8);
                    StatCounter? fourByteStatCounter = rootStatCounter.AddChild(fourbyteFunctionSignature, fourbyteFunctionSignature, rootStatCounter);
                    fourByteStatCounter.AddChild(functionSignature, functionSignature, fourByteStatCounter);
                    ++processedPermutationCounter;
                }
            }
            Console.WriteLine($"{DateTime.Now:HH:mm:ss}: {processedPermutationCounter} permutations made");

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new StatCounterJsonConverter());

            Console.WriteLine($"{DateTime.Now:HH:mm:ss}: Start inserting in redis");
            foreach (KeyValuePair<string, StatCounter> iter in rootStatCounter.Child)
            {
                _db.StringSet(iter.Key, JsonSerializer.Serialize(iter.Value, options: options));
            }
            Console.WriteLine($"{DateTime.Now:HH:mm:ss}: End inserting in redis");

            options = new JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new StatCounterJsonConverter());
            //Console.WriteLine(JsonSerializer.Serialize(rootStatCounter, options));
        }
    }
}

//todo: what is the 4bytecode of the constructor? with or without ()?
//todo: also for events?