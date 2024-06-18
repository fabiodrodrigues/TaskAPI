using Microsoft.Extensions.DependencyInjection;
using TaskAPI.Data.Repositories;
using TaskAPI.Domain.Contracts.Repositories;

namespace TaskAPI.Data
{
    public static class DataModuleDependency
    {
        public static void AddDataModule(this IServiceCollection services)
        {
            services.AddTransient<ITaskRepository, TaskRepository>();
            services.AddTransient<IConexaoBanco, ConexaoBanco>();
        }
    }
}
