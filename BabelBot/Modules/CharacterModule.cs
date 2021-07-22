using BabelBot.CustomPreconditions;
using BabelDatabase;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelBot.Modules
{
	public class CharacterModule : ModuleBase<SocketCommandContext>
	{
		private IConfiguration Configuration;

		public CharacterModule(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		[Command("cliques")]
		[RequireProfile]
		[RequireLivingActiveCharacter]
		public async Task MyCliques()
		{
			using var db = new BabelContext(Configuration);
			var profile = db.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == Context.User.Id.ToString());

			var embedBuilder = new EmbedBuilder();

			embedBuilder.Title = "Cliques";

			var activeCharacter = db.Characters.SingleOrDefault(c => c.CharacterId == profile.ActiveCharacterId);

			foreach (var clique in activeCharacter.Cliques)
			{
				var field = new EmbedFieldBuilder();
				field.Name = clique.Clique.CliqueName;

				field.Value += "Members:\n";

				foreach (var character in clique.Clique.CliqueMemberCharacter)
				{
					field.Value += character.Member.CharacterName + "\n";
				}

				field.Value += "\nOfficers:\n";

				foreach (var officer in clique.Clique.CliqueOfficerCharacter)
				{
					field.Value += officer.Officer.CharacterName + "\n";
				}

				embedBuilder.AddField(field);
			}

			await Context.User.SendMessageAsync(embed: embedBuilder.Build());
		}

		[Command("me")]
		[RequireProfile]
		public async Task ViewCurrentCharacter([Remainder] SocketGuildUser mention = null)
		{
			using var db = new BabelContext(Configuration);
			DiscordUser profile = null;

			if (mention != null)
			{
				profile = db.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == mention.Id.ToString());
			}
			else
			{
				profile = db.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == Context.User.Id.ToString());
			}

			if (profile.ActiveCharacterId == null)
			{
				await ReplyAsync("This user has no active character.");
				return;
			}

			var character = db.Characters.SingleOrDefault(c => c.CharacterId == profile.ActiveCharacterId);			

			var embedBuilder = new EmbedBuilder();

			embedBuilder.Title = $"{character.CharacterName} {character.YearOfBirth}-Present";
			var speciesField = new EmbedFieldBuilder();
			speciesField.Name = "Species";
			speciesField.Value = character.Species.SpeciesName;

			embedBuilder.Description = $"Bio:\n{character.CharacterBio}";

			embedBuilder.AddField(speciesField);

			await ReplyAsync(embed: embedBuilder.Build());
		}

		[Command("history")]
		[RequireProfile]
		public async Task ViewCharacters([Remainder] SocketGuildUser mention = null)
		{
			using var db = new BabelContext(Configuration);
			DiscordUser profile = null;

			if (mention != null)
			{
				profile = db.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == mention.Id.ToString());
			}
			else
			{
				profile = db.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == Context.User.Id.ToString());
			}

			if (profile.Characters.Count() == 0)
			{
				await ReplyAsync("User has no characters.");
				return;
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

		[Command("create character")]
		[RequireProfile]
		public async Task CreateCharacter(string name)
		{
			using var db = new BabelContext(Configuration);
			var userId = Context.Message.Author.Id.ToString();

			var hasActiveCharacter = db.Characters.AsQueryable().Where(c => c.DiscordUserId == userId && c.YearOfDeath == 0).ToList();

			if (hasActiveCharacter.Count > 0)
			{
				await ReplyAsync("You still have a living character, can't create a new one.");
				return;
			}

			if (name.Length < 3)
			{
				await ReplyAsync("Name must be at least 3 characters long.");
				return;
			}

			if (name.Length > 64)
			{
				await ReplyAsync("Name has a maximum length of 64 characters.");
				return;
			}

			var currentYear = db.GameState.First().CurrentYear;

			var rand = new Random();

			var age = rand.Next(18, 26);

			var yearOfBirth = currentYear - age;

			var human = db.Species.AsQueryable().First(s => s.SpeciesName == "Human");

			var newCharacter = new BabelDatabase.Character()
			{
				CharacterName = name,
				DiscordUserId = userId,
				SpeciesId = human.SpeciesId,
				CharacterBio = "",
				YearOfBirth = yearOfBirth
			};

			var discordUser = db.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

			discordUser.ActiveCharacterId = newCharacter.CharacterId;
			db.DiscordUsers.Update(discordUser);
			db.Characters.Add(newCharacter);
			db.SaveChanges();

			await ReplyAsync("Character created.");
		}

		[Command("bio")]
		[RequireLivingActiveCharacter]
		public async Task SetBio([Remainder] string bio)
		{
			using var db = new BabelContext(Configuration);
			if (bio.Length > 2000)
			{
				await ReplyAsync("Bio has a maximum length of 2000 characters.");
				return;
			}

			var activeCharacter =
				db.Characters
					.SingleOrDefault(c => c.DiscordUserId == Context.User.Id.ToString() && c.YearOfDeath == 0);

			activeCharacter.CharacterBio = bio;

			db.Characters.Update(activeCharacter);
			await db.SaveChangesAsync();

			await ReplyAsync("Bio set.");
		}

		[Command("species")]
		public async Task ListSpecies()
		{
			using var db = new BabelContext(Configuration);
			var species = db.Species.ToList();

			var embedBuilder = new EmbedBuilder();

			foreach(var s in species)
			{
				var field = new EmbedFieldBuilder();
				field.Name = s.SpeciesName;
				field.Value = s.SpeciesDescription;
				embedBuilder.AddField(field);
			}

			await ReplyAsync(embed: embedBuilder.Build());
		}
	}
}
