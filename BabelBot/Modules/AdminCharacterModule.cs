using BabelBot.CustomPreconditions;
using BabelBot.Services;
using BabelDatabase;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelBot.Modules
{
	public class AdminCharacterModule : ModuleBase<SocketCommandContext>
	{
		private readonly BabelContext _context;
		private readonly DeathService _deathService;

		public AdminCharacterModule(BabelContext context, DeathService deathService)
		{
			_context = context;
			_deathService = deathService;
		}

		[Command("change species")]
		[RequiresAdmin]
		public async Task ChangeCharacterSpecies(string speciesName, [Remainder] SocketGuildUser mention)
		{
			var pingedUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == mention.Id.ToString());

			if (pingedUser == null)
			{
				await ReplyAsync("User does not have a profile.");
			}

			if (pingedUser.ActiveCharacterId == null)
			{
				await ReplyAsync("Could not find a living character.");
				return;
			}

			var activeCharacter = _context.Characters.SingleOrDefault(c => c.CharacterId == pingedUser.ActiveCharacterId);

			var oldSpecies = activeCharacter.Species.SpeciesName;

			var newSpecies = _context.Species.SingleOrDefault(s => s.SpeciesName.ToLower() == speciesName.ToLower());

			if (newSpecies == null)
			{
				await ReplyAsync($"Could not find species {speciesName}.");
				return;
			}

			activeCharacter.Species = newSpecies;
			activeCharacter.SpeciesId = newSpecies.SpeciesId;

			_context.Characters.Update(activeCharacter);
			await _context.SaveChangesAsync();

			await ReplyAsync($"Changed species of {activeCharacter.CharacterName} from {oldSpecies} to {newSpecies.SpeciesName}.");
		}

		[Command("change name")]
		[RequiresAdmin]
		public async Task ChangeCharacterName(string newName, [Remainder] SocketGuildUser mention)
		{
			var pingedUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == mention.Id.ToString());

			if (pingedUser == null)
			{
				await ReplyAsync("User does not have a profile.");
			}

			if (pingedUser.ActiveCharacterId == null)
			{
				await ReplyAsync("Could not find a living character.");
				return;
			}

			var activeCharacter = _context.Characters.SingleOrDefault(c => c.CharacterId == pingedUser.ActiveCharacterId);

			var oldName = activeCharacter.CharacterName;

			activeCharacter.CharacterName = newName;

			_context.Characters.Update(activeCharacter);
			await _context.SaveChangesAsync();

			await ReplyAsync($"Changed name from {oldName} to {newName}.");
		}

		[Command("admin history")]
		[RequiresAdmin]
		public async Task ShowCharacterListWithId([Remainder] SocketGuildUser mention)
		{
			var discordUser = _context.DiscordUsers
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
			var character = _context.Characters.SingleOrDefault(c => c.CharacterId == id);

			if (character == null)
			{
				await ReplyAsync("Could not find character.");
				return;
			}

			if (character.DiscordUser.ActiveCharacterId == character.CharacterId)
			{
				character.DiscordUser.ActiveCharacterId = null;
			}

			_context.Characters.Remove(character);
			await _context.SaveChangesAsync();

			await ReplyAsync("Character has been deleted.");
		}

		[Command("raise dead")]
		[RequiresAdmin]
		public async Task RemoveGraveyard()
		{
			var existingYard = _context.Graveyards.FirstOrDefault(gy => gy.ChannelId == Context.Channel.Id.ToString());

			if (existingYard == null)
			{
				await ReplyAsync("There's no graveyard in this channel.");
				return;
			}

			_context.Graveyards.Remove(existingYard);
			await _context.SaveChangesAsync();

			await ReplyAsync("Graveyard removed.");
		}

		[Command("add graveyard")]
		[RequiresAdmin]
		public async Task AddGraveyard()
		{
			var existingYard = _context.Graveyards.FirstOrDefault(gy => gy.ChannelId == Context.Channel.Id.ToString());

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

			await _context.Graveyards.AddAsync(newGraveyard);
			await _context.SaveChangesAsync();

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

			var pingedUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == mention.Id.ToString());

			if (pingedUser == null)
			{
				await ReplyAsync("User does not have a profile.");
			}

			if (pingedUser.ActiveCharacterId == null)
			{
				await ReplyAsync("Could not find a living character.");
				return;
			}

			var activeCharacter = _context.Characters.SingleOrDefault(c => c.CharacterId == pingedUser.ActiveCharacterId);

			var year = _context.GameState.First();

			var newYearOfBirth = year.CurrentYear - newAge;

			activeCharacter.YearOfBirth = newYearOfBirth;

			_context.Characters.Update(activeCharacter);
			await _context.SaveChangesAsync();

			await ReplyAsync("Age changed.");
		}

		[Command("revive")]
		[RequiresAdmin]
		public async Task Revive(string characterId)
		{
			var deadCharacter =
				_context.Characters.SingleOrDefault(c => c.CharacterId == characterId);

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

			_context.Characters.Update(deadCharacter);
			_context.DiscordUsers.Update(deadCharacter.DiscordUser);
			await _context.SaveChangesAsync();

			await ReplyAsync("The dead walk once more!");
		}
	}
}
