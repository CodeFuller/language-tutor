using System.Text.Json;

namespace LanguageTutor.Infrastructure.Sqlite.Internal
{
	internal sealed class JsonSerializer : IJsonSerializer
	{
		private readonly JsonSerializerOptions serializerOptions = new()
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		};

		public string Serialize<TData>(TData data)
		{
			return System.Text.Json.JsonSerializer.Serialize(data, serializerOptions);
		}

		public TData Deserialize<TData>(string serializedData)
		{
			return System.Text.Json.JsonSerializer.Deserialize<TData>(serializedData, serializerOptions);
		}
	}
}
