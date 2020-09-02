﻿using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _repository;
        public int pageSize = 4;

        public ProductController(IProductRepository productRepository)
        {
            _repository = productRepository;
        }

        public ViewResult ShowProductList(string category, int page = 0)
        {
            ProductListViewModel model = new ProductListViewModel()
            {
                Products = _repository.Products
                                    .Where(p => p.Category == null || p.Category == category)
                                    .OrderBy(p => p.ProductId)
                                    .Skip(page * pageSize)
                                    .Take(pageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = _repository.Products.Count()
                },
                CurrentCategory = category
            };
            return View(model);
        }
    }
}