using Notist.Core.Adapters;
using Notist.Core.Models.Collections;

public class Collection<TAdapter, TConfig> : ICollection
	where TAdapter : Adapter<TConfig>
{
	public Guid ID { get; init; }
	public string Name { get; set; }
	public TAdapter Adapter { get; set; }
	public TConfig AdapterConfiguration { get; set; }

	public void Load()
	{
		Adapter.Initialize(AdapterConfiguration);
	}
}
