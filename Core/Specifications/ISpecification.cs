using System.Linq.Expressions;

namespace Core.Specifications
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }

        Expression<Func<T, object>> OrderBy { get; }

        //QUESTION: This way it is possible to specify both ascending and descending ordering at the same time. Why not specify the direction of the ordering instead? 
        Expression<Func<T, object>> OrderByDescending { get; }
    }
}