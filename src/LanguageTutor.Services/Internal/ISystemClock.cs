using System;

namespace LanguageTutor.Services.Internal
{
	internal interface ISystemClock
	{
		DateTimeOffset Now { get; }

		DateOnly Today { get; }
	}
}
