using System;
using LanguageTutor.Models;

namespace LanguageTutor.Services.Internal
{
	internal interface INextCheckDateProvider
	{
		DateOnly GetNextCheckDate(StudiedText studiedText);
	}
}
