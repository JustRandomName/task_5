using Stratage;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Stratage.localhost;
using Stratage.RubyServer;
using System.Text.RegularExpressions;
using System.Linq;

namespace Template
{
    public interface ILanguage
    {
        string Start(string[] namespaces, Pairs[] pearxes, List<Unit> chanks);
    }

    public class CSharp : ILanguage
    {
        private string head = "using System; class Program{ static public string Metod(";
        private string tail = "return resultString;}}";
        Assembly assembly { get; set; }


        private string GetNewText(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char ch in value)
            {
                sb.Append("\\u" + ((uint)ch).ToString("x4"));
            }
            return sb.ToString();
        }

        private string AddArguments(string[] namespaces)
        {
            string resultCode = "";
            foreach (string str in namespaces)
            {
                resultCode += str + ";\n";
            }
            return resultCode;
        }

        private string AddCunks(List<Unit> chanks)
        {
            string resultCode = "";
            foreach (Unit chank in chanks)
            {
                switch (chank.Type)
                {
                    case TokType.Code: resultCode += chank.Value + "\n"; break;
                    case TokType.Expr: resultCode += "resultString+=" + chank.Value + ";\n"; break;
                    case TokType.If: resultCode += "if(" + chank.Value + ")\n"; break;
                    case TokType.For: resultCode += "for(int i=0;i<" + chank.Value + ";i++)\n"; break;
                    case TokType.Text: resultCode += "resultString+=\"" + GetNewText( chank.Value) + "\";"; break;
                }
            }
            return resultCode;
        }


        private string GetResaltCode(string[] namespaces, Pairs[] pearses, List<Unit> chanks)
        {
            string resultCode = (namespaces != null) ? AddArguments(namespaces) : "";
            resultCode += head + AddList(pearses) + ") \n{string resultString= \"\";\n" + AddCunks(chanks) + tail;
            return resultCode;
        }

        private string AddList(Pairs[] list)
        {
            string resultString = "";
            foreach (Pairs paer in list)
            {
                resultString += paer.type + " " + paer.name;
                if (paer != list[list.GetLength(0) - 1])
                    resultString += ",";
            }
            return resultString;
        }

        public bool Compiler(string defaultCode)
        {
            CSharpCodeProvider provider = GetProvider();
            CompilerResults results = provider.CompileAssemblyFromSource(GetCompilerParam(), defaultCode);
            if (results.Errors.HasErrors)
            {
                return false;
            }
            assembly = results.CompiledAssembly;
            return true;
        }

        private CompilerParameters GetCompilerParam()
        {
            CompilerParameters parametrs = new CompilerParameters();
            parametrs.CompilerOptions = "/optimize";
            parametrs.GenerateExecutable = false;
            parametrs.GenerateInMemory = true;
            parametrs.IncludeDebugInformation = false;
            return parametrs;
        }

        private CSharpCodeProvider GetProvider()
        {
            Dictionary<string, string> options = new Dictionary<string, string> { { "CompilerVersion", "v3.5" } };
            CSharpCodeProvider provider = new CSharpCodeProvider(options);
            return provider;
        }

        public string Run(object[] values)
        {
            string result = "";
            System.Type type = assembly.GetType("Program");
            MethodInfo method = type.GetMethod("Metod");
            assembly.CreateInstance(type.FullName);
            try
            {
                result = (string)method.Invoke(null, values);
            } catch (Exception e)
            {
                result = "error";
            }
            return result;
        }
        public string Start(string[] namespaces, Pairs[] pearses, List<Unit> chanks)
        {
            string result = "error";
            if (Compiler(GetResaltCode(namespaces, pearses, chanks)))
            {
                result = Run(Pairs.value);
            }
            return result;
        }
    }

    public class Java : ILanguage
    {
        private string head = "import java.lang.*; public class Main{ public static void main(String[] argc){";
        private string tail = "System.out.println(resultString);}}";
        bool flag = false;
        private string AddCunks(List<Unit> chanks)
        {
            string resultCode = "";
            foreach (Unit chank in chanks)
            {
                switch (chank.Type)
                {
                    case TokType.Code: resultCode += chank.Value + "\n"; break;
                    case TokType.Expr: resultCode += AddExpresion(chank); break;
                    case TokType.If: resultCode += "if(" + chank.Value + ")\n"; break;
                    case TokType.For: resultCode += "for(int i=0;i<" + chank.Value + ";i++)"; break;
                    case TokType.Text: resultCode += "resultString+=\"" + GetNewText(chank.Value) + "\";"; break;
                }
            }
            return resultCode;
        }

        private string AddExpresion(Unit chank)
        {
            string resultString = "resultString+=";
            resultString += "" + chank.Value.ToString() + ";\n";
            return resultString;
        }


        private string GetResaltCode(string[] namespaces, Pairs[] pearses, List<Unit> chanks)
        {
                string resultCode = AddArguments(namespaces);
                resultCode += head + "String resultString= \"\";\n" + AddList(pearses) + AddCunks(chanks) + tail;
                return resultCode;
        }


        private string AddList(Pairs[] list)
        {
            string resultString = "";
            int i = 0;
            foreach (Pairs paer in list)
            {
                switch (paer.type)
                {
                    case "int ": resultString += "int " + paer.name + "=" + Pairs.value[i++].ToString() + ";\n"; break;
                    case "string": resultString += "String " + paer.name + "=\"" + Pairs.value[i++].ToString() + "\";\n"; break;
                    case "bool": resultString += "boolean " + paer.name + "=" + Pairs.value[i++].ToString().ToLower() + ";\n"; break;
                    default: break;
                }
            }
            return resultString;
        }
        private string AddArguments(string[] namespaces)
        {
            string resultCode = "";
            if (namespaces != null)
                foreach (string str in namespaces)
                {
                    resultCode += str + ";\n";
                }
            return resultCode;
        }

        private string GetNewText(string value)
        {
            flag = true;
            StringBuilder sb = new StringBuilder();
            foreach (char ch in value)
            {
                sb.Append("\\\\u" + ((uint)ch).ToString("x4"));
            }
            return sb.ToString();
        }

        public string Start(string[] namespaces, Pairs[] pearxes, List<Unit> chanks)
        {
            JavaCompilerWebServiceImplService java = new JavaCompilerWebServiceImplService();
            string res = java.compileCode(GetResaltCode(namespaces, pearxes, chanks));
            string result = res;
            return result;
        }

        private string Get(string res)
        {
            if (flag)
            {
                string pattern = @"(?<!\\\\u)\w{4}\b";
                Regex reg = new Regex(pattern);
                MatchCollection collection = reg.Matches(res);
                StringBuilder sb = new StringBuilder();
                int i = 0;
                foreach (Match match in collection)
                {
                    i = int.Parse(match.Value, System.Globalization.NumberStyles.AllowHexSpecifier);
                    // if (i == 0) { sb.Append("\\0"); continue; } я сделялъ 
                    sb.Append((char)i);
                }
                string text = string.Join(string.Empty, Enumerable.Repeat(sb.ToString(), 1));
                flag = false;
                return text;
            }
            else {
                return res;
            }
        }
    }


}
