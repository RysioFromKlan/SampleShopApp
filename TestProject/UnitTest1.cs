using SampleShopApp;
using SampleShopApp.Helpers;

namespace TestProject
{
    public class UnitTest1
    {
        [Fact]
        public void GetProductTest()
        {           
            var testProducts =  CartService.GetAllProducts().ToList();
            Assert.True(testProducts.Any());
        }

        [Fact]
        public void GetProductTestCount()
        {
            var testProducts = CartService.GetAllProducts(0,1).ToList();
            Assert.Equal(1, testProducts.Count());
        }

        [Fact]
        public void ChangeQuantity_UpdatesProductQuantity()
        {
            // Arrange
            var user = new LocalUser
            {
                Username = "Test",

                cart = new Cart
                {
                    Products = new List<Product>
                {
                    new Product
                    {
                        AdditionalProperties = new Dictionary<string, object>
                        {
                            { "productId", "1" },
                            { "quantity", 2 }
                        }
                    }
                }
                }
            };

            int productId = 1;
            int newQuantity = 5;

            // Act
            CartService.ChangeQuantity(ref user, productId, newQuantity);

            // Assert
            var updatedProduct = user.cart.Products.Single(x => int.Parse(x.AdditionalProperties["productId"].ToString()) == productId);
            Assert.Equal(newQuantity, updatedProduct.AdditionalProperties["quantity"]);
        }

        //ToDo - Mock danych FakeStore 


    }
}