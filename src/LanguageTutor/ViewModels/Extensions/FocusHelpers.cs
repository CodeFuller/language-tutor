using System;
using System.Linq.Expressions;

namespace LanguageTutor.ViewModels.Extensions
{
	internal static class FocusHelpers
	{
		public static void SetFocus(Expression<Func<bool>> propertyExpression)
		{
			var parameter = Expression.Parameter(typeof(bool), "value");
			var body = Expression.Assign(propertyExpression.Body, parameter);
			var lambda = Expression.Lambda<Action<bool>>(body, parameter);
			var setterAction = lambda.Compile();

			// We set property to false and true, so that PropertyChanged event is triggered.
			setterAction.Invoke(false);
			setterAction.Invoke(true);
		}
	}
}
