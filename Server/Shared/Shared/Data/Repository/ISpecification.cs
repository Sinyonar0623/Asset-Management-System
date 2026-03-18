using System;
using System.Linq.Expressions;

namespace Shared.Data;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> ToExpression();
}
