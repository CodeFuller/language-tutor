using System.Runtime.Serialization;

namespace LanguageTutor.Services.GoogleTextToSpeech.DataContracts
{
	[DataContract]
	internal class SynthesisInput
	{
		[DataMember(Name = "text")]
		public string Text { get; set; }
	}
}
