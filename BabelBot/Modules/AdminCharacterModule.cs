using BabelBot.CustomPreconditions;
using BabelBot.Services;
using BabelDatabase;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelBot.Modules
{
	public class AdminCharacterModule : ModuleBase<SocketCommandContext>
	{
		private readonly DeathService _deathService;

		private IConfiguration Configuration;

		public AdminCharacterModule(DeathService deathService, IConfiguration configuration)
		{
			_deathService = deathService;
			Configuration = configuration;
		}

		[Command("change species")]
		[RequiresAdmin]
		public async Task ChangeCharacterSpecies(string speciesName, [Remainder] SocketGuildUser mention)
		{
			using var db = new BabelContext(Configuration);
			var pingedUser = db.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == mention.Id.ToString());

			if (pingedUser == null)
			{
				await ReplyAsync("User does not have a profile.");
			}

			if (pingedUser.ActiveCharacterId == null)
			{
				await ReplyAsync("Could not find a living character.");
				return;
			}

			var activeCharacter = db.Characters.SingleOrDefault(c => c.CharacterId == pingedUser.ActiveCharacterId);

			var oldSpecies = activeCharacter.Species.SpeciesName;

			var newSpecies = db.Species.SingleOrDefault(s => s.SpeciesName.ToLower() == speciesName.ToLower());

			if (newSpecies == null)
			{
				await ReplyAsync($"Could not find species {speciesName}.");
				return;
			}

			activeCharacter.Species = newSpecies;
			activeCharacter.SpeciesId = newSpecies.SpeciesId;

			db.Characters.Update(activeCharacter);
			await db.SaveChangesAsync();

			await ReplyAsync($"Changed species of {activeCharacter.CharacterName} from {oldSpecies} to {newSpecies.SpeciesName}.");
		}

		[Command("change name")]
		[RequiresAdmin]
		public async Task ChangeCharacterName(string newName, [Remainder] SocketGuildUser mention)
		{
			using var db = new BabelContext(Configuration);
			var pingedUser = db.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == mention.Id.ToString());

			if (pingedUser == null)
			{
				await ReplyAsync("User does not have a profile.");
			}

			if (pingedUser.ActiveCharacterId == null)
			{
				await ReplyAsync("Could not find a living character.");
				return;
			}

			var activeCharacter = db.Characters.SingleOrDefault(c => c.CharacterId == pingedUser.ActiveCharacterId);

			var oldName = activeCharacter.CharacterName;

			activeCharacter.CharacterName = newName;

			db.Characters.Update(activeCharacter);
			await db.SaveChangesAsync();

			await ReplyAsync($"Changed name from {oldName} to {newName}.");
		}

		[Command("admin history")]
		[RequiresAdmin]
		public async Task ShowCharacterListWithId([Remainder] SocketGuildUser mention)
		{
			using var db = new BabelContext(Configuration);
			var discordUser = db.DiscordUsers
				.Include(du => du.Characters)
				.SingleOrDefault(du => du.DiscordUserId == mention.Id.ToString());

			if (discordUser == null)
			{
				await ReplyAsync("User does not have a profile.");
				return;
			}

			if (discordUser.Characters.Count == 0)
			{
				await ReplyAsync("User has no characters.");
				return;
			}

			var embedBuilder = new EmbedBuilder().WithColor(Color.Purple);

			var stringBuilder = new StringBuilder();

			foreach (var character in discordUser.Characters)
			{
				stringBuilder.AppendLine(
					$"{character.CharacterId} - {character.CharacterName} - Is alive: {!character.IsDead()}");
			}

			embedBuilder.AddField("Characters", stringBuilder.ToString());

			await ReplyAsync(embed: embedBuilder.Build());
		}

		[Command("delete character")]
		[RequiresAdmin]
		public async Task DeleteCharacter(string id)
		{
			using var db = new BabelContext(Configuration);
			var character = db.Characters.SingleOrDefault(c => c.CharacterId == id);

			if (character == null)
			{
				await ReplyAsync("Could not find character.");
				return;
			}

			if (character.DiscordUser.ActiveCharacterId == character.CharacterId)
			{
				character.DiscordUser.ActiveCharacterId = null;
			}

			db.Characters.Remove(character);
			await db.SaveChangesAsync();

			await ReplyAsync("Character has been deleted.");
		}

		[Command("raise dead")]
		[RequiresAdmin]
		public async Task RemoveGraveyard()
		{
			using var db = new BabelContext(Configuration);
			var existingYard = db.Graveyards.FirstOrDefault(gy => gy.ChannelId == Context.Channel.Id.ToString());

			if (existingYard == null)
			{
				await ReplyAsync("There's no graveyard in this channel.");
				return;
			}

			db.Graveyards.Remove(existingYard);
			await db.SaveChangesAsync();

			await ReplyAsync("Graveyard removed.");
		}

		[Command("add graveyard")]
		[RequiresAdmin]
		public async Task AddGraveyard()
		{
			using var db = new BabelContext(Configuration);
			var existingYard = db.Graveyards.FirstOrDefault(gy => gy.ChannelId == Context.Channel.Id.ToString());

			if (existingYard != null)
			{
				await ReplyAsync("This channel is already a graveyard. If you want to remove this graveyard, use 'raise dead'");
				return;
			}

			var newGraveyard = new Graveyard()
			{
				ChannelId = Context.Channel.Id.ToString(),
				ServerId = Context.Guild.Id.ToString()
			};

			await db.Graveyards.AddAsync(newGraveyard);
			await db.SaveChangesAsync();

			await ReplyAsync("Graveyard added.");
		}

		[Command("reset")]
		[RequiresAdmin]
		public async Task Reset([Remainder] SocketGuildUser mention)
		{
			await _deathService.Kill(mention.Id, false, Context.Channel);
			await ReplyAsync($"Press F for {mention}.");
		}

		[Command("kill")]
		[RequiresAdmin]
		public async Task Kill([Remainder] SocketGuildUser mention)
		{
			await _deathService.Kill(mention.Id, true, Context.Channel);
		}

		[Command("set age")]
		[RequiresAdmin]
		public async Task SetAge(int newAge, [Remainder] SocketGuildUser mention)
		{
			using var db = new BabelContext(Configuration);
			if (newAge < 18)
			{
				await ReplyAsync("Can't set age below 18.");
				return;
			}

			if (newAge > 100)
			{
				await ReplyAsync("Can't set age over 100.");
				return;
			}

			var pingedUser = db.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == mention.Id.ToString());

			if (pingedUser == null)
			{
				await ReplyAsync("User does not have a profile.");
			}

			if (pingedUser.ActiveCharacterId == null)
			{
				await ReplyAsync("Could not find a living character.");
				return;
			}

			var activeCharacter = db.Characters.SingleOrDefault(c => c.CharacterId == pingedUser.ActiveCharacterId);

			var year = db.GameState.First();

			var newYearOfBirth = year.CurrentYear - newAge;

			activeCharacter.YearOfBirth = newYearOfBirth;

			db.Characters.Update(activeCharacter);
			await db.SaveChangesAsync();

			await ReplyAsync("Age changed.");
		}

		[Command("revive")]
		[RequiresAdmin]
		public async Task Revive(string characterId)
		{
			using var db = new BabelContext(Configuration);
			var deadCharacter =
				db.Characters.SingleOrDefault(c => c.CharacterId == characterId);

			if (deadCharacter == null)
			{
				await ReplyAsync("Character could not be found.");
				return;
			}
			
			if (!deadCharacter.IsDead())
			{
				await ReplyAsync("Character isn't dead and so cannot be revived.");
				return;
			}

			var hasLivingCharacter = deadCharacter.DiscordUser.Characters.Where(c => c.YearOfDeath == 0);

			if (hasLivingCharacter.Count() != 0)
			{
				await ReplyAsync("User still has a living character.");
				return;
			}

			deadCharacter.YearOfDeath = 0;
			deadCharacter.CauseOfDeath = null;

			deadCharacter.DiscordUser.ActiveCharacterId = deadCharacter.CharacterId;

			db.Characters.Update(deadCharacter);
			db.DiscordUsers.Update(deadCharacter.DiscordUser);
			await db.SaveChangesAsync();

			await ReplyAsync($"{deadCharacter.CharacterName} walks amongst the living once again!");
		}
	}
}
