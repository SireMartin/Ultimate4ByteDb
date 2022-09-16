using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AbiParser
{
    internal class AbiManager
    {
        private Dictionary<string, StatCounter> cont = new Dictionary<string, StatCounter>();

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
                    string functionSignature = $"{iterFunction.name}({String.Join(",", iterFunction.inputs.Select(x => x.internalType))})";
                    string fourbyteFunctionSignature = new Nethereum.Util.Sha3Keccack().CalculateHash(functionSignature).Substring(0, 8);
                    Console.WriteLine($"4byte code for function {functionSignature} is {fourbyteFunctionSignature}");

                    //add the redundant 4byte code for referencing purposes to the container
                    StatCounter? fourByteStatCounter = null;
                    if (!cont.ContainsKey(fourbyteFunctionSignature))
                    {
                        fourByteStatCounter = new();
                        fourByteStatCounter.Key = fourbyteFunctionSignature;
                        //we are in the iteration of a function so there will always be a child of the 4byte sig => a function signature
                        cont.Add(fourbyteFunctionSignature, fourByteStatCounter);
                    }
                    else
                    {
                        fourByteStatCounter = cont[fourbyteFunctionSignature];
                        ++fourByteStatCounter.Occurance;
                    }

                    StatCounter? functionStatCounter = null;
                    if (!fourByteStatCounter.Child.ContainsKey(functionSignature))
                    {
                        functionStatCounter = new();
                        functionStatCounter.Key = functionSignature;
                        fourByteStatCounter.Child.Add(functionSignature, functionStatCounter);
                    }
                    else
                    {
                        functionStatCounter= fourByteStatCounter.Child[functionSignature];
                        ++functionStatCounter.Occurance;
                    }

                    //remark: if only the order of arguments differs for an equally named function, they will belong to a different 4byte code
                    foreach (var iterFunctionInputVariable in iterFunction.inputs)
                    {
                        //todo: same question as above: use internal type or type? what do those 2 mean?
                        /*
                            "internalType": "address payable",
                            "name": "to",
                            "type": "address"
                         */
                        StatCounter variableStatCounter = null;
                        if (!functionStatCounter.Child.ContainsKey(iterFunctionInputVariable.internalType))
                        {
                            variableStatCounter = new();
                            variableStatCounter.Key = iterFunctionInputVariable.name;
                            functionStatCounter.Child.Add(iterFunctionInputVariable.internalType, variableStatCounter);
                        }
                        else
                        {
                            variableStatCounter = functionStatCounter.Child[iterFunctionInputVariable.internalType];
                            ++variableStatCounter.Occurance;
                        }
                    }
                }
            }
            Console.WriteLine(JsonSerializer.Serialize(cont, options: new JsonSerializerOptions { WriteIndented = true } ));
        }
    }
}
