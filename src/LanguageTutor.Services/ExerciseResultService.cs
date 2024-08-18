using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageTutor.Models;
using LanguageTutor.Models.Exercises;
using LanguageTutor.Services.Interfaces;
using LanguageTutor.Services.Interfaces.Repositories;

namespace LanguageTutor.Services
{
	internal class ExerciseResultService : IExerciseResultService
	{
		private readonly IExerciseResultRepository exerciseResultRepository;

		public ExerciseResultService(IExerciseResultRepository exerciseResultRepository)
		{
			this.exerciseResultRepository = exerciseResultRepository ?? throw new ArgumentNullException(nameof(exerciseResultRepository));
		}

		public Task AddTranslateTextExerciseResult(User user, TranslateTextExercise exercise, TranslateTextExerciseResult exerciseResult, CancellationToken cancellationToken)
		{
			return exerciseResultRepository.AddTranslateTextExerciseResult(user.Id, exercise, exerciseResult, cancellationToken);
		}

		public Task AddInflectWordExerciseResult(User user, InflectWordExercise exercise, InflectWordExerciseResult exerciseResult, CancellationToken cancellationToken)
		{
			return exerciseResultRepository.AddInflectWordExerciseResult(user.Id, exercise, exerciseResult, cancellationToken);
		}
	}
}
