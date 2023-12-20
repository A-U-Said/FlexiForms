using FlexiForms.Data.Tables;

namespace FlexiForms.Messages.Views
{
    public interface IFlexiFormsView<TSchema, TView>
        where TSchema : IFlexiFormsSchema
    {
        static abstract TView CreateView(TSchema schema);
    }
}
