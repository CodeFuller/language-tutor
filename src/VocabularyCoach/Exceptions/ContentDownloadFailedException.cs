using System;

namespace VocabularyCoach.Exceptions
{
	public class ContentDownloadFailedException : Exception
	{
		public ContentDownloadFailedException()
		{
		}

		public ContentDownloadFailedException(string message)
			: base(message)
		{
		}

		public ContentDownloadFailedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
