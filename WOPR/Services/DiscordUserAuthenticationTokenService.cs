using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace WOPR.Services
{
	public class DiscordUserAuthenticationTokenService
	{
		private static List<DiscordUserAuthenticationToken> ActiveTokens;

		public DiscordUserAuthenticationTokenService()
		{
			ActiveTokens = new List<DiscordUserAuthenticationToken>();
		}

		public static void RemoveToken(DiscordUserAuthenticationToken token)
		{
			ActiveTokens.Remove(token);
			token.CloseTimer();
		}

		public static Guid GenerateTokenForDiscordUser(string discordUserId)
		{
			var activeTokens = ActiveTokens.Where(at => at.DiscordUserId == discordUserId);

			foreach(var t in activeTokens)
			{
				RemoveToken(t);
			}

			var newDiscordAuthenticationToken = new DiscordUserAuthenticationToken() 
			{
				Key = Guid.NewGuid(),
				DiscordUserId = discordUserId
			};

			ActiveTokens.Add(newDiscordAuthenticationToken);

			return newDiscordAuthenticationToken.Key;
		}

		public static bool CheckToken(string discordUserId, string authCode)
		{
			var storedCode = ActiveTokens.SingleOrDefault(du => du.DiscordUserId == discordUserId);

			if(storedCode == null)
			{
				return false;
			}

			if (storedCode.Key.ToString().Equals(authCode))
			{
				RemoveToken(storedCode);
				return true;				
			}

			RemoveToken(storedCode);

			return false;
		}
	}

	public class DiscordUserAuthenticationToken
	{
		public Guid Key { get; set; }
		public string DiscordUserId { get; set; }

		private Timer Timer;

		public DiscordUserAuthenticationToken()
		{
			Timer = new Timer(20000);
			Timer.Elapsed += TokenTimeOutEvent;
			Timer.AutoReset = false;
			Timer.Enabled = true;
		}

		public void CloseTimer()
		{
			Timer.Close();
		}

		public void TokenTimeOutEvent(Object source, ElapsedEventArgs e)
		{
			DiscordUserAuthenticationTokenService.RemoveToken(this);
		}
	}
}
