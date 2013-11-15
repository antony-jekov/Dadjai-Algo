namespace Dadjai.Common
{
    public class Link
    {
        public Link(Page from, Page to, string linkLabel)
        {
            FromPage = from;
            ToPage = to;
            Label = linkLabel;
        }

        public string Label { get; private set; }

        public Page FromPage { get; private set; }

        public Page ToPage { get; private set; }
    }
}