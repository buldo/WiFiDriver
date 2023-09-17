using Microsoft.Extensions.DependencyInjection;

namespace Rtl8812auNet.Grpc;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRtl8812auNetGrpcServer(this IServiceCollection services)
    {
        return services;
    }
}