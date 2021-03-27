using BabelDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WOPR.Popsim;

namespace WOPR.Services
{
	public class PopsimService
	{
		private readonly BabelContext _context;

		public PopsimService(BabelContext babelContext)
		{
			_context = babelContext;
		}

		public PopsimReport GenerateReport()
		{
			var report = new PopsimReport();

			return report;
		}

		public GroupsReport GenerateGroupsReport()
		{
			var reportBuilder = new PopsimGroupReportBuilder(_context.PopsimGlobalEthicGroups.ToList());

			return reportBuilder.Build();
		}

		
	}
}
