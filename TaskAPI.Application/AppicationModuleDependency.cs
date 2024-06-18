using Microsoft.Extensions.DependencyInjection;
using TaskAPI.Application.Services;
using TaskAPI.Domain.Contracts.Services;

namespace TaskAPI.Application
{
    public static class AppicationModuleDependency
    {
        public static void AddApplicationModule(this IServiceCollection services)
        {
            services.AddTransient<ITaskService, TaskService>();
        }
    }
}
