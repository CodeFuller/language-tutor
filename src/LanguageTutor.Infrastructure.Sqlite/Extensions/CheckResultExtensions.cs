using LanguageTutor.Infrastructure.Sqlite.Entities;
using LanguageTutor.Models;

namespace LanguageTutor.Infrastructure.Sqlite.Extensions
{
	internal static class CheckResultExtensions
	{
		public static CheckResult ToModel(this CheckResultEntity checkResultEntity)
		{
			return new CheckResult
			{
				Id = checkResultEntity.Id.ToItemId(),
				DateTime = checkResultEntity.DateTime,
				CheckResultType = checkResultEntity.ResultType,
				TypedText = checkResultEntity.TypedText,
			};
		}

		public static CheckResultEntity ToEntity(this CheckResult checkResultModel, ItemId userId, ItemId textId)
		{
			return new CheckResultEntity
			{
				Id = checkResultModel.Id?.ToInt32() ?? default,
				UserId = userId.ToInt32(),
				TextId = textId.ToInt32(),
				DateTime = checkResultModel.DateTime,
				ResultType = checkResultModel.CheckResultType,
				TypedText = checkResultModel.TypedText,
			};
		}
	}
}
