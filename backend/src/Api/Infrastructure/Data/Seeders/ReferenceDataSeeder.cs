using System.Text.Json;
using Domain.Entities.Reference;
using Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Seeders;

public static class ReferenceDataSeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static async Task SeedReferenceDataAsync(this ReferenceDbContext context, string seedDataPath)
    {
        if (!Directory.Exists(seedDataPath))
            return;

        await SeedEntitiesAsync<Nationality>(context, seedDataPath, "nationalities.json", context.Nationalities);
        await SeedEntitiesAsync<InsuranceOption>(context, seedDataPath, "insurance-options.json", context.InsuranceOptions);
        await SeedEntitiesAsync<Classification>(context, seedDataPath, "classifications.json", context.Classifications);
        await SeedEntitiesAsync<AvailabilityStatus>(context, seedDataPath, "availability-statuses.json", context.AvailabilityStatuses);
        await SeedEntitiesAsync<Coordinator>(context, seedDataPath, "coordinators.json", context.Coordinators);
        await SeedEntitiesAsync<WorkingHour>(context, seedDataPath, "working-hours.json", context.WorkingHours);
        await SeedEntitiesAsync<WorkingDay>(context, seedDataPath, "working-days.json", context.WorkingDays);
        await SeedEntitiesAsync<AgeGroup>(context, seedDataPath, "age-groups.json", context.AgeGroups);
        await SeedEntitiesAsync<ClinicMechanism>(context, seedDataPath, "clinic-mechanisms.json", context.ClinicMechanisms);
        await SeedEntitiesAsync<ServiceCatalog>(context, seedDataPath, "services.json", context.Services);
    }

    private static async Task SeedEntitiesAsync<TEntity>(
        DbContext context,
        string seedDataPath,
        string fileName,
        DbSet<TEntity> dbSet) where TEntity : class, new()
    {
        var filePath = Path.Combine(seedDataPath, fileName);
        if (!File.Exists(filePath))
            return;

        var json = await File.ReadAllTextAsync(filePath);
        var items = JsonSerializer.Deserialize<List<SeedItem>>(json, JsonOptions);
        if (items == null || items.Count == 0)
            return;

        var existingNames = new HashSet<string>(
            await dbSet.Select(e => EF.Property<string>(e, "Name")).ToListAsync(),
            StringComparer.Ordinal);

        foreach (var item in items)
        {
            if (string.IsNullOrWhiteSpace(item.Name) || existingNames.Contains(item.Name))
                continue;

            var entity = new TEntity();
            typeof(TEntity).GetProperty("Name")!.SetValue(entity, item.Name);
            typeof(TEntity).GetProperty("Id")!.SetValue(entity, Guid.NewGuid());
            typeof(TEntity).GetProperty("IsActive")!.SetValue(entity, true);
            typeof(TEntity).GetProperty("CreatedAt")!.SetValue(entity, DateTime.UtcNow);

            await dbSet.AddAsync(entity);
            existingNames.Add(item.Name);
        }

        await context.SaveChangesAsync();
    }

    private sealed class SeedItem
    {
        public string Name { get; set; } = string.Empty;
    }
}
