using BabelBot.CustomPreconditions;
using BabelBot.Services;
using BabelDatabase;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelBot.Modules
{
	public class TimeModule : ModuleBase<SocketCommandContext>
	{
		private readonly BabelContext _context;
		private readonly DeathService _deathService;

		public TimeModule(BabelContext context)
		{
			_context = context;
		}

		[Command("year")]
		public async Task GetCurrentYear()
		{
			var year = _context.GameState.SingleOrDefault();

			await ReplyAsync($"The current year is {year.CurrentYear}.");
		}

		[Command("advance")]
		[RequiresAdmin]
		public async Task AdvanceYear(int amount)
		{
			if (amount <= 0)
			{
				await ReplyAsync("YOU FOOL, YOU ABSOLUTE BUFFOON");
			}

			var gameState = _context.GameState.FirstOrDefault();

			var charactersOver60AndAlive = _context.Characters.AsQueryable().Where(c => c.YearOfDeath == 0
				&& c.YearOfBirth < gameState.CurrentYear - 60);

			foreach (var character in charactersOver60AndAlive)
			{
				await _deathService.PerformOldAgeCalculation(character, gameState.CurrentYear, gameState.CurrentYear + amount - 1);
			}

			gameState.CurrentYear += amount;

			_context.GameState.Update(gameState);
			await _context.SaveChangesAsync();

			await ReplyAsync($"The is now {gameState.CurrentYear}");
		}
	}
}
