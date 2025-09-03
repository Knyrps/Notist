using Notist.Core.Adapters.Configuration;
using Notist.Core.Helpers;
using Notist.Core.Models.NoteManagement;

namespace Notist.Core.Adapters
{
	public class FileSystemAdapter : Adapter<FileSystemAdapterConfiguration>
	{
		private string _dataDirectory;

		public sealed override void Initialize(FileSystemAdapterConfiguration config)
		{
			if (!Directory.Exists(config.Path))
			{
				throw new DirectoryNotFoundException(
					$"[{nameof(FileSystemAdapter)}] Directory not found: {config.Path}."
				);
			}

			if (!IOHelper.IsDirectoryWritable(config.Path))
			{
				throw new UnauthorizedAccessException(
					$"[{nameof(FileSystemAdapter)}] Missing write permission for directory: {config.Path}."
                );
			}

			this._dataDirectory = config.Path;

			this._initialized = true;
		}

		public sealed override void CreateNote(NoteCreationOptions options)
		{
			EnsureInitialized();
		}
    }
}
