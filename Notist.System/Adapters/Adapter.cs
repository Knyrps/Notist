using Notist.Core.Adapters.Exceptions;
using Notist.Core.Models.NoteManagement;

namespace Notist.Core.Adapters
{
	public abstract class Adapter<TConfig>
	{
		protected bool _initialized = false;

		public abstract void Initialize(TConfig config);

		public abstract void CreateNote(NoteCreationOptions options);

		protected virtual void EnsureInitialized()
		{
			if (!this._initialized)
			{
				throw new AdapterNotInitializedException(adapterType: this.GetType());
			}
		}
    }
}
