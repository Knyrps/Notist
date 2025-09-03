using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Notist.Core.DependencyInjection;

public static class DI
{
	#region Other Fields

	private static readonly ServiceCollection _serviceCollection = new();
	private static bool _isInitialized;

	#endregion

	#region Public Properties

	public static IServiceProvider Services { get; private set; } = null!;

	#endregion

	#region Public Methods

	public static void Bind<TInterface, TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
		where TImplementation : class, TInterface
		where TInterface : class
	{
		if (_isInitialized)
		{
			throw new InvalidOperationException("Cannot add services to an already built service collection.");
		}

		ServiceDescriptor descriptor = new(typeof(TInterface), typeof(TImplementation), lifetime);
		_serviceCollection.Add(descriptor);
	}

	public static void Bind<T>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
		where T : class
	{
		if (_isInitialized)
		{
			throw new InvalidOperationException("Cannot add services to an already built service collection.");
		}

		ServiceDescriptor descriptor = new(typeof(T), typeof(T), lifetime);
		_serviceCollection.Add(descriptor);
	}

	public static void Init(bool ignoreAttributes = false)
	{
		if (_isInitialized)
		{
			throw new InvalidOperationException("Cannot add services to an already built service collection.");
		}

		if (!ignoreAttributes)
		{
			IEnumerable<Type?> types = AppDomain.CurrentDomain.GetAssemblies()
												.SelectMany(a =>
													{
														try
														{
															return a.GetTypes();
														} catch (ReflectionTypeLoadException ex)
														{
															return ex.Types.Where(t => t != null)!;
														}
													}
												)
												.Where(t => t != null
															&& t.IsClass
															&& !t.IsAbstract
															&& t.GetCustomAttribute<InjectAttribute>() != null
												);

			foreach (Type? type in types!)
			{
				InjectAttribute attr = type.GetCustomAttribute<InjectAttribute>()!;
				Type serviceType = attr.InterfaceType ?? type;

				_serviceCollection.Add(new ServiceDescriptor(serviceType, type, attr.Lifetime));
			}
		}

		// Build the provider
		Services = _serviceCollection.BuildServiceProvider();
		_isInitialized = true;
	}

	#endregion
}
