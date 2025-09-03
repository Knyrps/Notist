using Notist.Core.Models.Notes.Content;

namespace Notist.Core.Models.Notes
{
    public class Note<TContent> where TContent : ContentBase
    {
        public Guid ID { get; init; }

        public string Title { get; private set; }

        public TContent Content { get; private set; }
    }

    public class TextNote : Note<TextContent> {}
}
