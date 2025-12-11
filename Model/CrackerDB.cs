using Npgsql;
using System;
using System.Data.Entity;

namespace Cracker
{
  public class CrackerDB : DbContext
  {
    public CrackerDB() : base("crConnStr")
    {
    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      modelBuilder.HasDefaultSchema("public");

      modelBuilder.Entity<User>()
        .HasMany(s => s.tasksD)
        .WithRequired(a => a.developer)
        .HasForeignKey(a => a.developer_id)
        .WillCascadeOnDelete(false);

      modelBuilder.Entity<User>()
        .HasMany(s => s.tasksM)
        .WithRequired(a => a.manager)
        .HasForeignKey(a => a.manager_id)
        .WillCascadeOnDelete(false);

      modelBuilder.Entity<Sprint>()
        .HasMany(s => s.tasks)
        .WithRequired(a => a.sprint)
        .HasForeignKey(a => a.sprint_id)
        .WillCascadeOnDelete(true);

      modelBuilder.Entity<Task>()
        .HasMany(t => t.notes)
        .WithRequired(a => a.task)
        .HasForeignKey(a => a.task_id)
        .WillCascadeOnDelete(true);

      modelBuilder.Entity<Task>()
        .HasMany(t => t.logs)
        .WithRequired(a => a.task)
        .HasForeignKey(a => a.task_id)
        .WillCascadeOnDelete(true);

      modelBuilder.Entity<Task>()
        .HasMany(t => t.files)
        .WithRequired(a => a.task)
        .HasForeignKey(a => a.task_id)
        .WillCascadeOnDelete(true);

      modelBuilder.Entity<Project>()
        .HasMany(p => p.tasks)
        .WithRequired(a => a.project)
        .HasForeignKey(a => a.project_id)
        .WillCascadeOnDelete(true);

      modelBuilder.Entity<User>()
        .HasMany(u => u.notes)
        .WithRequired(a => a.user)
        .HasForeignKey(a => a.user_id)
        .WillCascadeOnDelete(true);

      modelBuilder.Entity<User>()
        .HasMany(u => u.logs)
        .WithRequired(a => a.user)
        .HasForeignKey(a => a.user_id)
        .WillCascadeOnDelete(true);
    }

    public DbSet<User> users { get; set; }
    public DbSet<Project> projects { get; set; }
    public DbSet<Note> notes { get; set; }
    public DbSet<Task> tasks { get; set; }
    public DbSet<Log> logs { get; set; }
    public DbSet<Sprint> sprints { get; set; }
    public DbSet<File> files { get; set; }
  }
}