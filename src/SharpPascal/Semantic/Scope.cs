using System.Collections.Generic;
using System.Linq;
using SharpPascal.Syntax;

namespace SharpPascal.Semantic
{
    public class Scope
    {
        private readonly Scope? _previous;

        private readonly ISet<Symbol> _symbols = new HashSet<Symbol>();

        public Scope(Scope? previous = null)
        {
            _previous = previous;
        }

        public void Add(Symbol symbol)
            => _symbols.Add(symbol);

        public Symbol? FindLocal(PascalName name)
            => _symbols.FirstOrDefault(s => s.Name == name);

        public Symbol? Find(PascalName name)
            => AllScopes
                .Select(scope => scope.FindLocal(name))
                .SkipWhile(s => s is null)
                .FirstOrDefault();

        private IEnumerable<Scope> AllScopes
        {
            get
            {
                for (var scope = this; scope != null; scope = scope._previous)
                {
                    yield return scope;
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Scope scope)
            {
                if (!scope._symbols.SetEquals(_symbols))
                {
                    return false;
                }

                if (scope._previous is null != _previous is null)
                {
                    return false;
                }

                if (scope._previous is null)
                {
                    return true;
                }

                return scope._previous.Equals(_previous!);
            }
            return false;
        }

        public override int GetHashCode()
            => _symbols.GetHashCode();
    }
}
