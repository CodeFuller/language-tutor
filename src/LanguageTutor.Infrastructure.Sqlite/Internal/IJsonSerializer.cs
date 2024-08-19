namespace LanguageTutor.Infrastructure.Sqlite.Internal
{
	internal interface IJsonSerializer
	{
		string Serialize<TData>(TData data);

		TData Deserialize<TData>(string serializedData);
	}
}
