namespace Notist.Connectors
{
    public class ConnectorInvocationResult
    {
        public bool Success { get; set; }

		public string? Error { get; set; }
		
		public string? Data { get; set; }  // For JSON payloads
    }
}
