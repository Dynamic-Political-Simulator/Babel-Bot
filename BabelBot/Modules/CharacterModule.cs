using BabelBot.CustomPreconditions;
using BabelDatabase;
using Discord;
using Discord.Commands;
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

		[Command("create character")]
		[RequireProfile]
		[RequireLivingActiveCharacter]
		public async Task CreateCharacter(string name, string species = "Human")
		{
			if(name.Length < 2 || name.Length > 32)
			{
				await ReplyAsync("Name may be a minimum of 2 characters and a maximum of 32 characters long.");
				return;
			}

			//something about species

			var profile = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == Context.User.Id.ToString());

			var newCharacter = new Character()
			{
				CharacterName = name,
				DiscordUser = profile
			};

			_context.Characters.Add(newCharacter);
			await _context.SaveChangesAsync();

			await ReplyAsync("Character created.");
		}

		[Command("history")]
		[RequireProfile]
		public async Task ViewCharacters()
		{
			var profile = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == Context.User.Id.ToString());

			var embedBuilder = new EmbedBuilder();

			foreach(var character in profile.Characters)
			{
				var field = new EmbedFieldBuilder();

				if (!character.IsDead())
				{
					field.Name = $"{character.CharacterName} {character.YearOfBirth}-Present";
					field.Value = $"Species: {character.Species}\n{character.CharacterBio}";
				}
				else
				{
					field.Name = $"{character.CharacterName} {character.YearOfBirth}-{character.YearOfDeath}";
					field.Value = $"Species: {character.Species}\n{character.CharacterBio}";
				}

				embedBuilder.AddField(field);
			}

			await ReplyAsync(embed: embedBuilder.Build());
		}
	}
}
