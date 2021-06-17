using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SwaggerOData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SwaggerOData.DbContexts
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<Employee> Employees { get; set; }
		public DbSet<Book> Books { get; set; }

		public override int SaveChanges()
		{
			UpdateEntitiesInfo();
			return base.SaveChanges();
		}

		public async Task<int> SaveChangesAsync()
		{
			UpdateEntitiesInfo();
			return await base.SaveChangesAsync();
		}

		private void UpdateEntitiesInfo()
		{
			var entries = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));
			foreach (var entry in entries)
			{
				if (entry.State == EntityState.Added)
				{
					((BaseEntity)entry.Entity).Created = DateTime.UtcNow;
				}
				((BaseEntity)entry.Entity).Modified = DateTime.UtcNow;
			}
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.Entity<Book>(o =>
			{
				o.HasKey(b => b.Id);
			});
			builder.Entity<Employee>(o =>
			{
				o.HasKey(x => x.Id);
				o.HasMany(x => x.Books).WithOne(b => b.Author).HasForeignKey(b => b.AuthorId);
				o.Property(x => x.Properties).HasConversion(
					v => JsonConvert.SerializeObject(v),
					v => JsonConvert.DeserializeObject<Dictionary<string, object>>(v, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects }));
			});
		}
	}
}