using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BabelBot.Models
{
	public class Species
	{
		[Key]
		public string SpeciesId { get; set; } = Guid.NewGuid().ToString();

		public string SpeciesName { get; set; }
	}
}
