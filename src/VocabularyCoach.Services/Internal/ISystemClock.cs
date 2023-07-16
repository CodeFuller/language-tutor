using System;

namespace VocabularyCoach.Services.Internal
{
	internal interface ISystemClock
	{
		DateTimeOffset Now { get; }

		DateOnly Today { get; }
	}
}
