namespace Notist.Core.DependencyInjection
{
	[AttributeUsage(AttributeTargets.Class)]
	public class InjectAttribute : Attribute
	{
		public Type? InterfaceType { get; }
		public ServiceLifetime Lifetime { get; }

		public InjectAttribute(Type? interfaceType = null,
			ServiceLifetime lifetime = ServiceLifetime.Singleton)
		{
			InterfaceType = interfaceType;
			Lifetime = lifetime;
		}
	}
}
