using FlexiForms.Data.Repositories;
using Umbraco.Cms.Core.Composing;

namespace FlexiForms.Data
{
    public class DI : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IFlexiFormsResponsesRepository, FlexiFormsResponsesRepository>();
        }
    }
}