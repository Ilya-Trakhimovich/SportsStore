using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System.Linq;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests.TestController
{
    [TestClass]
    public class ProductControllerTest
    {
        [TestMethod]
        public void ShowProductList_CanPaginate_Returns_Products()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                new Product{ ProductId = 1, Name="A"},
                new Product{ ProductId = 2, Name="B"},
                new Product { ProductId = 3, Name = "C"},
                new Product{ ProductId = 4, Name = "D"},
                new Product{ ProductId = 5, Name="E"}
                });

            ProductController controller = new ProductController(mock.Object)
            {
                pageSize = 3
            };

            //Act
            ProductListViewModel result = (ProductListViewModel)controller.ShowProductList(null, 2).Model;
            Product[] productArray = result.Products.ToArray();

            //Assert
            Assert.IsTrue(productArray.Length == 2);
            Assert.AreEqual(productArray[0].Name, "D");
            Assert.AreEqual(productArray[1].Name, "E");
        }

        [TestMethod]
        public void ShowProductList_SendPagination_Returns_CorrectViewModel()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                new Product{  ProductId =1, Name = "A"},
                new Product{  ProductId =2, Name = "B"},
                new Product{  ProductId =3, Name = "C"},
                new Product{  ProductId =4, Name = "D"},
                new Product{  ProductId =5, Name = "E"},
                });

            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 3;

            //Act
            ProductListViewModel result = (ProductListViewModel)controller.ShowProductList(null, 2).Model;
            PagingInfo pagingInfo = result.PagingInfo;

            //Assert
            Assert.AreEqual(pagingInfo.CurrentPage, 2);
            Assert.AreEqual(pagingInfo.ItemsPerPage, 3);
            Assert.AreEqual(pagingInfo.TotalItems, 5);
            Assert.AreEqual(pagingInfo.TotalPages, 2);
        }

        [TestMethod]
        public void ShowProductList_CanFilterProducts_Returns_CorrectProducts()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                new Product{ ProductId = 1, Name = "P1", Category = "cat1"},
                new Product{ ProductId = 2, Name = "P2", Category = "cat2"},
                new Product{ ProductId = 3, Name = "P3", Category = "cat1"},
                new Product{ ProductId = 4, Name = "P4", Category = "cat2"},
                new Product{ ProductId = 5, Name = "P5", Category = "cat3"},
                });

            ProductController controller = new ProductController(mock.Object)
            {
                pageSize = 3
            };


            //Act
            Product[] result = ((ProductListViewModel)controller.ShowProductList("cat2", 1).Model).Products.ToArray();

            //Assert
            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "cat2");
        }

        [TestMethod]
        public void ShowProductList_GenerateCategorySpecificProductCount()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                new Product{ ProductId = 1, Name = "P1", Category = "cat1"},
                new Product{ ProductId = 2, Name = "P2", Category = "cat2"},
                new Product{ ProductId = 3, Name = "P3", Category = "cat1"},
                new Product{ ProductId = 4, Name = "P4", Category = "cat2"},
                new Product{ ProductId = 5, Name = "P5", Category = "cat3"},
                });

            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 3;

            //Act
            int res1 = ((ProductListViewModel)controller.ShowProductList("cat1").Model).PagingInfo.TotalItems;
            int res2 = ((ProductListViewModel)controller.ShowProductList("cat2").Model).PagingInfo.TotalItems;
            int res3 = ((ProductListViewModel)controller.ShowProductList("cat3").Model).PagingInfo.TotalItems;

            //Assert
            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
        }
    }
}
