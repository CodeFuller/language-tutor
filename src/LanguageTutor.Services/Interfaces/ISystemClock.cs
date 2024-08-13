using System;

namespace LanguageTutor.Services.Interfaces
{
	public interface ISystemClock
	{
		DateTimeOffset Now { get; }

		DateOnly Today { get; }
	}
}
