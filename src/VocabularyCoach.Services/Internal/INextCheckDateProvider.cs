using System;
using VocabularyCoach.Models;

namespace VocabularyCoach.Services.Internal
{
	internal interface INextCheckDateProvider
	{
		DateOnly GetNextCheckDate(StudiedText studiedText);
	}
}
