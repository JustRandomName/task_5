using Stratage;
using System;
using System.Collections.Generic;
using System.IO;

namespace Template
{
    public class Template : IDisposable
    {
        public ILanguage _strategy { get; set; }
        public string _defaultCode { get; set; }
        public List<Unit> listChanks { get; set; }
        public Pairs[] paerses { get; set; }
        public string[] _namespaces { get; set; }

        public Template(ILanguage strategy, string defaultCode, string[] namespaces = null, params Pairs[] parametrs)
        {
            _strategy = strategy;
            _defaultCode = defaultCode;
            listChanks = new Parser(defaultCode).units;
            paerses = parametrs;
            _namespaces = namespaces;
        }

        public void Render(TextWriter output, params object[] values)
        {
            Pairs.value = values;
            output.Write(_strategy.Start(_namespaces, paerses, listChanks));
        }

        public void Dispose()
        {
        }
    }
}
