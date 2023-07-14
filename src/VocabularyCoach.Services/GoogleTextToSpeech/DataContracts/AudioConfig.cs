using System.Runtime.Serialization;

namespace VocabularyCoach.Services.GoogleTextToSpeech.DataContracts
{
	[DataContract]
	internal class AudioConfig
	{
		[DataMember(Name = "audioEncoding")]
		public string AudioEncoding { get; set; }
	}
}
