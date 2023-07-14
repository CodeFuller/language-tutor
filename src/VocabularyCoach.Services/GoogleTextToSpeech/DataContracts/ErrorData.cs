using System.Runtime.Serialization;

namespace VocabularyCoach.Services.GoogleTextToSpeech.DataContracts
{
	[DataContract]
	internal class ErrorData
	{
		[DataMember(Name = "code")]
		public int Code { get; set; }

		[DataMember(Name = "message")]
		public string Message { get; set; }

		[DataMember(Name = "status")]
		public string Status { get; set; }
	}
}
