using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Entities;
using System.Linq;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Controllers;
using System.Web.Mvc;
using SportsStore.WebUI.Models;
using SportsStore.UnitTests.App_Start;

namespace SportsStore.UnitTests.TestCart
{
    [TestClass]
    public class CartTest
    {
        [TestMethod]
        public void Cart_AddNewLines_Returns_LinesInCart()
        {
            //Arrange
            var product1 = new Product { ProductId = 1, Name = "P1" };
            var product2 = new Product { ProductId = 2, Name = "P2" };
            var cart = new Cart();

            //Act
            cart.AddItem(product1, 2);
            cart.AddItem(product2, 3);
            CartLine[] result = cart.Lines.ToArray();

            //Assert
            Assert.AreEqual(result.Length, 2);
            Assert.AreEqual(result[0].Product, product1);
            Assert.AreEqual(result[1].Product, product2);
        }

        [TestMethod]
        public void Cart_AddQuantityForExistingLines_Returns_NewQuantity()
        {
            //Arrange
            var product1 = new Product { ProductId = 1, Name = "P1" };
            var product2 = new Product { ProductId = 2, Name = "P2" };
            var cart = new Cart();

            //Act
            cart.AddItem(product1, 2);
            cart.AddItem(product2, 3);
            cart.AddItem(product1, 1);
            CartLine[] result = cart.Lines.OrderBy(x => x.Product.ProductId).ToArray();

            //Assert
            Assert.AreEqual(result.Length, 2);
            Assert.AreEqual(result[0].Quantity, 3);
            Assert.AreEqual(result[1].Quantity, 3);
        }

        [TestMethod]
        public void Cart_RemoveLine()
        {
            //Arrange
            var product1 = new Product { ProductId = 1, Name = "P1" };
            var product2 = new Product { ProductId = 2, Name = "P2" };
            var product3 = new Product { ProductId = 3, Name = "P3" };
            var cart = new Cart();

            cart.AddItem(product1, 1);
            cart.AddItem(product2, 2);
            cart.AddItem(product3, 4);
            cart.AddItem(product2, 12);

            //Act
            cart.RemoveLine(product2);

            //Assert
            Assert.AreEqual(cart.Lines.Where(x => x.Product == product2).Count(), 0);
            Assert.AreEqual(cart.Lines.Count(), 2);
        }

        [TestMethod]
        public void Cart_CalculateTotalSum()
        {
            //Arrange
            var product1 = new Product { ProductId = 1, Name = "P1", Price = 123M };
            var product2 = new Product { ProductId = 2, Name = "P2", Price = 102M };
            var cart = new Cart();

            //Act
            cart.AddItem(product1, 1);
            cart.AddItem(product2, 2);
            cart.AddItem(product1, 4);

            decimal result = cart.ComputeTotalValue();

            //Assert
            Assert.AreEqual(result, 819M);
        }

        [TestMethod]
        public void Cart_Clear_Returns_EmtptyCart()
        {
            //Arrange
            var product1 = new Product { ProductId = 1, Name = "P1", Price = 123M };
            var product2 = new Product { ProductId = 2, Name = "P2", Price = 102M };
            var cart = new Cart();

            cart.AddItem(product1, 2);
            cart.AddItem(product2, 3);

            //Act
            cart.Clear();

            //Assert
            Assert.AreEqual(cart.Lines.Count(), 0);
        }

        [TestMethod]
        public void AddToCart_CanAddToCart()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                new Product{ ProductId = 1, Name = "P1", Category = "Cat1"},
                }.AsQueryable());
            Cart cart = new Cart();
            CartController controller = new CartController(mock.Object, null);

            //Act
            controller.AddToCart(cart, 1, null);

            //Assert
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductId, 1);
        }

        [TestMethod]
        public void AddToCart_AddProductToCart_Goes_ToCartScreen()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                new Product{ ProductId = 1, Name = "P1", Category = "Cat1"},
                }.AsQueryable());

            Cart cart = new Cart();
            CartController controller = new CartController(mock.Object, null);

            //Act
            RedirectToRouteResult result = controller.AddToCart(cart, 2, "myUrl");

            //Assert
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }

        [TestMethod]
        public void Index_CanViewCartContents()
        {
            //Arrange
            Cart cart = new Cart();
            CartController controller = new CartController(null, null);

            //Act 
            CartIndexViewModel result = (CartIndexViewModel)controller.Index(cart, "myUrl").ViewData.Model;

            //Assert
            Assert.AreEqual(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }

        [TestMethod]
        public void CheckOutPost_CannotCheckOutEmptyCart()
        {
            //Arrange
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            ShippingDetails shippingDetails = new ShippingDetails();
            CartController controller = new CartController(null, mock.Object);

            //Act
            ViewResult result = controller.Checkout(cart, shippingDetails);

            //Assert
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void CheckOutPost_CannotCheckoutInvalidShippingDetails()
        {
            //Arrange
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            CartController controller = new CartController(null, mock.Object);
            controller.ModelState.AddModelError("error", "error");

            //Act
            ViewResult result = controller.Checkout(cart, new ShippingDetails());

            //Assert
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void CheckOut_CanCheckOutAndSubmitOrder()
        {
            //Arrange
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            CartController controller = new CartController(null, mock.Object);

            //Act
            ViewResult result = controller.Checkout(cart, new ShippingDetails());

            //Assert
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once());
            Assert.AreEqual("Completed", result.ViewName);
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);            
        }
    }
}
