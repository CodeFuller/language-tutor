namespace VocabularyCoach.Infrastructure.Sqlite.Internal
{
	internal interface IChecksumCalculator
	{
		uint CalculateChecksum(byte[] data);
	}
}
