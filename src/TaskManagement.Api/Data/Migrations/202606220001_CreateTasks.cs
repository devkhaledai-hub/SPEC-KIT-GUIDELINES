using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
namespace TaskManagement.Api.Data.Migrations;
[DbContext(typeof(TaskManagementDbContext))]
[Migration("202606220001_CreateTasks")]
public partial class CreateTasks : Migration
{
 protected override void Up(MigrationBuilder m)
 {
  m.CreateTable(name:"tasks",columns:t=>new { Id=t.Column<Guid>(type:"uuid",nullable:false),TeamId=t.Column<string>(type:"text",nullable:false),Title=t.Column<string>(type:"text",nullable:false),Description=t.Column<string>(type:"text",nullable:true),DueDate=t.Column<DateOnly>(type:"date",nullable:true),CreatorId=t.Column<string>(type:"text",nullable:false),AssigneeId=t.Column<string>(type:"text",nullable:false),Status=t.Column<string>(type:"text",nullable:false),CreatedAt=t.Column<DateTimeOffset>(type:"timestamp with time zone",nullable:false),UpdatedAt=t.Column<DateTimeOffset>(type:"timestamp with time zone",nullable:false),Version=t.Column<long>(type:"bigint",nullable:false)},constraints:t=>t.PrimaryKey("PK_tasks",x=>x.Id));
  m.CreateIndex(name:"IX_tasks_TeamId_Status",table:"tasks",columns:new[]{"TeamId","Status"}); m.CreateIndex(name:"IX_tasks_TeamId_AssigneeId",table:"tasks",columns:new[]{"TeamId","AssigneeId"}); m.CreateIndex(name:"IX_tasks_TeamId_Status_AssigneeId",table:"tasks",columns:new[]{"TeamId","Status","AssigneeId"});
 }
 protected override void Down(MigrationBuilder m)=>m.DropTable("tasks");
}
