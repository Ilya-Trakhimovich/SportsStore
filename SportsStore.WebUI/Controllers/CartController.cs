﻿using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductRepository _repository;
        private readonly IOrderProcessor _orderProcessor;

        public CartController(IProductRepository productRepository, IOrderProcessor orderProcessor)
        {
            _repository = productRepository;
            _orderProcessor = orderProcessor;
        }

        public RedirectToRouteResult AddToCart(Cart cart, int productId, string returnUrl)
        {
            Product product = _repository.Products.FirstOrDefault(x => x.ProductId == productId);

            if (product != null)
            {
                cart.AddItem(product, 1);
            }

            return RedirectToAction("Index", new { returnUrl });
        }

        public RedirectToRouteResult RemoveFromCart(Cart cart, int productId, string returnUrl)
        {
            Product product = _repository.Products.FirstOrDefault(x => x.ProductId == productId);

            if (product != null)
            {
                cart.RemoveLine(product);
            }

            return RedirectToAction("Index", new { returnUrl });
        }

        public ViewResult Index(Cart cart,  string returnUrl)
        {
            return View(new CartIndexViewModel
            {
                Cart = cart,
                ReturnUrl = returnUrl
            });
        }

        public PartialViewResult Summary(Cart cart)
        {
            return PartialView(cart);
        }

        public ViewResult Checkout()
        {
            return View(new ShippingDetails());
        }

        [HttpPost]
        public ViewResult Checkout(Cart cart, ShippingDetails shippingDetails)
        {
            if (cart.Lines.Count() == 0)
            {
                ModelState.AddModelError("", "Sorry, your cart is empty");
            }

            if (ModelState.IsValid)
            {
                _orderProcessor.ProcessOrder(cart, shippingDetails);
                cart.Clear();

                return View("Completed");
            }
            else
            {
                return View(shippingDetails);
            }
        }
    }
}