using FlexiForms.Data.Tables;


namespace FlexiForms.Data.Repositories
{
    public interface IFlexiFormsResponsesRepository : IFlexiFormsRepository<FlexiFormResponsesSchema>
    {
        Task<ICollection<FlexiFormResponsesSchema>> GetAliases();
        Task<ICollection<FlexiFormResponsesSchema>> GetResponsesByFormAlias(string formAlias);
        Task<int> DeleteFormById(int responseId);
        Task<int> DeleteAllByIdentifier(string identifier);
    }
}
