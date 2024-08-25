using System;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeFuller.Library.Bootstrap;
using CommandLine;
using LanguageTutor.Infrastructure.Sqlite.Entities;
using LanguageTutor.Infrastructure.Sqlite.Internal;
using Microsoft.EntityFrameworkCore;

namespace LanguageTutor.MergeDatabases
{
	internal sealed class ApplicationLogic : IApplicationLogic
	{
		public async Task<int> Run(string[] args, CancellationToken cancellationToken)
		{
			var parseResult = Parser.Default.ParseArguments<CommandLineOptions>(args);

			if (parseResult.Tag == ParserResultType.Parsed)
			{
				await MergeDatabases(parseResult.Value, cancellationToken);
				return 0;
			}

			return 1;
		}

		private static async Task MergeDatabases(CommandLineOptions commandLineOptions, CancellationToken cancellationToken)
		{
			var sourceDbContext = GetDbContextOptions(commandLineOptions.SourceDatabaseDirectory);
			var targetDbContext = GetDbContextOptions(commandLineOptions.TargetDatabaseDirectory);

			var mergeUpdate = new MergeUpdate();

			await GetChangesForTexts(sourceDbContext, targetDbContext, mergeUpdate, cancellationToken);
			await GetChangesForTranslations(sourceDbContext, targetDbContext, mergeUpdate, cancellationToken);
			await GetChangesForInflectWordExercises(sourceDbContext, targetDbContext, mergeUpdate, cancellationToken);
			GetChangesForPronunciationRecords(commandLineOptions.SourceDatabaseDirectory, commandLineOptions.TargetDatabaseDirectory, mergeUpdate);

			if (mergeUpdate.IsEmpty)
			{
				Console.WriteLine("Target database is up to date");
				return;
			}

			var sb = new StringBuilder();
			sb.AppendLine("The following changes will be applied:");
			sb.AppendLine();
			sb.AppendLine(CultureInfo.InvariantCulture, $"{"Added texts: ",-36}{mergeUpdate.AddedTexts,6:N0}");
			sb.AppendLine(CultureInfo.InvariantCulture, $"{"Updated texts: ",-36}{mergeUpdate.UpdatedTexts,6:N0}");
			sb.AppendLine(CultureInfo.InvariantCulture, $"{"Deleted texts: ",-36}{mergeUpdate.DeletedTexts,6:N0}");
			sb.AppendLine(CultureInfo.InvariantCulture, $"{"Added translations: ",-36}{mergeUpdate.AddedTranslations,6:N0}");
			sb.AppendLine(CultureInfo.InvariantCulture, $"{"Deleted translations: ",-36}{mergeUpdate.DeletedTranslations,6:N0}");
			sb.AppendLine(CultureInfo.InvariantCulture, $"{"Added Inflect Word exercises: ",-36}{mergeUpdate.AddedInflectWordExercises,6:N0}");
			sb.AppendLine(CultureInfo.InvariantCulture, $"{"Updated Inflect Word exercises: ",-36}{mergeUpdate.UpdatedInflectWordExercises,6:N0}");
			sb.AppendLine(CultureInfo.InvariantCulture, $"{"Deleted Inflect Word exercises: ",-36}{mergeUpdate.DeletedInflectWordExercises,6:N0}");
			sb.AppendLine(CultureInfo.InvariantCulture, $"{"Added pronunciation records: ",-36}{mergeUpdate.PronunciationRecordsToAdd.Count,6:N0}");
			sb.AppendLine(CultureInfo.InvariantCulture, $"{"Deleted pronunciation records: ",-36}{mergeUpdate.PronunciationRecordsToDelete.Count,6:N0}");
			sb.AppendLine();
			sb.Append("Do you want to continue? [y/N] ");

			Console.Write(sb.ToString());

			var input = Console.ReadLine();
			if (String.Equals(input, "y", StringComparison.OrdinalIgnoreCase))
			{
				Console.WriteLine("Applying changes ...");

				await targetDbContext.SaveChangesAsync(cancellationToken);

				SavePronunciationRecordsChanges(mergeUpdate);

				Console.WriteLine("The changes were applied successfully");
			}
			else
			{
				Console.WriteLine("Exiting without applying changes");
			}
		}

