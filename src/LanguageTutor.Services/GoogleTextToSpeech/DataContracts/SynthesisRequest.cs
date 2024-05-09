using System.Runtime.Serialization;

namespace LanguageTutor.Services.GoogleTextToSpeech.DataContracts
{
	[DataContract]
	internal class SynthesisRequest
	{
		[DataMember(Name = "input")]
		public SynthesisInput Input { get; set; }

		[DataMember(Name = "voice")]
		public VoiceSelectionParams Voice { get; set; }

		[DataMember(Name = "audioConfig")]
		public AudioConfig AudioConfig { get; set; }
	}
}
