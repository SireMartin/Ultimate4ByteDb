using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbiParser
{
    public class RootAbi
    {
        public Compiler compiler { get; set; }
        public string language { get; set; }
        public RootOutput output { get; set; }
        public Settings settings { get; set; }
        public Sources sources { get; set; }
        public int version { get; set; }
    }

    public class Compiler
    {
        public string version { get; set; }
    }

    public class RootOutput
    {
        public Abi[] abi { get; set; }
        public Devdoc devdoc { get; set; }
        public Userdoc userdoc { get; set; }
    }

    public class Devdoc
    {
        public string kind { get; set; }
        public DevDocMethods methods { get; set; }
        public int version { get; set; }
    }

    public class DevDocMethods
    {
    }

    public class Userdoc
    {
        public string kind { get; set; }
        public UserDocMethods methods { get; set; }
        public int version { get; set; }
    }

    public class UserDocMethods
    {
    }

    public class Abi
    {
        public Input[] inputs { get; set; }
        public string stateMutability { get; set; }
        public string type { get; set; }
        public bool anonymous { get; set; }
        public string name { get; set; }
        public Output[] outputs { get; set; }
    }

    public class Input
    {
        public bool indexed { get; set; }
        public string internalType { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    public class Output
    {
        public string internalType { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    public class Settings
    {
        public Compilationtarget compilationTarget { get; set; }
        public string evmVersion { get; set; }
        public Libraries libraries { get; set; }
        public Metadata metadata { get; set; }
        public Optimizer optimizer { get; set; }
        public object[] remappings { get; set; }
    }

    public class Compilationtarget
    {
        public string browserSynthetixAMMsol { get; set; }
    }

    public class Libraries
    {
    }

    public class Metadata
    {
        public string bytecodeHash { get; set; }
    }

    public class Optimizer
    {
        public bool enabled { get; set; }
        public int runs { get; set; }
    }

    public class Sources
    {
        public BrowserSynthetixammSol browserSynthetixAMMsol { get; set; }
    }

    public class BrowserSynthetixammSol
    {
        public string keccak256 { get; set; }
        public string license { get; set; }
        public string[] urls { get; set; }
    }

}
