using Bot.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Bot.Models.Data
{
	public class ApplicationContext:DbContext
	{
		public ApplicationContext()
		{
			Database.EnsureCreated();
        }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("Server=localhost;Database=Bot;User Id=sa;Password=Valuetech@123;");
		}

		public DbSet<Movie> Movies { get; set; } = null!;
	}
}

