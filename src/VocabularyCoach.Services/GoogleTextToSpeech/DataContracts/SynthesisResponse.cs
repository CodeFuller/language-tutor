using System.Runtime.Serialization;

namespace VocabularyCoach.Services.GoogleTextToSpeech.DataContracts
{
	[DataContract]
	internal sealed class SynthesisResponse
	{
		[DataMember(Name = "audioContent")]
		public string AudioContent { get; set; }
	}
}
