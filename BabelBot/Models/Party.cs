using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BabelBot.Models
{
	public class Party
	{
		[Key]
		public string PartyId { get; set; } = Guid.NewGuid().ToString();

		[Required]
		public string PartyName { get; set; }

		public virtual List<Character> Members { get; set; }
	}
}
