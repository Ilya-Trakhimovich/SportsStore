using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System.Linq;

namespace SportsStore.UnitTests.TestController
{
    /// <summary>
    /// Summary description for NavControllerTest
    /// </summary>
    [TestClass]
    public class NavControllerTest
    {
        [TestMethod]
        public void Menu_CanCreateCategories_Returns_CorrectCategories()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                { 
                new Product {ProductId = 1, Name = "p1", Category = "Apple"},
                new Product {ProductId = 2, Name = "p2", Category = "Apple"},
                new Product {ProductId = 3, Name = "p3", Category = "Plums"},
                new Product {ProductId = 4, Name = "p4", Category = "Oranges"},
                new Product {ProductId = 5, Name = "p5", Category = "Oranges"},
                });

            //Act
            NavController controller = new NavController(mock.Object);
            string[] result = ((IEnumerable<string>)controller.Menu().Model).ToArray();

            //Assert
            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual(result[0], "Apple");
            Assert.AreEqual(result[1], "Oranges");
            Assert.AreEqual(result[2], "Plums");
        }
    }
}
