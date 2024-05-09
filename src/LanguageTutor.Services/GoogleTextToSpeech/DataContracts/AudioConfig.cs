using System.Runtime.Serialization;

namespace LanguageTutor.Services.GoogleTextToSpeech.DataContracts
{
	[DataContract]
	internal class AudioConfig
	{
		[DataMember(Name = "audioEncoding")]
		public string AudioEncoding { get; set; }
	}
}
