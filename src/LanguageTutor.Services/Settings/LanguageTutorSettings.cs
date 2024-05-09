using LanguageTutor.Services.GoogleTextToSpeech;

namespace LanguageTutor.Services.Settings
{
	public class LanguageTutorSettings
	{
		public GoogleTextToSpeechApiSettings GoogleTextToSpeechApi { get; set; }

		public PracticeSettings Practice { get; set; }
	}
}
