using System.Collections.Generic;
using System.Linq;
using SharpPascal.Syntax;

namespace SharpPascal.Semantic
{
    public class Scope
    {
        private readonly Scope? _previous;

        private readonly ICollection<Symbol> _symbols = new HashSet<Symbol>();

        public Scope(Scope? previous = null)
        {
            _previous = previous;
        }

        public void Add(Symbol symbol)
            => _symbols.Add(symbol);

        public Symbol? FindLocal(PascalName name)
            => _symbols.FirstOrDefault(s => s.Name == name);

        public Symbol? Find(PascalName name)
            => UntilLastScope
                .Select(scope => scope.FindLocal(name))
                .SkipWhile(s => s is null)
                .FirstOrDefault();

        protected IEnumerable<Scope> UntilLastScope
        {
            get
            {
                for (var scope = this; scope != null; scope = scope._previous)
                {
                    yield return scope;
                }
            }
        }
    }
}
