using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebApplication1.Controllers;
using WebApplication1.Models;
using System.Linq;
using Newtonsoft.Json;
using System.Net;
using Moq.Protected;

namespace WebApplication1.Tests.Controllers
{
    public class ProductsControllerTests
    {
        private Mock<IHttpClientFactory> _mockHttpClientFactory;
        private Mock<ILogger<ProductsController>> _mockLogger;
        private ProductsController _controller;

        [SetUp]
        public void Setup()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockLogger = new Mock<ILogger<ProductsController>>();
            _controller = new ProductsController(_mockHttpClientFactory.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetAllProducts_ReturnsOkResult_WithListOfProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Title = "Product 1", Price = 100, Thumbnail = "Image1.jpg", Description = "Description 1" },
                new Product { Id = 2, Title = "Product 2", Price = 200, Thumbnail = "Image2.jpg", Description = "Description 2" }
            };

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(new { products }))
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var result = await _controller.GetAllProducts();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            var returnedProducts = okResult.Value as IEnumerable<ProductDTO>;
            Assert.NotNull(returnedProducts);
            Assert.AreEqual(2, returnedProducts.Count());
        }

        [Test]
        public async Task GetProductById_ReturnsOkResult_WithProduct()
        {
            // Arrange
            var product = new Product { Id = 1, Title = "Product 1", Price = 100, Thumbnail = "Image1.jpg", Description = "Description 1" };

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(product))
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var result = await _controller.GetProductById(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            var returnedProduct = okResult.Value as Product;
            Assert.NotNull(returnedProduct);
            Assert.AreEqual(1, returnedProduct.Id);
        }

        [Test]
        public async Task SearchProducts_ReturnsOkResult_WithFilteredProducts()
        {
            // Arrange
            string title = "Laptop";

            var products = new List<Product>
            {
                new Product { Id = 1, Title = "Laptop Lenovo", Price = 1200, Thumbnail = "laptop.jpg", Description = "Powerful laptop" },
                new Product { Id = 2, Title = "Tablet", Price = 300, Thumbnail = "tablet.jpg", Description = "Portable tablet" }
            };

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(new { products }))
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var result = await _controller.SearchProducts(title);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            var returnedProducts = okResult.Value as IEnumerable<Product>;
            Assert.NotNull(returnedProducts);
            Assert.AreEqual(1, returnedProducts.Count());
        }
    }
}
