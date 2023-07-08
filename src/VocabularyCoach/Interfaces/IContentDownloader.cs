using System;
using System.Threading;
using System.Threading.Tasks;

namespace VocabularyCoach.Interfaces
{
	public interface IContentDownloader
	{
		Task<byte[]> Download(Uri contentUrl, CancellationToken cancellationToken);
	}
}
