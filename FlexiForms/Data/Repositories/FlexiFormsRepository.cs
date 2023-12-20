using FlexiForms.Data.Tables;
using NPoco;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Scoping;
using System.Linq;
using System.Linq.Expressions;

namespace FlexiForms.Data.Repositories
{
    public class FlexiFormsRepository<T> : IFlexiFormsRepository<T>
        where T : IFlexiFormsSchema
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly string _dbName;
        private readonly string _primaryKey;

        public FlexiFormsRepository(IScopeProvider scopeProvider, string dbName, string primaryKey = "Id")
        {
            _scopeProvider = scopeProvider;
            _dbName = dbName;
            _primaryKey = primaryKey;
        }

        public virtual async Task<T> Create(T record)
        {
            using var scope = _scopeProvider.CreateScope();
            await scope.Database.InsertAsync(record);
            scope.Complete();

            return record;
        }

        public virtual async Task<ICollection<T>> GetAll()
        {
            using var scope = _scopeProvider.CreateScope();

            Sql<ISqlContext>? sql1 = scope.SqlContext?.Sql()?
                .SelectAll()
                .From<T>();

            var queryResults = await scope.Database.FetchAsync<T>(sql1);
            scope.Complete();

            return queryResults;
        }

        public virtual async Task<IEnumerable<T>?> GetBy(Expression<Func<T, bool>> predicate)
        {
            using var scope = _scopeProvider.CreateScope();
            Sql<ISqlContext>? sql1 = scope.SqlContext?.Sql()?
                .Where(predicate)
                .From<T>();

            var queryResults = await scope.Database.FetchAsync<T>(sql1);
            scope.Complete();

            return queryResults;
        }

        public virtual async Task<T?> GetSingle(int recordId)
        {
            using var scope = _scopeProvider.CreateScope();
            var queryResult = await scope.Database.SingleByIdAsync<T>(recordId);
            scope.Complete();

            if (queryResult == null)
            {
                return default;
            }

            return queryResult;
        }

        public virtual async Task Update(T record)
        {
            using var scope = _scopeProvider.CreateScope();
            var queryResults = await scope.Database.UpdateAsync(record);
            scope.Complete();
        }

        public virtual async Task Delete(T record)
        {
            using var scope = _scopeProvider.CreateScope();
            var queryResults = await scope.Database.DeleteAsync(record);
            scope.Complete();
        }
    }
}
