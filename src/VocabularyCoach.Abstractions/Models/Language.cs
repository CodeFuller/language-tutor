using System;

namespace VocabularyCoach.Abstractions.Models
{
	public sealed class Language : IEquatable<Language>
	{
		public string Id { get; init; }

		public string Name { get; init; }

		public bool Equals(Language other)
		{
			if (other is null)
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return Id == other.Id && Name == other.Name;
		}

		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj) || (obj is Language other && Equals(other));
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Id, Name);
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
