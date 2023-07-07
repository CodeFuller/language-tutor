using System;
using VocabularyCoach.ViewModels.Data;

namespace VocabularyCoach.Events
{
	internal sealed class SwitchToCheckResultsPageEventArgs : EventArgs
	{
		public CheckResults CheckResults { get; }

		public SwitchToCheckResultsPageEventArgs(CheckResults checkResults)
		{
			CheckResults = checkResults ?? throw new ArgumentNullException(nameof(checkResults));
		}
	}
}
