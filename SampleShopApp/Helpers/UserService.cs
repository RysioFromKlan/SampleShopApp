using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace SampleShopApp.Helpers
{
    public static class UserService
    {
        private static Dictionary<string, LocalUser> localUsers = new Dictionary<string, LocalUser>();


        public static string? GetNameFromUsersession(ClaimsPrincipal user)
        {
            if (user.Identity != null && user.Identity.Name != null)
                return user.Identity.Name;
            else
                return null;
            //throw new Exception("User not in session");
        }

        public static LocalUser? GetUser(string? sessionLogin, HttpContext context)
        {

            if (sessionLogin != null)
            {
                if (localUsers.ContainsKey(sessionLogin))
                    return localUsers[sessionLogin];
                else
                {
                    context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
                    return null;
                }
            }
            else
                return null;
        }

        public static int IdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            var tokenS = jsonToken as JwtSecurityToken;
            return int.Parse(tokenS.Payload["sub"].ToString());
        }

        /// <summary>
        /// Zapisanie danych do FakeStoreAPI
        /// Saving data to FakeStoreAPI
        /// </summary>
        public static void SaveCartToExternalApi(LocalUser localUser, Cart cart)
        {
            using (var httpclient = new HttpClient())
            {
                try
                {
                    var storeApi = new fakestoreapiClient(httpclient);
                    if (localUser == null)
                        throw new Exception("No user");
                    if (localUser.cart == null)
                        throw new Exception("No cart");
                    storeApi.UpdateCartAsync(localUser.cart.Id, cart);

                }
                catch (Exception ex)
                {
                    GlobalLogger.logger.Error(ex.Message);
                }
            }
        }
        public static bool LoginUser(UserLoginPass userLoginPass, out string? error)
        {
            using (var httpclient = new HttpClient())
            {
                try
                {
                    var storeApi = new fakestoreapiClient(httpclient);
                    var fakeApiStoreUser = storeApi.LoginUserAsync(new Login
                    {
                        Username = userLoginPass.Login,
                        Password = userLoginPass.Password,
                    }).Result;

                    var id = IdFromToken(fakeApiStoreUser.Token);
                    error = null;
                    var cart = CartService.GetCart(id, storeApi);
                    if (localUsers.ContainsKey(userLoginPass.Login) == false)
                    {
                        localUsers.Add(userLoginPass.Login, new LocalUser
                        {
                            Username = userLoginPass.Login,
                            cart = cart,
                            Id = id
                        });

                    }
                    return true;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null && ex.InnerException is ApiException)
                    {
                        ApiException apiException = ex.InnerException as ApiException;
                        if (apiException.StatusCode == 401)
                        {
                            error = "Błędny użytkownik lub hasło";
                            return false;
                        }
                        else
                        {
                            error = "Bład serwera";
                            return false;
                        }
                    }
                    else
                    {
                        error = ex.Message;
                        return false;
                    }
                }
            }
        }
    }



    public class UserLoginPass
    {
        public required string Login { get; set; }
        public required string Password { get; set; }
        //public string ReturnUrl { get; set; }
    }



    public class LocalUser
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public Cart? cart { get; set; }

    }

}
