using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IHttpClientFactory httpClientFactory, ILogger<ProductsController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpGet("GetAllProducts")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProducts()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await httpClient.GetAsync("https://dummyjson.com/products");

                if (response.IsSuccessStatusCode)
                {
                    string productsJson = await response.Content.ReadAsStringAsync();
                    JObject jsonResponse = JObject.Parse(productsJson);

                    if (jsonResponse != null && jsonResponse["products"] != null)
                    {
                        var products = jsonResponse["products"].ToObject<List<Product>>();
                        var productsDTO = products.Select(p => new ProductDTO
                        {
                            Id = p.Id,
                            Image = p.Thumbnail,
                            Title = p.Title,
                            Price = p.Price,
                            ShortDescription = p.Description.Length > 100 ? p.Description.Substring(0, 100) : p.Description
                        });

                        return Ok(productsDTO);
                    }
                    else
                    {
                        _logger.LogError("Neuspješno dohvaćanje proizvoda. Polje 'products' nije pronađeno u odgovoru.");
                        return StatusCode(500, "Neuspješno dohvaćanje proizvoda.");
                    }
                }
                else
                {
                    _logger.LogError($"Neuspješno dohvaćanje proizvoda. Statusni kod: {response.StatusCode}");
                    return StatusCode((int)response.StatusCode, "Neuspješno dohvaćanje proizvoda.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Došlo je do pogreške prilikom dohvaćanja proizvoda: {ex.Message}");
                return StatusCode(500, $"Interna pogreška poslužitelja: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await httpClient.GetAsync($"https://dummyjson.com/products/{id}");

                if (response.IsSuccessStatusCode)
                {
                    string productJson = await response.Content.ReadAsStringAsync();
                    var product = Newtonsoft.Json.JsonConvert.DeserializeObject<Product>(productJson);
                    return Ok(product);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError($"Neuspješno dohvaćanje detalja proizvoda. Statusni kod: {response.StatusCode}");
                    return StatusCode((int)response.StatusCode, "Neuspješno dohvaćanje detalja proizvoda.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Došlo je do pogreške prilikom dohvaćanja detalja proizvoda: {ex.Message}");
                return StatusCode(500, $"Interna pogreška poslužitelja: {ex.Message}");
            }
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> FilterProductsByCategoryAndPrice(string category, decimal? price)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                string apiUrl = $"https://dummyjson.com/products";

                if (!string.IsNullOrEmpty(category))
                {
                    apiUrl += $"?category={category}";
                }

                if (price.HasValue)
                {
                    apiUrl += apiUrl.Contains("?") ? $"&price={price}" : $"?price={price}";
                }

                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string productsJson = await response.Content.ReadAsStringAsync();
                    JObject jsonResponse = JObject.Parse(productsJson);

                    if (jsonResponse["products"] != null && jsonResponse["products"].Type == JTokenType.Array)
                    {
                        var products = jsonResponse["products"].ToObject<List<Product>>();

                        if (!string.IsNullOrEmpty(category))
                        {
                            products = products.Where(p => p.Category.ToLower() == category.ToLower()).ToList();
                        }

                        if (price.HasValue)
                        {
                            products = products.Where(p => p.Price <= price).ToList();
                        }

                        var productsDTO = products.Select(p => new ProductDTO
                        {
                            Id = p.Id,
                            Image = p.Thumbnail,
                            Title = p.Title,
                            Price = p.Price,
                            ShortDescription = p.Description.Length > 100 ? p.Description.Substring(0, 100) : p.Description
                        });

                        return Ok(productsDTO);
                    }
                    else
                    {
                        _logger.LogError("Neuspješno dohvaćanje filtriranih proizvoda. Polje 'products' nije pronađeno ili nije polje u odgovoru.");
                        return StatusCode(500, "Neuspješno dohvaćanje filtriranih proizvoda.");
                    }
                }
                else
                {
                    _logger.LogError($"Neuspješno dohvaćanje filtriranih proizvoda. Statusni kod: {response.StatusCode}");
                    return StatusCode((int)response.StatusCode, "Neuspješno dohvaćanje filtriranih proizvoda.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Došlo je do pogreške prilikom filtriranja proizvoda: {ex.Message}");
                return StatusCode(500, $"Interna pogreška poslužitelja: {ex.Message}");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Product>>> SearchProducts(string title)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await httpClient.GetAsync($"https://dummyjson.com/products");

                if (response.IsSuccessStatusCode)
                {
                    string productsJson = await response.Content.ReadAsStringAsync();
                    JObject jsonResponse = JObject.Parse(productsJson);

                    if (jsonResponse != null && jsonResponse["products"] != null)
                    {
                        var products = jsonResponse["products"].ToObject<List<Product>>();

                        var filteredProducts = products.Where(p => p.Title.ToLower().Contains(title.ToLower())).ToList();

                        return Ok(filteredProducts);
                    }
                    else
                    {
                        _logger.LogError("Neuspješno dohvaćanje proizvoda. Polje 'products' nije pronađeno u odgovoru.");
                        return StatusCode(500, "Neuspješno dohvaćanje proizvoda.");
                    }
                }
                else
                {
                    _logger.LogError($"Neuspješno dohvaćanje proizvoda. Statusni kod: {response.StatusCode}");
                    return StatusCode((int)response.StatusCode, "Neuspješno dohvaćanje proizvoda.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Došlo je do pogreške prilikom pretrage proizvoda: {ex.Message}");
                return StatusCode(500, $"Interna pogreška poslužitelja: {ex.Message}");
            }
        }
    }
}
