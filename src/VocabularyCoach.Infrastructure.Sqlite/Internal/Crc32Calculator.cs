using Force.Crc32;

namespace VocabularyCoach.Infrastructure.Sqlite.Internal
{
	internal sealed class Crc32Calculator : IChecksumCalculator
	{
		public uint CalculateChecksum(byte[] data)
		{
			return Crc32Algorithm.Compute(data);
		}
	}
}
