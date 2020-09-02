using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelpers;
using System.Web.Mvc;

namespace SportsStore.UnitTests.WebUI
{
    [TestClass]
    public class HTMlHelpers
    {
        [TestMethod]
        public void PageLinks_Paginate_Returns_CorrectPageNUmber()
        {
            //Arrange
            HtmlHelper html = null;
            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            Func<int, string> pageUrl = i => "Page" + i;

            //Act
            MvcHtmlString result = html.PageLinks(pagingInfo, pageUrl);

            //Assert
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page0"">0</a><a class=""btn btn-default"" href=""Page1"">1</a><a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>",
                result.ToString());
        }
    }

     
}
