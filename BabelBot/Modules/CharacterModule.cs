﻿using BabelBot.CustomPreconditions;
using BabelDatabase;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelBot.Modules
{
	public class CharacterModule : ModuleBase<SocketCommandContext>
	{
		private readonly BabelContext _context;

		public CharacterModule(BabelContext context)
		{
			_context = context;
		}

		[Command("me")]
		[RequireProfile]
		public async Task ViewCurrentCharacter([Remainder] SocketGuildUser mention = null)
		{
			DiscordUser profile = null;

			if (mention != null)
			{
				profile = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == mention.Id.ToString());
			}
			else
			{
				profile = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == Context.User.Id.ToString());
			}

			var character = profile.ActiveCharacter;

			if (character == null)
			{
				await ReplyAsync("This user has no active character.");
				return;
			}

			var embedBuilder = new EmbedBuilder();

			embedBuilder.Title = $"{character.CharacterName} {character.YearOfBirth}-Present";
			embedBuilder.Description = $"Bio: {character.CharacterBio}";
		}

		[Command("history")]
		[RequireProfile]
		public async Task ViewCharacters([Remainder] SocketGuildUser mention = null)
		{
			DiscordUser profile = null;

			if (mention != null)
			{
				profile = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == mention.Id.ToString());
			}
			else
			{
				profile = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == Context.User.Id.ToString());
			}		

			var embedBuilder = new EmbedBuilder();

			foreach(var character in profile.Characters)
			{
				var field = new EmbedFieldBuilder();

				if (!character.IsDead())
				{
					field.Name = $"{character.CharacterName} {character.YearOfBirth}-Present";
					field.Value = $"Species: {character.Species.SpeciesName}\n{character.CharacterBio}";
				}
				else
				{
					field.Name = $"{character.CharacterName} {character.YearOfBirth}-{character.YearOfDeath}";
					field.Value = $"Species: {character.Species.SpeciesName}\n{character.CharacterBio}";
				}

				embedBuilder.AddField(field);
			}

			await ReplyAsync(embed: embedBuilder.Build());
		}
	}
}
