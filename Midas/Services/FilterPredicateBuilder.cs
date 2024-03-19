using Midas.Data.Entities;
using Midas.Extensions;
using System.Linq.Expressions;

namespace Midas.Services
{
    public class FilterPredicateBuilder
    {
        private Expression<Func<Candle, bool>> _filterPredicate = null!;

        public Expression<Func<Candle, bool>> FilterPredicate
        {
            get
            {
                if (_filterPredicate == null)
                {
                    return x => true;
                }

                return _filterPredicate;
            }
        }

        public static implicit operator Expression<Func<Candle, bool>>(FilterPredicateBuilder filterPredicateBuilder) => filterPredicateBuilder.FilterPredicate;

        public FilterPredicateBuilder IncludeSymbol(string? filter)
        {
            if (filter != null)
            {
                Expression<Func<Candle, bool>> predicate = x => x.Symbol != null && x.Symbol == filter;

                _filterPredicate = _filterPredicate.And(predicate);
            }

            return this;
        }

        public FilterPredicateBuilder IncludeTimeInterval(string? filter)
        {
            if (filter != null)
            {
                Expression<Func<Candle, bool>> predicate = x => x.Interval == filter;
                _filterPredicate = _filterPredicate.And(predicate);
            }

            return this;
        }

    }
}
