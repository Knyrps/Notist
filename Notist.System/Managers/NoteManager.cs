using Notist.Core.DependencyInjection;

namespace Notist.Core.Managers
{
    [Inject(typeof(NoteManager), ServiceLifetime.Singleton)]
    public class NoteManager
    {
		public void CreateNote()
		{

		}
    }
}
