using FlexiForms.Data.Tables;
using FlexiForms.Messages.Views;

namespace FlexiForms.Data.Repositories
{
    public interface IFlexiFormsResponsesRepository : IFlexiFormsRepository<FlexiFormResponsesSchema>
    {
        Task<ICollection<FormResponseCount>> GetAliases();
        Task<ICollection<FlexiFormResponsesSchema>> GetResponsesByFormAlias(string formAlias);
        Task<int> DeleteFormById(int responseId);
        Task<int> DeleteAllByIdentifier(string identifier);
    }
}
