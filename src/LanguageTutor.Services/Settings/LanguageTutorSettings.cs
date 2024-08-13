using LanguageTutor.Services.GoogleTextToSpeech;

namespace LanguageTutor.Services.Settings
{
	public class LanguageTutorSettings
	{
		public GoogleTextToSpeechApiSettings GoogleTextToSpeechApi { get; set; }

		public ExercisesSettings Exercises { get; set; }
	}
}
