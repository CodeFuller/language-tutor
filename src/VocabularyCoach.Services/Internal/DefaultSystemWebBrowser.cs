using System;
using System.Diagnostics;

namespace VocabularyCoach.Services.Internal
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
