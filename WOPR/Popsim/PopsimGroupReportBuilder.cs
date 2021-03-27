using BabelDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WOPR.Popsim
{
	public class PopsimGroupReportBuilder
	{
		public List<PopsimGlobalEthicGroup> GlobalEthicGroups { get; set; }

		public PopsimGroupReportBuilder(List<PopsimGlobalEthicGroup> globalEthicGroups)
		{
			GlobalEthicGroups = globalEthicGroups;
		}

		public GroupsReport Build()
		{
			var report = new GroupsReport();

			report.FederalismCentralismAverage = GlobalEthicGroups.Sum(g => g.FederalismCentralism) / GlobalEthicGroups.Count;
			report.DemocracyAuthorityAverage = GlobalEthicGroups.Sum(g => g.DemocracyAuthority) / GlobalEthicGroups.Count;
			report.GlobalismIsolationismAverage = GlobalEthicGroups.Sum(g => g.GlobalismIsolationism) / GlobalEthicGroups.Count;
			report.MilitarismPacifismAverage = GlobalEthicGroups.Sum(g => g.MilitarismPacifism) / GlobalEthicGroups.Count;
			report.SecurityFreedomAverage = GlobalEthicGroups.Sum(g => g.SecurityFreedom) / GlobalEthicGroups.Count;
			report.CooperationCompetitionAverage = GlobalEthicGroups.Sum(g => g.CooperationCompetition) / GlobalEthicGroups.Count;
			report.SecularismSpiritualismAverage = GlobalEthicGroups.Sum(g => g.SecularismSpiritualism) / GlobalEthicGroups.Count;
			report.ProgressivismTraditionalismAverage = GlobalEthicGroups.Sum(g => g.ProgressivismTraditionalism) / GlobalEthicGroups.Count;
			report.MonoculturalismMulticulturalismAverage = GlobalEthicGroups.Sum(g => g.MonoculturalismMulticulturalism) / GlobalEthicGroups.Count;

			return report;
		}
	}

	public class GroupsReport
	{
		public int FederalismCentralismAverage { get; set; }
		public int DemocracyAuthorityAverage { get; set; }
		public int GlobalismIsolationismAverage { get; set; }
		public int MilitarismPacifismAverage { get; set; }
		public int SecurityFreedomAverage { get; set; }
		public int CooperationCompetitionAverage { get; set; }
		public int SecularismSpiritualismAverage { get; set; }
		public int ProgressivismTraditionalismAverage { get; set; }
		public int MonoculturalismMulticulturalismAverage { get; set; }
	}
}
