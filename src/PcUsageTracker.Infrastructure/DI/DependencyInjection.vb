Imports System.Runtime.CompilerServices
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.EntityFrameworkCore
Imports PcUsageTracker.Core.Interfaces
Imports PcUsageTracker.Core.Services
Imports PcUsageTracker.Infrastructure.Data
Imports PcUsageTracker.Infrastructure.System

Namespace DI

    Public Module DiExtensions

        <Extension>
        Public Function AddInfrastructure(services As IServiceCollection, dbPath As String) As IServiceCollection
            services.AddDbContext(Of AppDbContext)(Sub(options)
                options.UseSqlite($"Data Source={dbPath}")
            End Sub)

            services.AddScoped(Of IUsageRepository, UsageRepository)()
            services.AddSingleton(Of ISystemMonitor, SystemMonitor)()
            services.AddSingleton(Of IdleDetectionService)()
            services.AddSingleton(Of UsageCalculator)()

            Return services
        End Function

    End Module

End Namespace