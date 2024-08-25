using CommandLine;

namespace LanguageTutor.MergeDatabases
{
	internal sealed class CommandLineOptions
	{
		[Option('s', "source", Required = true, HelpText = "Source database directory.")]
		public string SourceDatabaseDirectory { get; set; }

		[Option('t', "target", Required = true, HelpText = "Target database directory.")]
		public string TargetDatabaseDirectory { get; set; }
	}
}
