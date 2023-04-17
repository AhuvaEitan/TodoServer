using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TodoApi;

public partial class ToDoDbContext : DbContext
{
    
    protected readonly IConfiguration Configuration;

    public ToDoDbContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    public ToDoDbContext()
    {
       
    }
     

    public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
        : base(options)
    {

    }

    public virtual DbSet<Item> Items { get; set; }

      protected override void OnConfiguring(DbContextOptionsBuilder options)
      {
        
        var connectionString = Configuration.GetConnectionString("ToDoDB");
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
      }
        
            protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("items");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsComplete).HasColumnName("isComplete");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    internal Item Find(string name)
    {
        throw new NotImplementedException();
    }
}
