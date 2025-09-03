namespace Notist.Core.DependencyInjection;

/// <summary>
///     Proxy enum for Microsoft.Extensions.DependencyInjection.ServiceLifetime
///     Allows attributes to reference lifetime without extra using
/// </summary>
public enum ServiceLifetime
{
	Singleton = Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton,
	Scoped = Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped,
	Transient = Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient
}
