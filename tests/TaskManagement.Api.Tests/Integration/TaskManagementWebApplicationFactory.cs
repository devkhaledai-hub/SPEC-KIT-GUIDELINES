using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Api.Data;
namespace TaskManagement.Api.Tests.Integration;
public sealed class TaskManagementWebApplicationFactory : WebApplicationFactory<Program>
{ protected override void ConfigureWebHost(IWebHostBuilder b) => b.ConfigureServices(s=>{var d=s.Single(x=>x.ServiceType==typeof(DbContextOptions<TaskManagementDbContext>));s.Remove(d);s.AddDbContext<TaskManagementDbContext>(o=>o.UseInMemoryDatabase(Guid.NewGuid().ToString()));}); }
