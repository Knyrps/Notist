using Notist.Core.Models.Collections;
using Notist.Core.Models.Notes;
using Notist.Core.Models.Notes.Content;

namespace Notist.Core.Models.NoteManagement
{
    public class NoteCreationOptions
    {
        public ICollection Collection { get; set; }

        public Note<ContentBase> Note { get; set; }
	}
}
