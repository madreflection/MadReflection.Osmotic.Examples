using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Configuration;

namespace DBVariantDemo.Data
{
	public class DBVariantDemoDataContext : DbContext
	{
		public DBVariantDemoDataContext()
		{
		}


		public DbSet<Variant> Variants { get; set; }


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("Server=(local); Database=DBVariantDemo; Integrated Security=SSPI; MultipleActiveResultSets=True");
			optionsBuilder.EnableSensitiveDataLogging();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			EntityTypeBuilder<Variant> variantsEntityBuilder = modelBuilder.Entity<Variant>();
			variantsEntityBuilder.ToTable("Variants");
			variantsEntityBuilder.HasKey(e => e.Name);
			variantsEntityBuilder.Property(e => e.Name).IsRequired().IsUnicode().HasMaxLength(200);
			variantsEntityBuilder.Property(e => e.TypeId).IsRequired();
			variantsEntityBuilder.Property(e => e.Value).IsRequired(false).IsUnicode();
		}
	}
}
