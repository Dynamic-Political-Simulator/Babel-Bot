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

		public PopsimReport GetExistingReport(string guid)
		{
			var report = _context.PopsimReports.SingleOrDefault(pr => pr.PopsimReportId == guid);

			if (report == null)
			{
				return null;
			}

			return report;
		}

		/// <summary>
		/// A redacted popsim report for public viewing
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		public PopsimReport GetPublicVersion(string guid)
		{
			var report = GetExistingReport(guid);

			// TODO: do some fancy public redactions tuff

			return report;
		}

		/// <summary>
		/// Gets the most current version of the public popsim report
		/// </summary>
		/// <returns></returns>
		public PopsimReport GetCurrentPublicVersion()
		{
			var report = _context.PopsimReports.FirstOrDefault(pr => pr.IsCurrent == true);

			return report;
		}

		public GroupsReport GenerateGroupsReport()
		{
			var reportBuilder = new PopsimGroupReportBuilder(_context.PopsimGlobalEthicGroups.ToList());

			return reportBuilder.Build();
		}

		
	}
}
