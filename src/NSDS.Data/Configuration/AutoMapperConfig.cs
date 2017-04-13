using AutoMapper;
using NSDS.Core.Models;
using NSDS.Data.Models;

namespace NSDS.Data.Configuration
{
	static class AutoMapperConfig
    {
		private static bool _initialized = false;

		public static void Configure()
		{
			if (_initialized)
			{
				return;
			}

			_initialized = true;

			Mapper.Initialize(config =>
			{
				config.CreateMap<ClientDataModel, Client>();
				config.CreateMap<PackageDataModel, Package>();
				config.CreateMap<DeploymentDataModel, Deployment>();
				config.CreateMap<PoolDataModel, Pool>();
				config.CreateMap<ModuleDataModel, Module>();
			});
		}
    }
}
