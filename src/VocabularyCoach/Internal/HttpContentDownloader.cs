using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VocabularyCoach.Interfaces;

namespace VocabularyCoach.Internal
{
	internal sealed class HttpContentDownloader : IContentDownloader
	{
		public async Task<byte[]> Download(Uri contentUrl, CancellationToken cancellationToken)
		{
			using var httpClient = new HttpClient();
			using var request = new HttpRequestMessage(HttpMethod.Get, contentUrl);

			request.Headers.Accept.ParseAdd(@"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			request.Headers.AcceptEncoding.ParseAdd(@"gzip, deflate, br");
			request.Headers.UserAgent.ParseAdd(@"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.90 Safari/537.36 OPR/47.0.2631.80");

			using var response = await httpClient.SendAsync(request, cancellationToken);
			if (!response.IsSuccessStatusCode)
			{
				throw new InvalidOperationException($"Failed to download document '{contentUrl}'. Error code: {response.StatusCode}");
			}

			return await response.Content.ReadAsByteArrayAsync(cancellationToken);
		}
	}
}
