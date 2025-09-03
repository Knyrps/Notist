namespace Notist.Core.Adapters.Exceptions
{
    public class AdapterNotInitializedException : Exception
	{
		private const string _message = "Adapter not initialized.";

		public Type? AdapterType { get; set; }

		public AdapterNotInitializedException(Type? adapterType = null, Exception ? innerException = null) : base(
			message: adapterType != null ? $"[{adapterType.Name}] {_message}" : _message, innerException)
		{
			this.AdapterType = adapterType;
		}
    }
}
