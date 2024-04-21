using VocabularyCoach.Services.GoogleTextToSpeech;

namespace VocabularyCoach.Services.Settings
{
	public class VocabularyCoachSettings
	{
		public GoogleTextToSpeechApiSettings GoogleTextToSpeechApi { get; set; }

		public PracticeSettings Practice { get; set; }
	}
}
