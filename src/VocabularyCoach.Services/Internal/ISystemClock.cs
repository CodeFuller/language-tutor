using System;

namespace VocabularyCoach.Services.Internal
{
	internal interface ISystemClock
	{
		DateTimeOffset Now { get; }
	}
}
