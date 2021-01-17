using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BabelBot.Models
{
	public class Year
	{
		[Key]
		public string YearId { get; set; } = Guid.NewGuid().ToString();
		public int CurrentYear { get; set; }
	}
}
