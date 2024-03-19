using Mapster;
using System.Reflection;

namespace Midas.Configuration
{
    public static class MapsterConfiguration
    {
        public static void AddMapster(this IServiceCollection services)
        {
            var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
            Assembly applicationAssembly = typeof(Program).Assembly;
            typeAdapterConfig.Scan(applicationAssembly);
        }
    }
}
