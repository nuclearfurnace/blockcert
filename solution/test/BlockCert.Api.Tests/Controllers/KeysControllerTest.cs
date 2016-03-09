using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using BlockCert.Api.Controllers;
using BlockCert.Api.Database.Models;
using BlockCert.Api.Tests.Models;
using BlockCert.Api.Database;
using Moq;

namespace BlockCert.Api.Tests.Controllers
{
	public class KeysControllerTest
	{
		[Fact]
		public void CreateWith()
		{
			var keys = TypedKeyFixture.GenerateTypedKeyQueryable(0);
			var keysSet = MockHelper.ConvertQueryableToDbSet(keys);
			var mockContext = new Mock<BlockCertContext>();
			mockContext.Setup(c => c.Keys).Returns(keysSet.Object);

			var controller = new KeysController(mockContext.Object);
			var model = new PartialKey () { KeyName = "John Doe", KeyType = "Learner" };

			Assert.Equal("John Doe", model.KeyName);
			Assert.Equal("Learner", model.KeyType);

			var result = controller.Create(model);

			Assert.Equal("John Doe", result.KeyName);
			Assert.Equal("Learner", result.KeyType);
			Assert.NotEmpty(result.PrivateKey);
			Assert.NotEmpty(result.PublicAddress);
		}
	}
}