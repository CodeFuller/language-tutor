using System.Runtime.Serialization;

namespace LanguageTutor.Services.GoogleTextToSpeech.DataContracts
{
	internal class VoiceSelectionParams
	{
		[DataMember(Name = "languageCode")]
		public string LanguageCode { get; set; }

		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "ssmlGender")]
		public string SsmlGender { get; set; }
	}
}
