using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Api.Domain;
namespace TaskManagement.Api.Data.Configurations;
public sealed class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{ public void Configure(EntityTypeBuilder<TaskItem> b) { b.ToTable("tasks"); b.HasKey(x=>x.Id); b.Property(x=>x.TeamId).IsRequired(); b.Property(x=>x.Title).IsRequired(); b.Property(x=>x.CreatorId).IsRequired(); b.Property(x=>x.AssigneeId).IsRequired(); b.Property(x=>x.Status).HasConversion<string>().IsRequired(); b.Property(x=>x.Version).IsConcurrencyToken(); b.HasIndex(x=>new{x.TeamId,x.Status}); b.HasIndex(x=>new{x.TeamId,x.AssigneeId}); b.HasIndex(x=>new{x.TeamId,x.Status,x.AssigneeId}); } }
