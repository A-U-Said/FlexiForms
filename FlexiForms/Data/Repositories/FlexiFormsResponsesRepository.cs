using FlexiForms.Data.Tables;
using FlexiForms.Data.Repositories;
using Umbraco.Cms.Infrastructure.Scoping;
using NPoco;
using Umbraco.Cms.Infrastructure.Persistence;

namespace FlexiForms.Data.Repositories
{
    public class FlexiFormsResponsesRepository : FlexiFormsRepository<FlexiFormResponsesSchema>, IFlexiFormsResponsesRepository
    {
        private readonly IScopeProvider _scopeProvider;
        private const string _dbName = FlexiFormConstants.Database.ResponseTable;

        public FlexiFormsResponsesRepository(IScopeProvider scopeProvider) 
            : base(scopeProvider, _dbName)
        {
            _scopeProvider = scopeProvider;
        }


        public async Task<ICollection<FlexiFormResponsesSchema>> GetAliases()
        {
            using var scope = _scopeProvider.CreateScope();

            Sql<ISqlContext>? sql1 = scope.SqlContext?.Sql()?
                .Select<FlexiFormResponsesSchema>(x => x.FormIdentifier)
                .From<FlexiFormResponsesSchema>()
                .GroupBy<FlexiFormResponsesSchema>(x => x.FormIdentifier);

            var queryResults = await scope.Database.FetchAsync<FlexiFormResponsesSchema>(sql1);
            scope.Complete();

            return queryResults;
        }


        public async Task<ICollection<FlexiFormResponsesSchema>> GetResponsesByFormAlias(string formAlias)
        {
            using var scope = _scopeProvider.CreateScope();

            Sql<ISqlContext>? sql1 = scope.SqlContext?.Sql()?
                .Where<FlexiFormResponsesSchema>(x => x.FormIdentifier == formAlias);

            var queryResults = await scope.Database.FetchAsync<FlexiFormResponsesSchema>(sql1);
            scope.Complete();

            return queryResults;
        }


        public async Task<int> DeleteFormById(int responseId)
        {
            using var scope = _scopeProvider.CreateScope();

            var queryResult = await scope.Database
                .DeleteMany<FlexiFormResponsesSchema>()
                .Where(x => x.Id == responseId)
                .ExecuteAsync();

            scope.Complete();
            return queryResult;
        }


        public async Task<int> DeleteAllByIdentifier(string identifier)
        {
            using var scope = _scopeProvider.CreateScope();

            var queryResult = await scope.Database
                .DeleteMany<FlexiFormResponsesSchema>()
                .Where(x => x.FormIdentifier == identifier)
                .ExecuteAsync();

            scope.Complete();
            return queryResult;
        }

    }
}