		private static async Task GetChangesForTexts(LanguageTutorDbContext sourceDbContext, LanguageTutorDbContext targetDbContext, MergeUpdate mergeUpdate, CancellationToken cancellationToken)
		{
			var sourceTexts = (await sourceDbContext.Texts.ToListAsync(cancellationToken)).ToDictionary(x => x.Id);
			var targetTexts = (await targetDbContext.Texts.ToListAsync(cancellationToken)).ToDictionary(x => x.Id);

			foreach (var sourceTextItem in sourceTexts.OrderBy(x => x.Key))
			{
				var sourceText = sourceTextItem.Value;

				if (targetTexts.TryGetValue(sourceTextItem.Key, out var targetText))
				{
					if (UpdateText(sourceText, targetText))
					{
						++mergeUpdate.UpdatedTexts;
					}
				}
				else
				{
					targetDbContext.Texts.Add(new TextEntity
					{
						Id = sourceText.Id,
						LanguageId = sourceText.LanguageId,
						Text = sourceText.Text,
						Note = sourceText.Note,
						CreationTimestamp = sourceText.CreationTimestamp,
					});

					++mergeUpdate.AddedTexts;
				}
			}

			foreach (var targetTextItem in targetTexts.OrderBy(x => x.Key))
			{
				if (!sourceTexts.ContainsKey(targetTextItem.Key))
				{
					targetDbContext.Texts.Remove(targetTextItem.Value);
					++mergeUpdate.DeletedTexts;
				}
			}
		}

		private static bool UpdateText(TextEntity sourceText, TextEntity targetText)
		{
			var updated = false;

			if (targetText.LanguageId != sourceText.LanguageId)
			{
				targetText.LanguageId = sourceText.LanguageId;
				updated = true;
			}

			if (targetText.Text != sourceText.Text)
			{
				targetText.Text = sourceText.Text;
				updated = true;
			}

			if (targetText.Note != sourceText.Note)
			{
				targetText.Note = sourceText.Note;
				updated = true;
			}

			if (targetText.CreationTimestamp != sourceText.CreationTimestamp)
			{
				targetText.CreationTimestamp = sourceText.CreationTimestamp;
				updated = true;
			}

			return updated;
		}

		private static async Task GetChangesForTranslations(LanguageTutorDbContext sourceDbContext, LanguageTutorDbContext targetDbContext, MergeUpdate mergeUpdate, CancellationToken cancellationToken)
		{
			var sourceTranslations = (await sourceDbContext.Translations.ToListAsync(cancellationToken)).ToDictionary(x => (x.Text1Id, x.Text2Id));
			var targetTranslations = (await targetDbContext.Translations.ToListAsync(cancellationToken)).ToDictionary(x => (x.Text1Id, x.Text2Id));

			foreach (var sourceTranslationItem in sourceTranslations.OrderBy(x => x.Key.Text1Id).ThenBy(x => x.Key.Text2Id))
			{
				if (!targetTranslations.ContainsKey(sourceTranslationItem.Key))
				{
					targetDbContext.Translations.Add(new TranslationEntity
					{
						Text1Id = sourceTranslationItem.Value.Text1Id,
						Text2Id = sourceTranslationItem.Value.Text2Id,
					});

					++mergeUpdate.AddedTranslations;
				}
			}

			foreach (var targetTranslationItem in targetTranslations.OrderBy(x => x.Key.Text1Id).ThenBy(x => x.Key.Text2Id))
			{
				if (!sourceTranslations.ContainsKey(targetTranslationItem.Key))
				{
					targetDbContext.Translations.Remove(targetTranslationItem.Value);
					++mergeUpdate.DeletedTranslations;
				}
			}
		}

		private static async Task GetChangesForInflectWordExercises(LanguageTutorDbContext sourceDbContext, LanguageTutorDbContext targetDbContext, MergeUpdate mergeUpdate, CancellationToken cancellationToken)
		{
			var sourceExercises = (await sourceDbContext.InflectWordExercises.ToListAsync(cancellationToken)).ToDictionary(x => x.Id);
			var targetExercises = (await targetDbContext.InflectWordExercises.ToListAsync(cancellationToken)).ToDictionary(x => x.Id);

			foreach (var sourceExerciseItem in sourceExercises.OrderBy(x => x.Key))
			{
				var sourceExercise = sourceExerciseItem.Value;

				if (targetExercises.TryGetValue(sourceExerciseItem.Key, out var targetExercise))
				{
					if (UpdateInflectWordExercise(sourceExercise, targetExercise))
					{
						++mergeUpdate.UpdatedInflectWordExercises;
					}
				}
				else
				{
					targetDbContext.InflectWordExercises.Add(new InflectWordExerciseEntity
					{
						Id = sourceExercise.Id,
						LanguageId = sourceExercise.LanguageId,
						TemplateId = sourceExercise.TemplateId,
						Description = sourceExercise.Description,
						BaseForm = sourceExercise.BaseForm,
						WordForms = sourceExercise.WordForms,
						CreationTimestamp = sourceExercise.CreationTimestamp,
					});

					++mergeUpdate.AddedInflectWordExercises;
				}
			}

			foreach (var targetExerciseItem in targetExercises.OrderBy(x => x.Key))
			{
				if (!sourceExercises.ContainsKey(targetExerciseItem.Key))
				{
					targetDbContext.InflectWordExercises.Remove(targetExerciseItem.Value);
					++mergeUpdate.DeletedInflectWordExercises;
				}
			}
		}

