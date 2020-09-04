using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Concurrent;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System.Linq;

namespace SportsStore.UnitTests.AdminTests
{
    /// <summary>
    /// Summary description for TestAdmin
    /// </summary>
    [TestClass]
    public class TestAdmin
    {
        [TestMethod]
        public void Index_Returns_CorrectProducts()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                { 
                new Product{ ProductId = 1, Name = "P1"},
                new Product{ ProductId = 2, Name = "P2"},
                new Product{ ProductId = 3, Name = "P3"}
                });

            AdminController controller = new AdminController(mock.Object);

            //Act
            Product[] result = ((IEnumerable<Product>)controller.Index().ViewData.Model).ToArray();

            //Assert
            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual("P1", result[0].Name);
            Assert.AreEqual("P2", result[1].Name);
            Assert.AreEqual("P3", result[2].Name);
        }
    }
}
