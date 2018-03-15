using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Stratage {
    public enum TokType {
        Code,
        Text,
        If,
        For,
        Error,
        Expr
    }

    class Parser {
        private string regCode = @"\{%(?<code>((?!%}).)*)%}";
        private string regIf = @"\{%\?(?<if>((?!%}).)*)%}";
        private string regFor = @"\{%@(?<for>((?!%}).)*)%}";
        private string regText = @"(?<text>((?!\{%).)+)";
        private string regExpr = @"\{%=(?<expr>((?!%}).)*)%}";
        private string regBadUnopene = @"(?<error>((?!\{%).)*%})";
        private string regBadUncloce = @"(?<error>\{%.*)";
        private string regBadEmpty = @"(?<error>^$)"; 

        public string RegexString { set; get; }

        public List<Unit> units { set; get; }

        public string TemplteString { set; get; }

        public string GetRegStr () {
            return $"({regExpr}|{regIf}|{regFor}|{regCode}|{regText}|{regBadEmpty}|{regBadUncloce}|{regBadUnopene})";
        }

        private List<Unit> GetGroups(string nameOfGroup, TokType token, Match match)
        {
            return match.Groups[nameOfGroup].Captures
            .Cast<Capture>()
            .Select(p => new Unit
            {
                Type = token,
                Value = p.Value,
                Index = p.Index
            }).ToList();
        }


        public List<Unit> ParsUnit() {

            MatchCollection matches = GetMatches();
            List<Unit> chunks = new List<Unit>();
            foreach (Match match in matches)
            {
                chunks.AddRange(GetGroups("code", TokType.Code, match));
                chunks.AddRange(GetGroups("expr", TokType.Expr, match));
                chunks.AddRange(GetGroups("if", TokType.If, match));
                chunks.AddRange(GetGroups("for", TokType.For, match));
                chunks.AddRange(GetGroups("text", TokType.Text, match));
            }
            return chunks;
        }

        public MatchCollection GetMatches() {
            Regex templeteRegex = new Regex(RegexString, RegexOptions.ExplicitCapture | RegexOptions.Singleline);
            return templeteRegex.Matches(TemplteString);
        }

        public List<Unit> GetUnit() {
            return ParsUnit().OrderBy(p => p.Index).ToList();   //sort
        }

        //public List<Unit> AddRang(List<Unit> units, Match marches) {   //add back
        //    units.AddRange(GetGroups(TokType.Expr, marches, "expr"));
        //    units.AddRange(GetGroups(TokType.Code, marches, "code"));
        //    units.AddRange(GetGroups(TokType.Error, marches, "Error"));
        //    units.AddRange(GetGroups(TokType.For, marches, "for"));
        //    units.AddRange(GetGroups(TokType.If, marches, "if"));
        //    units.AddRange(GetGroups(TokType.Text, marches, "text"));
        //    return units;
        //} 
        

        public Parser(string tempStr){
            TemplteString = tempStr;
            RegexString = GetRegStr();
            units = GetUnit();
        }

    }
}
