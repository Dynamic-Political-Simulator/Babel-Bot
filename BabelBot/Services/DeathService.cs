using BabelDatabase;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace BabelBot.Services
{
	public class DeathService
	{
		private readonly DiscordSocketClient _client;

		private IConfiguration Configuration;

		public DeathService(DiscordSocketClient client, IConfiguration configuration)
		{
			_client = client;
			Configuration = configuration;
			StartOldAgeCheckerTimer();
		}

		private void StartOldAgeCheckerTimer()
		{
			var timer = new Timer(30000);
			timer.Elapsed += OldAgeDeathCheck;
			timer.Enabled = true;
		}

		private async void OldAgeDeathCheck(Object source, ElapsedEventArgs e)
		{
			using var db = new BabelContext(Configuration);
			var charactersToKill = db.CharacterDeathTimers.AsQueryable().Where(cdt => cdt.DeathTime < DateTime.UtcNow).ToList();

			foreach (var characterTimer in charactersToKill)
			{
				var character = db.Characters.SingleOrDefault(c => c.CharacterId == characterTimer.CharacterId);
				character.YearOfDeath = characterTimer.YearOfDeath;
				character.DiscordUser.ActiveCharacterId = null;

				db.Characters.Update(character);
				db.CharacterDeathTimers.Remove(characterTimer);

				try
				{
					var discordUser = await _client.Rest.GetUserAsync(ulong.Parse(character.DiscordUserId));

					await discordUser.SendMessageAsync($"Your 24 hours are up. {character.CharacterName} is dead.");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Exception occurred whilst trying to inform user that their character has died of old age. Exception: " + ex.Message);
				}

				await SendGraveyardMessage(characterTimer.Character);
			}
			await db.SaveChangesAsync();
		}

		public async Task PerformOldAgeCalculation(Character character, int startYear, int endYear)
		{
			using var db = new BabelContext(Configuration);
			var alreadyHasTimer = db.CharacterDeathTimers.SingleOrDefault(cdt =>
					cdt.CharacterId == character.CharacterId);

			if (alreadyHasTimer != null)
			{
				return;
			}

			for (var year = startYear; year < endYear + 1; year++)
			{
				var age = character.GetAge(year);
				var yearsOver80 = age - 80;

				var chanceOfDeath = yearsOver80 * 2;

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
						var discordUser = await _client.Rest.GetUserAsync(ulong.Parse(character.DiscordUserId));
						await discordUser.SendMessageAsync(
							$"You feel your life force waning. You are certain you only have 24 hours left, it is best to deal with everything undone now before it's too late.");
					}
					catch (Exception e)
					{
						Console.WriteLine("Exception occurred whilst trying to inform user that their character has 24 hours left. Exception: " + e.Message);
					}

					db.CharacterDeathTimers.Add(timer);
					await db.SaveChangesAsync();
					break;
				}
			}
		}

		private async Task SendGraveyardMessage(Character activeCharacter)
		{
			using var db = new BabelContext(Configuration);
			var graveYards = db.Graveyards.ToList();

			var message = $"{activeCharacter.CharacterName} {activeCharacter.YearOfBirth} - {activeCharacter.YearOfDeath}";

			foreach (var graveyardChannel in graveYards.Select(graveyard => _client.GetChannel(ulong.Parse(graveyard.ChannelId)) as SocketTextChannel))
			{
				try
				{
					await graveyardChannel.SendMessageAsync(message);
				}
				catch (NullReferenceException e)
				{
					Console.WriteLine(e.StackTrace);
					//nothing, I just don't want it to crash the command
				}
			}
		}

		public async Task Kill(ulong id, bool graveyardMessage, ISocketMessageChannel channel)
		{
			using var db = new BabelContext(Configuration);
			var user = db.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == id.ToString());

			if (user.ActiveCharacterId == null)
			{
				await channel.SendMessageAsync("This user does not have an active character.");
				return;
			}

			var activeCharacter = db.Characters.SingleOrDefault(c => c.CharacterId == user.ActiveCharacterId);

			var year = db.GameState.SingleOrDefault();

			activeCharacter.YearOfDeath = year.CurrentYear;
			user.ActiveCharacterId = null;

			if (graveyardMessage)
			{
				await SendGraveyardMessage(activeCharacter);
			}

			db.Characters.Update(activeCharacter);
			db.DiscordUsers.Update(user);
			await db.SaveChangesAsync();

			await channel.SendMessageAsync("Press F.");
		}
	}
}
