using System.Runtime.Serialization;

namespace LanguageTutor.Services.GoogleTextToSpeech.DataContracts
{
	[DataContract]
	internal class ErrorResponse
	{
		[DataMember(Name = "error")]
		public ErrorData Error { get; set; }
	}
}
