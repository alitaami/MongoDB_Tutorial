using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Context;
using MongoDB.Driver;
using MongoDB.Entity;
using System.Text.Json;

namespace MongoDB.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HomeController : ControllerBase
    {
        private readonly MongoDBContext _mongoDBContext;
        public HomeController(MongoDBContext mongoDBContext)
        {
            _mongoDBContext = mongoDBContext;
        }

        // GET: HomeController
        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _mongoDBContext.Products
                                          .Find(_ => true)
                                          .ToList();

            return Ok(products);
        }

        // GET: HomeController/Details/id
        public ActionResult Details(int id)
        {
            var product = _mongoDBContext.Products
                                         .Find(x => x.Id == id)
                                         .FirstOrDefault();
            return Ok(product);
        }

        // Post: HomeController/Create
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            var collection = _mongoDBContext.Products;

            // Check if there are any documents in the collection.
            if (await collection.CountDocumentsAsync(new BsonDocument()) == 0)
            {
                product.Id = 1;
            }
            else
            {
                // Find the highest Id and increment it by one.
                var maxId = await collection.Find(_ => true)
                                           .Sort(Builders<Product>.Sort.Descending(p => p.Id))
                                           .Limit(1)
                                           .Project(p => p.Id)
                                           .FirstOrDefaultAsync();

                product.Id = maxId + 1;
            }
             
            await collection.InsertOneAsync(product);
 
            return Ok();
        }

        [HttpPut]
        public ActionResult Edit(int id, Product viewModel)
        {
            // Find the existing Product document by its ID
            var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
            var product = _mongoDBContext.Products.Find(filter).FirstOrDefault();

            if (product == null)
            {
                return NotFound();
            }

            // Update the fields of the retrieved product with values from the view model
            product.Name = viewModel.Name;
            product.Price = viewModel.Price;
            // Update other fields as needed

            // Save the updated product back to MongoDB
            var updateResult = _mongoDBContext.Products
                               .ReplaceOne(filter, product);

            if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
            {
                // The update was successful
                return Ok();
            }
            else
            {
                // Handle the case where the update failed
                return StatusCode(500); // Internal Server Error
            }
        }

        // Put: HomeController/Edit/5
        [HttpPut]
        public ActionResult Edit1(int id, Product product)
        {
            var filter = Builders<Product>.Filter
                         .Eq(product => product.Id, id);

            var update = Builders<Product>.Update
                .Set(p => p.Name, product.Name)
                .Set(p => p.Price, product.Price);

            var updateResult = _mongoDBContext.Products
                .UpdateOne(filter, update);

            if (updateResult.ModifiedCount == 1)
            {
                // The update was successful
                return Ok("Product updated successfully.");
            }
            else
            {
                // No documents matched the filter, or the update didn't modify any documents
                return NotFound("Product not found or no changes made.");
            }
        }

        // Delete: HomeController/Delete/5
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, id);

            var deleteProduct = _mongoDBContext.Products.DeleteOne(filter);

            if (deleteProduct.IsAcknowledged)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

    }

    //var x = new BsonDocument
    //{
    //    {
    //        "address",new BsonDocument
    //        {
    //           { "street","elahiparast" },
    //           { "zipCode","12343" }
    //        } 
    //    },
    //    {
    //        "family",new BsonDocument
    //        {
    //          {"sister","zahra" },
    //          { "brother","ali"}
    //        }
    //    }
    //};

    // ------

}
