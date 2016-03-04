using System;
using System.Linq;
using Microsoft.Data.Entity;
using Moq;

namespace BlockCert.Api.Tests.Models
{
	public static class MockHelper
	{
		public static Mock<DbSet<TModel>> ConvertQueryableToDbSet<TModel>(IQueryable<TModel> queryable)
			where TModel : class
		{
			var mockSet = new Mock<DbSet<TModel>>();
			mockSet.As<IQueryable<TModel>>().Setup(m => m.Provider).Returns(queryable.Provider);
			mockSet.As<IQueryable<TModel>>().Setup(m => m.Expression).Returns(queryable.Expression);
			mockSet.As<IQueryable<TModel>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
			mockSet.As<IQueryable<TModel>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

			return mockSet;
		}
	}
}

