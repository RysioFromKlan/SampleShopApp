namespace SampleShopApp.Helpers
{

    /// <summary>
    /// Klasa CartService działa lokalny danych jaki i na danych Fake Store API. Dane o koszyku są wysyłane, ale nie zapisywane w API.
    /// The CartService class works with local data as well as Fake Store API data. Cart data is sent, but not saved in the API.
    /// </summary>
    /// TODO: parsowanie AdditionalProperties
    public static class CartService
    {
        /// <summary>
        /// Koszyk dla użytkownika z API
        /// Cart for user from API
        /// </summary>
        /// <param name="cartId"></param>
        /// <param name="storeApi"></param>
        /// <returns></returns>
        public static Cart GetCart(int cartId, fakestoreapiClient storeApi)
        {
            var cart = storeApi.GetCartByIdAsync(cartId).Result;
            return cart;
        }
        /// <summary>
        /// Koszyk dla lokalnego użytkownika
        /// Cart for local user
        /// </summary>
        /// <param name="user">Local user</param>
        /// <returns></returns>
        public static List<Product> GetCartItems(LocalUser? user)
        {
            if (user == null || user.cart == null)
                return new List<Product>();
            else
                return user.cart.Products.ToList();
        }

        //Zmiana założeń, po stronie frontu
        public static float CalculateTotal(LocalUser? user)
        {
            float total = 0;
            var cart = GetCartItems(user);
            var products = GetAllProducts();
            if (products != null)
                throw new Exception("No products");
            foreach (var item in cart)
            {
                var quanity = int.Parse(item.AdditionalProperties["quantity"].ToString());
                var productId = int.Parse(item.AdditionalProperties["productId"].ToString());
                var productPrice = products.Where(o => o.Id == productId).Single().Price;
                total += productPrice * quanity;
            }
            return total;
        }

        public static void ChangeQuantity(ref LocalUser user, int productId, int quantity)
        {
            if (user.cart == null)
                throw new Exception("Cart is empty");
            var currentProduct = user.cart.Products.Where(x => int.Parse(x.AdditionalProperties["productId"].ToString()) == productId).Single();
            currentProduct.AdditionalProperties["quantity"] = quantity;
        }


        public static int? GetProductsCount()
        {
            using (var httpclient = new HttpClient())
            {
                try
                {
                    var storeApi = new fakestoreapiClient(httpclient);
                    var products = storeApi.GetAllProductsAsync().Result.Count();
                    return products;
                }
                catch (Exception ex)
                {
                    GlobalLogger.logger.Error(ex.Message);
                    return null;
                }

            }
        }

        public static List<Product>? GetAllProducts()
        {
            using (var httpclient = new HttpClient())
            {
                try
                {
                    var storeApi = new fakestoreapiClient(httpclient);
                    var products = storeApi.GetAllProductsAsync().Result.ToList();
                    return products;
                }
                catch (Exception ex)
                {
                    GlobalLogger.logger.Error(ex.Message);
                    return null;
                }

            }
        }

        public static List<Product>? GetAllProducts(int skip = 0, int take = 10)
        {
            using (var httpclient = new HttpClient())
            {
                try
                {
                    var storeApi = new fakestoreapiClient(httpclient);
                    var products = storeApi.GetAllProductsAsync().Result.Skip(skip).Take(take).ToList();
                    return products;
                }
                catch (Exception ex)
                {
                    GlobalLogger.logger.Error(ex.Message);
                    return null;
                }

            }
        }
        public static List<Product> GetAllProducts(fakestoreapiClient fakestoreapi)
        {
            return fakestoreapi.GetAllProductsAsync().Result.ToList();
        }
    }
}

