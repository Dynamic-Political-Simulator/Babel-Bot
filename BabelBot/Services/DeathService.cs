using BabelDatabase;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelBot.Services
{
	class DeathService
	{
		private readonly BabelContext _context;
		private readonly DiscordSocketClient _client;

		public DeathService(BabelContext context, DiscordSocketClient client)
		{
			_context = context;
			_client = client;
		}

		public async Task PerformOldAgeCalculation(Character character, int startYear, int endYear)
		{
			var alreadyHasTimer = _context.CharacterDeathTimers.SingleOrDefault(cdt =>
					cdt.CharacterId == character.CharacterId);

			if (alreadyHasTimer != null)
			{
				return;
			}

			for (var year = startYear; year < endYear + 1; year++)
			{
				var age = character.GetAge(year);
				var yearsOver60 = age - 60;

				var chanceOfDeath = yearsOver60 * 2;

				if (chanceOfDeath > 10) chanceOfDeath = 10;

				var randomValue = new Random().Next(100);

				if (randomValue < chanceOfDeath)
				{
					var timer = new CharacterDeathTimer()
					{
						CharacterId = character.CharacterId,
						YearOfDeath = year,
						DeathTime = DateTime.UtcNow.AddDays(1)
					};

					try
					{
						var discordUser = _client.GetUser(ulong.Parse(character.DiscordUserId));
						await discordUser.SendMessageAsync(
							$"You feel your life force waning. You are certain you only have 24 hours left, it is best to deal with everything undone now before it's too late.");
					}
					catch (Exception e)
					{
						Console.WriteLine("Exception occurred whilst trying to inform user that their character has 24 hours left. Exception: " + e.Message);
					}

					_context.CharacterDeathTimers.Add(timer);
					await _context.SaveChangesAsync();
					break;
				}
			}
		}
	}
}
