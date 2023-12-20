using Azure;
using FlexiForms.Data.Tables;
using FlexiForms.Messages.Views;
using NPoco;

namespace FlexiForms.Extensions
{
    public static class ViewExtensions
    {
        public static IEnumerable<TResult> MapToView<TSource, TResult>(this ICollection<TSource> source)
            where TSource : IFlexiFormsSchema
            where TResult : IFlexiFormsView<TSource, TResult>
        {
            return source.Select(x => TResult.CreateView(x));
        }
    }
}
