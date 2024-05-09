namespace LanguageTutor.Infrastructure.Sqlite.Internal
{
	internal interface IChecksumCalculator
	{
		uint CalculateChecksum(byte[] data);
	}
}
