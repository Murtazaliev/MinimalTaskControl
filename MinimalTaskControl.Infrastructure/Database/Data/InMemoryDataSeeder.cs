using MinimalTaskControl.Core.Entities;
using MinimalTaskControl.Core.Enums;
using MinimalTaskControl.Core.Interfaces;

namespace MinimalTaskControl.Infrastructure.Database.Data;

public class InMemoryDataSeeder : IDataSeeder
{
    private readonly MinimalTaskControlDbContext _context;

    public InMemoryDataSeeder(MinimalTaskControlDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        if (!_context.Tasks.Any())
        {
            _context.Tasks.AddRange(
                new TaskInfo("Тестовая задача 1", "Описание 1", "admin", null, TasksPriority.High, null),
                new TaskInfo("Тестовая задача 2", "Описание 2", "user", "developer", TasksPriority.Medium, null)
            );

            await _context.SaveChangesAsync();
        }
    }
}
