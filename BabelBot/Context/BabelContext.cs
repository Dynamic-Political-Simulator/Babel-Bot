using BabelBot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BabelBot.Context
{
	public class BabelContext : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var configBuilder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

			var config = new BabelConfig();
			configBuilder.Build().GetSection("BabelConfig").Bind(config);

			//optionsBuilder.UseSqlServer("");
			optionsBuilder
			.UseLazyLoadingProxies()
			.UseInMemoryDatabase("test");
		}

		public DbSet<Character> Characters { get; set; }
		public DbSet<DiscordUser> DiscordUsers { get; set; }
		public DbSet<Party> Parties { get; set; }
		public DbSet<Species> Species { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

		}
	}
}
