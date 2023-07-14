using System.Runtime.Serialization;

namespace VocabularyCoach.Services.GoogleTextToSpeech.DataContracts
{
	[DataContract]
	internal class SynthesisInput
	{
		[DataMember(Name = "text")]
		public string Text { get; set; }
	}
}
