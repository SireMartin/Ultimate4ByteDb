using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AbiParser
{
    internal class AbiManager
    {
        //4bytecode data based on parsed contract abi
        private Dictionary<string, StatCounter> parsedContractAbiColl = new();
        //collections to permutating
        private HashSet<string> functionNameColl = new();
        private HashSet<string[]> functionArgumentColl = new();

        public void ProcessData()
        {
            foreach (var iterFile in Directory.EnumerateFiles(".", "contract*.json"))
            {
                Console.WriteLine($"{iterFile} will be processed");
                StreamReader streamReader = new StreamReader(iterFile);
                RootAbi? iterAbi = JsonSerializer.Deserialize<RootAbi>(streamReader.ReadToEnd());

                foreach (var iterFunction in iterAbi.output.abi.Where(x => x.type == "function"))
                {
                    //todo: what is the 4bytecode of the constructor? with or without ()?
                    //todo: also for events?
                    //todo: use internal type or type? what do those 2 mean?
                    /*
                        "internalType": "address payable",
						"name": "to",
						"type": "address"
                     */
                    string arguments = $"({string.Join(",", iterFunction.inputs.Select(x => x.internalType))})";
                    string functionSignature = iterFunction.name + arguments;
                    functionNameColl.Add(iterFunction.name);
                    functionArgumentColl.Add(iterFunction.inputs.Select(x => x.internalType).ToArray());
                    string fourbyteFunctionSignature = new Nethereum.Util.Sha3Keccack().CalculateHash(functionSignature).Substring(0, 8);
                    Console.WriteLine($"4byte code for function {functionSignature} is {fourbyteFunctionSignature}");

                    //add the redundant 4byte code for referencing purposes to the container
                    StatCounter? fourByteStatCounter = null;
                    if (!parsedContractAbiColl.ContainsKey(fourbyteFunctionSignature))
                    {
                        fourByteStatCounter = new();
                        fourByteStatCounter.Name = fourbyteFunctionSignature;
                        fourByteStatCounter.Type = StatCounter.StatCounterType.FourByteCode;
                        //we are in the iteration of a function so there will always be a child of the 4byte sig => a function signature
                        parsedContractAbiColl.Add(fourbyteFunctionSignature, fourByteStatCounter);
                    }
                    else
                    {
                        fourByteStatCounter = parsedContractAbiColl[fourbyteFunctionSignature];
                        ++fourByteStatCounter.Occurance;
                    }

                    StatCounter? functionStatCounter = null;
                    if (!fourByteStatCounter.Child.ContainsKey(functionSignature))
                    {
                        functionStatCounter = new();
                        functionStatCounter.Name = functionSignature;
                        functionStatCounter.Type = StatCounter.StatCounterType.FunctionSignature;
                        fourByteStatCounter.Child.Add(functionSignature, functionStatCounter);
                    }
                    else
                    {
                        functionStatCounter= fourByteStatCounter.Child[functionSignature];
                        ++functionStatCounter.Occurance;
                    }

                    //remark: if only the order of arguments differs for an equally named function, they will belong to a different 4byte code
                    foreach (var iterFunctionInputVariable in iterFunction.inputs.Select((x, y) => new { seq = y, obj = x }))
                    {
                        //todo: same question as above: use internal type or type? what do those 2 mean?
                        /*
                            "internalType": "address payable",
                            "name": "to",
                            "type": "address"
                         */
                        StatCounter inputVariableStatCounter = null;
                        //we add a prefix to garantee uniqueness of the key
                        string key = $"{iterFunctionInputVariable.seq}_{iterFunctionInputVariable.obj.internalType}";
                        if (!functionStatCounter.Child.ContainsKey(key))
                        {
                            inputVariableStatCounter = new();
                            inputVariableStatCounter.Name = iterFunctionInputVariable.obj.internalType;
                            inputVariableStatCounter.Type = StatCounter.StatCounterType.InputVariable;
                            functionStatCounter.Child.Add(key, inputVariableStatCounter);
                        }
                        else
                        {
                            inputVariableStatCounter = functionStatCounter.Child[key];
                            ++inputVariableStatCounter.Occurance;
                        }

                        StatCounter variableNameStatCounter = null;
                        if (!inputVariableStatCounter.Child.ContainsKey(iterFunctionInputVariable.obj.name))
                        {
                            variableNameStatCounter = new();
                            variableNameStatCounter.Name = iterFunctionInputVariable.obj.name;
                            variableNameStatCounter.Type = StatCounter.StatCounterType.VariableName;
                            inputVariableStatCounter.Child.Add(iterFunctionInputVariable.obj.name, variableNameStatCounter);
                        }
                        else
                        {
                            variableNameStatCounter = inputVariableStatCounter.Child[iterFunctionInputVariable.obj.name];
                            ++variableNameStatCounter.Occurance;
                        }
                    }
                }
            }
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
            options.Converters.Add(new StatCounterJsonConverter());
            Console.WriteLine(JsonSerializer.Serialize(parsedContractAbiColl, options: options ));

            //todo: write parsed contracts abi 4byte data to redis

            //permutate the collections and write to redis
            foreach (string iterFunctionName in functionNameColl)
            {
                foreach (string[] iterArgumentTypeColl in functionArgumentColl)
                {
                    string functionSig = $"{iterFunctionName}({string.Join(",", iterArgumentTypeColl)})";
                    string fourbyteCode = new Nethereum.Util.Sha3Keccack().CalculateHash(functionSig).Substring(0, 8);
                    if (!parsedContractAbiColl.ContainsKey(fourbyteCode))
                    {
                        var perm = new { fctSelector = fourbyteCode, occurance = 1, 
                            fctSigs = new[] 
                            { 
                                new {
                                    fctSig = functionSig,
                                    occurance = 1,
                                    likelyhood = 1,
                                    inputVars = new[] 
                                    { 
                                        iterArgumentTypeColl.Select(x => new 
                                        { 
                                            varType = x
                                        })
                                    }
                                }
                            }
                        };
                        //push to redis
                        Console.WriteLine(JsonSerializer.Serialize(perm, options: options ));
                    }
                    
                }
            }
        }
    }
}
