using FlexiForms.Data.Tables;
using System.Linq.Expressions;

namespace FlexiForms.Data.Repositories
{
    public interface IFlexiFormsRepository<T> where T : IFlexiFormsSchema
    {
        Task<T> Create(T record);
        Task<ICollection<T>> GetAll();
        Task<T?> GetSingle(int recordId);
        Task<IEnumerable<T>?> GetBy(Expression<Func<T, bool>> predicate);
        Task Update(T record);
        Task Delete(T record);
    }
}
