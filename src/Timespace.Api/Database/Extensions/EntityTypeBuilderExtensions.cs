using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query;

namespace Timespace.Api.Database.Extensions;

internal static class EntityTypeBuilderExtensions
{
	public static void AppendQueryFilter<T>(this EntityTypeBuilder entityTypeBuilder, Expression<Func<T, bool>> expression) where T : class
	{
		var parameterType = Expression.Parameter(entityTypeBuilder.Metadata.ClrType);

		var expressionFilter = ReplacingExpressionVisitor.Replace(
			expression.Parameters.Single(), parameterType, expression.Body);

		if (entityTypeBuilder.Metadata.GetQueryFilter() != null)
		{
			var currentQueryFilter = entityTypeBuilder.Metadata.GetQueryFilter()!;
			var currentExpressionFilter =
				ReplacingExpressionVisitor.Replace(currentQueryFilter.Parameters.Single(), parameterType, currentQueryFilter.Body);
			expressionFilter = Expression.AndAlso(currentExpressionFilter, expressionFilter);
		}

		var lambdaExpression = Expression.Lambda(expressionFilter, parameterType);
		_ = entityTypeBuilder.HasQueryFilter(lambdaExpression);
	}
}