		private static bool UpdateInflectWordExercise(InflectWordExerciseEntity sourceExercise, InflectWordExerciseEntity targetExercise)
		{
			var updated = false;

			if (targetExercise.LanguageId != sourceExercise.LanguageId)
			{
				targetExercise.LanguageId = sourceExercise.LanguageId;
				updated = true;
			}

			if (targetExercise.TemplateId != sourceExercise.TemplateId)
			{
				targetExercise.TemplateId = sourceExercise.TemplateId;
				updated = true;
			}

			if (targetExercise.Description != sourceExercise.Description)
			{
				targetExercise.Description = sourceExercise.Description;
				updated = true;
			}

			if (targetExercise.BaseForm != sourceExercise.BaseForm)
			{
				targetExercise.BaseForm = sourceExercise.BaseForm;
				updated = true;
			}

			if (targetExercise.WordForms != sourceExercise.WordForms)
			{
				targetExercise.WordForms = sourceExercise.WordForms;
				updated = true;
			}

			if (targetExercise.CreationTimestamp != sourceExercise.CreationTimestamp)
			{
				targetExercise.CreationTimestamp = sourceExercise.CreationTimestamp;
				updated = true;
			}

			return updated;
		}

		private static void GetChangesForPronunciationRecords(string sourceDatabaseDirectory, string targetDatabaseDirectory, MergeUpdate mergeUpdate)
		{
			var sourcePronunciationRecordsPath = Path.Combine(sourceDatabaseDirectory, "PronunciationRecords");
			var targetPronunciationRecordsPath = Path.Combine(targetDatabaseDirectory, "PronunciationRecords");

			var sourcePronunciationRecords = Directory.GetFiles(sourcePronunciationRecordsPath).Select(Path.GetFileName).ToHashSet();
			var targetPronunciationRecords = Directory.GetFiles(targetPronunciationRecordsPath).Select(Path.GetFileName).ToHashSet();

			foreach (var sourcePronunciationRecord in sourcePronunciationRecords)
			{
				if (!targetPronunciationRecords.Contains(sourcePronunciationRecord))
				{
					mergeUpdate.PronunciationRecordsToAdd.Add((SourcePath: Path.Combine(sourcePronunciationRecordsPath, sourcePronunciationRecord), Path.Combine(targetPronunciationRecordsPath, sourcePronunciationRecord)));
				}
			}

			foreach (var targetPronunciationRecord in targetPronunciationRecords)
			{
				if (!sourcePronunciationRecords.Contains(targetPronunciationRecord))
				{
					mergeUpdate.PronunciationRecordsToDelete.Add(Path.Combine(targetPronunciationRecordsPath, targetPronunciationRecord));
				}
			}
		}

		private static void SavePronunciationRecordsChanges(MergeUpdate mergeUpdate)
		{
			foreach (var pronunciationRecordsToAdd in mergeUpdate.PronunciationRecordsToAdd)
			{
				File.Copy(pronunciationRecordsToAdd.SourcePath, pronunciationRecordsToAdd.TargetPath);
			}

			foreach (var pronunciationRecordsToDelete in mergeUpdate.PronunciationRecordsToDelete)
			{
				File.Delete(pronunciationRecordsToDelete);
			}
		}

		private static LanguageTutorDbContext GetDbContextOptions(string databaseDirectory)
		{
			var databasePath = Path.Combine(databaseDirectory, "LanguageTutor.db");

			var connectionStringBuilder = new DbConnectionStringBuilder
			{
				{ "Data Source", databasePath },
				{ "Foreign Keys", "True" },
			};

			var connectionString = connectionStringBuilder.ToString();

			var dbContextOptionsBuilder = new DbContextOptionsBuilder<LanguageTutorDbContext>();
			dbContextOptionsBuilder.UseSqlite(connectionString, sqLiteOptions => sqLiteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));

			var dbContextFactory = new LanguageTutorDbContextFactory(dbContextOptionsBuilder.Options);

			return dbContextFactory.CreateDbContext();
		}
	}
}
