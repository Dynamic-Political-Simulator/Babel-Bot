using BabelDatabase;
using Discord;
using Discord.WebSocket;
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
		private readonly BabelContext _context;
		private readonly DiscordSocketClient _client;

		public DeathService(BabelContext context, DiscordSocketClient client)
		{
			_context = context;
			_client = client;
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
			var charactersToKill = _context.CharacterDeathTimers.AsQueryable().Where(cdt => cdt.DeathTime < DateTime.UtcNow).ToList();

			foreach (var characterTimer in charactersToKill)
			{
				characterTimer.Character.YearOfDeath = characterTimer.YearOfDeath;

				_context.Characters.Update(characterTimer.Character);
				_context.CharacterDeathTimers.Remove(characterTimer);

				try
				{
					var discordUser = _client.GetUser(ulong.Parse(characterTimer.Character.DiscordUserId));

					await discordUser.SendMessageAsync($"Your 24 hours are up. {characterTimer.Character.CharacterName} is dead.");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Exception occurred whilst trying to inform user that their character has died of old age. Exception: " + ex.Message);
				}

				await SendGraveyardMessage(characterTimer.Character);
			}
			await _context.SaveChangesAsync();
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

		private async Task SendGraveyardMessage(Character activeCharacter)
		{
			var graveYards = _context.Graveyards.ToList();

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
			var user = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == id.ToString());

			var activeCharacter = user.ActiveCharacter;

			if (activeCharacter == null || activeCharacter.IsDead())
			{
				await channel.SendMessageAsync("This user does not have an active character.");
				return;
			}

			var year = _context.GameState.SingleOrDefault();

			activeCharacter.YearOfDeath = year.CurrentYear;
			user.ActiveCharacter = null;
			user.ActiveCharacterId = null;

			if (graveyardMessage)
			{
				await SendGraveyardMessage(activeCharacter);
			}

			_context.Characters.Update(activeCharacter);
			_context.DiscordUsers.Update(user);
			await _context.SaveChangesAsync();
		}
	}
}
