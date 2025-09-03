using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Notist.Models;

namespace Notist.Connectors
{
	[ComVisible(true)]
    public class NotistConnector
    {
		public ConnectorInvocationResult GetContext()
		{
			try
			{
				var context = new ApplicationContext(); // This might need to be populated properly
				return new ConnectorInvocationResult 
				{ 
					Success = true, 
					Data = JsonConvert.SerializeObject(context) 
				};
			}
			catch (Exception ex)
			{
				return new ConnectorInvocationResult 
				{ 
					Success = false, 
					Error = ex.Message 
				};
			}
		}

		public ConnectorInvocationResult CreateEmptyNote()
		{
			return new ConnectorInvocationResult { Success = true };
		}
    }
}
