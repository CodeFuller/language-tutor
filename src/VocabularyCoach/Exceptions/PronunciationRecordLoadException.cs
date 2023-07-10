using System;

namespace VocabularyCoach.Exceptions
{
	public class PronunciationRecordLoadException : Exception
	{
		public PronunciationRecordLoadException()
		{
		}

		public PronunciationRecordLoadException(string message)
			: base(message)
		{
		}

		public PronunciationRecordLoadException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
