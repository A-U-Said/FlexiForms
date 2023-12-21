namespace FlexiForms.Messages.Views
{
    public class FormResponseCount
    {
        public FormResponseCount(string identifier, int count)
        {
            Identifier = identifier;
            Count = count;
        }

        public string Identifier { get; set; }
        public int Count { get; set; }
    }
}
