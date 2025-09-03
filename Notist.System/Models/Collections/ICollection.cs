namespace Notist.Core.Models.Collections;

public interface ICollection
{
	#region Public Properties

	Guid ID { get; }

	string Name { get; }

	#endregion

	#region Public Methods

	void Load();

	#endregion
}
