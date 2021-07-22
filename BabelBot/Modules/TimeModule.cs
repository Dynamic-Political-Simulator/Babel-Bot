using BabelBot.CustomPreconditions;
using BabelBot.Services;
using BabelDatabase;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace BabelBot.Modules
{
	public class TimeModule : ModuleBase<SocketCommandContext>
	{
		private readonly DeathService _deathService;
		private readonly DiscordSocketClient _client;

		private IConfiguration Configuration;
		private static Timer timer;

		public TimeModule(BabelContext context, DeathService deathService, DiscordSocketClient client, IConfiguration configuration)
		{
			_deathService = deathService;
			_client = client;
			Configuration = configuration;

			StartDailyAdvanceTimer();
		}

		private void StartDailyAdvanceTimer()
		{
			if (timer == null)
			{
				timer = new Timer(30000);
				timer.Elapsed += TriggerAutoAdvance;
				timer.Enabled = true;
			}
		}

		private async void TriggerAutoAdvance(Object source, ElapsedEventArgs e)
		{
			using var db = new BabelContext(Configuration);
			var timeToTrigger = DateTime.UtcNow.Date.AddHours(22);

			var timeNow = DateTime.UtcNow;

			var today = timeNow.Date;

			var autoAdvance = db.AutoAdvance.SingleOrDefault(aa => aa.AutoAdvanceId == "1");

			if (!autoAdvance.Enabled)
			{
				return;
			}

			var dayOfTheWeek = DateTime.UtcNow.DayOfWeek;

			var exception = autoAdvance.DayExceptions[(int)dayOfTheWeek];

			if (exception == '1')
			{
				return;
			}

			if (autoAdvance.LastDayTriggered < today && timeNow > timeToTrigger)
			{
				autoAdvance.LastDayTriggered = today;

				var gameState = db.GameState.First();

				var charactersOver80AndAlive = db.Characters.AsQueryable().Where(c => c.YearOfDeath == 0
				&& c.YearOfBirth < gameState.CurrentYear - 80);

				foreach (var character in charactersOver80AndAlive)
				{
					await _deathService.PerformOldAgeCalculation(character, gameState.CurrentYear, gameState.CurrentYear + autoAdvance.AmountOfYears - 1);
				}

				gameState.CurrentYear += autoAdvance.AmountOfYears;

				db.GameState.Update(gameState);
				db.AutoAdvance.Update(autoAdvance);
				await db.SaveChangesAsync();

				var channel = await _client.Rest.GetChannelAsync(ulong.Parse(autoAdvance.ChannelId));
				var guildChannel = channel as RestTextChannel;
				await guildChannel.SendMessageAsync($"The year is now {gameState.CurrentYear}");
			}
		}

		[Command("year")]
		public async Task GetCurrentYear()
		{
			using var db = new BabelContext(Configuration);
			var year = db.GameState.SingleOrDefault();

			await ReplyAsync($"The current year is {year.CurrentYear}.");
		}

		[Command("advance")]
		[RequiresAdmin]
		public async Task AdvanceYear(int amount)
		{
			using var db = new BabelContext(Configuration);
			if (amount <= 0)
			{
				await ReplyAsync("YOU FOOL, YOU ABSOLUTE BUFFOON");
			}

			var gameState = db.GameState.FirstOrDefault();

			var charactersOver80AndAlive = db.Characters.AsQueryable().Where(c => c.YearOfDeath == 0
				&& c.YearOfBirth < gameState.CurrentYear - 80);

			foreach (var character in charactersOver80AndAlive)
			{
				await _deathService.PerformOldAgeCalculation(character, gameState.CurrentYear, gameState.CurrentYear + amount - 1);
			}

			gameState.CurrentYear += amount;

			db.GameState.Update(gameState);
			await db.SaveChangesAsync();

			await ReplyAsync($"The year is now {gameState.CurrentYear}");
		}

		[Command("auto advance channel")]
		[RequiresAdmin]
		public async Task SetAutoAdvanceChannel()
		{
			using var db = new BabelContext(Configuration);
			var autoAdvance = db.AutoAdvance.First();

			autoAdvance.ChannelId = Context.Channel.Id.ToString();

			db.AutoAdvance.Update(autoAdvance);
			await db.SaveChangesAsync();

			await ReplyAsync("Auto Advance announcement channel set.");
		}

		[Command("auto advance toggle")]
		[RequiresAdmin]
		public async Task ToggleAutoAdvance()
		{
			using var db = new BabelContext(Configuration);
			var autoAdvance = db.AutoAdvance.First();

			autoAdvance.Enabled = !autoAdvance.Enabled;

			db.AutoAdvance.Update(autoAdvance);
			await db.SaveChangesAsync();

			if (autoAdvance.Enabled)
			{
				await ReplyAsync("Auto advance is now enabled.");
			}
			else
			{
				await ReplyAsync("Auto advance is now disabled.");
			}
		}

		[Command("auto advance amount")]
		[RequiresAdmin]
		public async Task AutoAdvanceAmount(int amount)
		{
			using var db = new BabelContext(Configuration);
			if (amount <= 0)
			{
				await ReplyAsync("Amount must be greater than zero.");
				return;
			}

			var autoAdvance = db.AutoAdvance.First();

			var oldAmount = autoAdvance.AmountOfYears;

			autoAdvance.AmountOfYears = amount;

			db.AutoAdvance.Update(autoAdvance);
			await db.SaveChangesAsync();

			await ReplyAsync($"Changed amount from {oldAmount} to {amount}.");
		}

		[Command("auto advance exceptions")]
		public async Task ListAutoAdvanceExceptions()
		{
			using var db = new BabelContext(Configuration);

			var exceptions = db.AutoAdvance.First().DayExceptions;

			var sb = new StringBuilder();
			sb.Append("Auto Advance Exceptions:\n");

			var count = 0;
			foreach (var day in exceptions)
			{
				if (day == '1')
				{
					sb.Append((DayOfWeek)count + "\n");
				}
				count++;
			}

			await ReplyAsync(sb.ToString());
		}

		[Command("auto advance exception")]
		[RequiresAdmin]
		public async Task AutoAdvanceExceptions([Remainder] string day)
		{
			using var db = new BabelContext(Configuration);
			if (!Enum.IsDefined(typeof(DayOfWeek), day))
			{
				await ReplyAsync("Please specify a valid day.");
				return;
			}

			var autoAdvance = db.AutoAdvance.First();

			var dayOfTheWeek = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), day);
			
			var dayValue = autoAdvance.DayExceptions[(int) dayOfTheWeek];

			if (dayValue == '0')
			{
				StringBuilder sb = new StringBuilder(autoAdvance.DayExceptions);
				sb[(int)dayOfTheWeek] = '1';
				autoAdvance.DayExceptions = sb.ToString();

				db.AutoAdvance.Update(autoAdvance);
				await db.SaveChangesAsync();

				await ReplyAsync($"Exception on {dayOfTheWeek} added.");
			}
			else
			{
				StringBuilder sb = new StringBuilder(autoAdvance.DayExceptions);
				sb[(int)dayOfTheWeek] = '0';
				autoAdvance.DayExceptions = sb.ToString();

				db.AutoAdvance.Update(autoAdvance);
				await db.SaveChangesAsync();

				await ReplyAsync($"Exception on {dayOfTheWeek} removed.");
			}
		}
	}
}
