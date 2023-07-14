using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VocabularyCoach.Models;
using VocabularyCoach.Services.GoogleTextToSpeech.DataContracts;
using VocabularyCoach.Services.Interfaces;
using VocabularyCoach.Services.LanguageTraits;

namespace VocabularyCoach.Services.GoogleTextToSpeech
{
	internal class GoogleTextToSpeechSynthesizer : IPronunciationRecordSynthesizer
	{
		private readonly ISupportedLanguageTraits supportedLanguageTraits;

		private readonly HttpClient httpClient;

		private readonly GoogleTextToSpeechApiSettings settings;

		public GoogleTextToSpeechSynthesizer(ISupportedLanguageTraits supportedLanguageTraits, HttpClient httpClient, IOptions<GoogleTextToSpeechApiSettings> options)
		{
			this.supportedLanguageTraits = supportedLanguageTraits ?? throw new ArgumentNullException(nameof(supportedLanguageTraits));
			this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public async Task<PronunciationRecord> SynthesizePronunciationRecord(Language language, string text, CancellationToken cancellationToken)
		{
			var requestData = new SynthesisRequest
			{
				Input = new SynthesisInput
				{
					Text = text,
				},

				Voice = supportedLanguageTraits.GetLanguageTraits(language).GetSynthesisVoiceConfiguration(),

				AudioConfig = new AudioConfig
				{
					AudioEncoding = "MP3",
				},
			};

			using var jsonContent = JsonContent.Create(requestData);
			using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://texttospeech.googleapis.com/v1/text:synthesize")
			{
				Content = jsonContent,
			};

			if (String.IsNullOrEmpty(settings.ApiKey))
			{
				throw new InvalidOperationException("API key for Google Cloud Text-to-Speech API is not set");
			}

			requestMessage.Headers.Add("X-goog-api-key", settings.ApiKey);

			using var responseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);

			if (!responseMessage.IsSuccessStatusCode)
			{
				var errorResponse = await responseMessage.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: cancellationToken);
				var errorData = errorResponse.Error;
				throw new InvalidOperationException($"Failed to synthesize pronunciation record. HTTP error: {responseMessage.StatusCode} (code: {errorData?.Code}, status: {errorData?.Status}, message: {errorData?.Message})");
			}

			var responseData = await responseMessage.Content.ReadFromJsonAsync<SynthesisResponse>(cancellationToken: cancellationToken);
			var content = Convert.FromBase64String(responseData.AudioContent);

			return new PronunciationRecord
			{
				Data = content,
				Format = RecordFormat.Mp3,
				Source = "Google Cloud Text-to-Speech API",
			};
		}
	}
}
