using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Models;
using LanguageTutor.Services.Data;
using LanguageTutor.Services.Interfaces;
using LanguageTutor.Services.Interfaces.Repositories;
using LanguageTutor.Services.LanguageTraits;

namespace LanguageTutor.Services
{
	internal class ExerciseService : IExerciseService
	{
		private readonly IExerciseRepository exerciseRepository;

		private readonly ISupportedLanguageTraits supportedLanguageTraits;

		private readonly ISystemClock systemClock;

		public ExerciseService(ISupportedLanguageTraits supportedLanguageTraits, IExerciseRepository exerciseRepository, ISystemClock systemClock)
		{
			this.exerciseRepository = exerciseRepository ?? throw new ArgumentNullException(nameof(exerciseRepository));
			this.supportedLanguageTraits = supportedLanguageTraits ?? throw new ArgumentNullException(nameof(supportedLanguageTraits));
			this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
		}

		public bool InflectWordExercisesAreSupported(Language studiedLanguage)
		{
			return GetLanguageTraits(studiedLanguage).InflectWordExercisesAreSupported();
		}

		public Task<IReadOnlyCollection<InflectWordExerciseTypeDescriptor>> GetInflectWordExerciseTypes(Language studiedLanguage, CancellationToken cancellationToken)
		{
			var exerciseTypes = GetLanguageTraits(studiedLanguage).GetInflectWordExerciseTypes();
			return Task.FromResult(exerciseTypes);
		}

		private ILanguageTraits GetLanguageTraits(Language language)
		{
			return supportedLanguageTraits.GetLanguageTraits(language.Id);
		}

		public async Task AddInflectWordExercise(CreateInflectWordExerciseData createExerciseData, CancellationToken cancellationToken)
		{
			var exerciseTypeDescriptor = createExerciseData.ExerciseTypeDescriptor;

			var descriptionFilledFromTemplate = exerciseTypeDescriptor.GetDescription(createExerciseData.BaseForm);
			var hasDescriptionFromTemplate = String.Equals(descriptionFilledFromTemplate, createExerciseData.Description, StringComparison.Ordinal);

			var exerciseData = new SaveInflectWordExerciseData
			{
				LanguageId = createExerciseData.LanguageId,
				DescriptionTemplateId = hasDescriptionFromTemplate ? exerciseTypeDescriptor.DescriptionTemplate.Id : null,
				Description = hasDescriptionFromTemplate ? null : descriptionFilledFromTemplate,
				BaseForm = createExerciseData.BaseForm,
				WordForms = createExerciseData.WordForms,
				CreationTimestamp = systemClock.Now,
			};

			await exerciseRepository.AddInflectWordExercise(exerciseData, cancellationToken);
		}
	}
}
