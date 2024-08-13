using System;
using LanguageTutor.Services.Interfaces;

namespace LanguageTutor.Services.Internal
{
	internal class SystemClock : ISystemClock
	{
		public DateTimeOffset Now => DateTimeOffset.Now;

		public DateOnly Today => DateOnly.FromDateTime(DateTime.Now);
	}
}
