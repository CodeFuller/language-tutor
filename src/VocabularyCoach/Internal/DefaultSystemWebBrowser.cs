using System;
using System.Diagnostics;
using VocabularyCoach.Interfaces;

namespace VocabularyCoach.Internal
{
	internal sealed class DefaultSystemWebBrowser : IWebBrowser
	{
		public void OpenPage(Uri pageUrl)
		{
			var urlString = pageUrl.OriginalString;

			urlString = urlString.Replace("\"", "\\\"", StringComparison.Ordinal);

			var psi = new ProcessStartInfo
			{
				FileName = urlString,
				UseShellExecute = true,
			};

			Process.Start(psi);
		}
	}
}
