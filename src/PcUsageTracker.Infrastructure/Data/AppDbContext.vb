Imports Microsoft.EntityFrameworkCore
Imports PcUsageTracker.Core.Models

Namespace Data

    Public Class AppDbContext
        Inherits DbContext

        Public Property UsageSessions As DbSet(Of UsageSession)
        Public Property DailySummaries As DbSet(Of DailyUsage)

        Public Sub New(options As DbContextOptions(Of AppDbContext))
            MyBase.New(options)
        End Sub

        Protected Overrides Sub OnModelCreating(modelBuilder As ModelBuilder)
            MyBase.OnModelCreating(modelBuilder)

            modelBuilder.Entity(Of UsageSession)(Sub(entity)
                entity.HasKey(Function(e) e.Id)
                entity.Property(Function(e) e.StartTime).IsRequired()
                entity.Property(Function(e) e.Status).HasConversion(Of Integer)()
            End Sub)

            modelBuilder.Entity(Of DailyUsage)(Sub(entity)
                entity.HasKey(Function(e) e.Id)
                entity.HasIndex(Function(e) e.Date).IsUnique()
            End Sub)
        End Sub
    End Class

End Namespace