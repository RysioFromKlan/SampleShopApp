using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using SampleShopApp.Helpers;
using SampleShopApp.Models;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SampleShopApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Product>? Get(int page, int pagesize)
        {
            page--;
            int skip = page * pagesize;
            return CartService.GetAllProducts(skip, pagesize);
        }

        [HttpGet("GetProductCount")]
        public int? GetProductCount()
        {
            return CartService.GetProductsCount();
        }

    }


    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        [HttpGet()]
        public IEnumerable<CartItem>? Get()
        {
            try
            {
                var sessionUser = UserService.GetNameFromUsersession(User);
                var user = UserService.GetUser(sessionUser, HttpContext);
                var products = CartService.GetAllProducts();

                if (user != null)
                {
                    List<CartItem> items = new List<CartItem>();
                    var cart = CartService.GetCartItems(user);


                    foreach (var cartItem in cart)
                    {
                        var productId = int.Parse(cartItem.AdditionalProperties["productId"].ToString());
                        var quantity = int.Parse(cartItem.AdditionalProperties["quantity"].ToString());
                        var currentProduct = products.Where(o => o.Id == productId).Single();

                        items.Add(new CartItem()
                        {
                            ProductId = productId,
                            Quantity = quantity,
                            ProductName = currentProduct.Title,
                            Price = currentProduct.Price
                        });

                    }
                    return items;
                }
                return null;
            }
            catch (Exception ex)
            {
                GlobalLogger.logger.Error(ex.Message);
                return null;
            }

        }

        [HttpGet("GetCartCount")]
        public int? GetCartCount()
        {
            try
            {
                var sessionUser = UserService.GetNameFromUsersession(User);
                var user = UserService.GetUser(sessionUser, HttpContext);
                if (user != null && user.cart != null)
                    return user.cart.Products.Count;
                else return null;
            }
            catch (Exception ex)
            {
                GlobalLogger.logger.Error(ex.Message);
                return null;
            }

        }

        public class LocalProduct
        {
            public int productId { get; set; }
        }
        public class ChangelProduct
        {
            public int productId { get; set; }
            public int quantity { get; set; }
        }

        [HttpPost("AddProduct")]
        public SendStatus AddProduct([FromBody] LocalProduct product)
        {
            #if (DEBUG)
            Random rnd = new Random();
            int dice = rnd.Next(1000, 5000);
            Thread.Sleep(5000);
            #endif

            try
            {
                var sessionUser = UserService.GetNameFromUsersession(User);
                var user = UserService.GetUser(sessionUser, HttpContext);
                if (user == null)
                    throw new Exception("No user");
                if (user.cart == null)
                    throw new Exception("No cart");
                if (product?.productId == null)
                    throw new Exception("No product");
                int productId = product.productId;

                //If product exist
                if (user.cart.Products.Where(o => (int.Parse(o.AdditionalProperties["productId"].ToString()) == productId)).Any())
                {
                    var changeProduct = user.cart.Products.Where(o => (int.Parse(o.AdditionalProperties["productId"].ToString()) == productId)).Single();
                    var quantity = int.Parse(changeProduct.AdditionalProperties["quantity"].ToString()) + 1;
                }
                //New product in cart
                else
                {
                    var maxId = user.cart.Products.Max(x => x.Id) + 1;
                    user.cart.Products.Add(new Product
                    {
                        Id = maxId,
                        AdditionalProperties = new Dictionary<string, object>()
                    {
                        { "productId", productId },
                        { "quantity", 1 }
                    }
                    });
                }
                UserService.SaveCartToExternalApi(user, user.cart);
                return SendStatus.Success;
            }
            catch (Exception ex)
            {
                GlobalLogger.logger.Error(ex.Message);
                return SendStatus.Error;
            }
        }

        [HttpPost]
        public SendStatus ChangeProduct([FromBody] ChangelProduct product)
        {
            try
            {
                var sessionUser = UserService.GetNameFromUsersession(User);
                var user = UserService.GetUser(sessionUser, HttpContext);
                if (user == null)
                    throw new Exception("No user");
                if (user.cart == null)
                    throw new Exception("No cart");
                if (product.quantity < 1)
                    throw new Exception("Wrong quantity");
                CartService.ChangeQuantity(ref user, product.productId, product.quantity);

                UserService.SaveCartToExternalApi(user, user.cart);
                return SendStatus.Success;
            }
            catch (Exception ex)
            {
                GlobalLogger.logger.Error(ex.Message);
                return SendStatus.Error;
            }
        }


        [HttpDelete("{productId:int}")]
        public SendStatus RemoveProduct(int productId)
        {
            try
            {
                var sessionUser = UserService.GetNameFromUsersession(User);
                var user = UserService.GetUser(sessionUser, HttpContext);
                if (user == null)
                    throw new Exception("No user");
                if (user.cart == null)
                    throw new Exception("No cart");
                var tempCart = user.cart.Products.ToList();
                tempCart.RemoveAll(x => int.Parse(x.AdditionalProperties["productId"].ToString()) == productId);
                user.cart.Products = tempCart;

                UserService.SaveCartToExternalApi(user, user.cart);

                return SendStatus.Success;
            }
            catch (Exception ex)
            {
                GlobalLogger.logger.Error(ex.Message);
                return SendStatus.Error;
            }
        }

    }
}
