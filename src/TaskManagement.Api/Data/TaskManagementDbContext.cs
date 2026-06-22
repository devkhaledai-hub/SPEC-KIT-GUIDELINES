using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Domain;
namespace TaskManagement.Api.Data;
public sealed class TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options) : DbContext(options)
{ public DbSet<TaskItem> Tasks => Set<TaskItem>(); protected override void OnModelCreating(ModelBuilder b) => b.ApplyConfigurationsFromAssembly(typeof(TaskManagementDbContext).Assembly); }
