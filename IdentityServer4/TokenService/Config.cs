
/*
 Identity Server dispose de trois entités principales que nous devons configurer : le ApiResource, le Clientet le TestUser. 
 Tous auront besoin d’une configuration minimale, mais avant de commencer, il est utile de garder à l’esprit les points suivants:

-L’ application console jouera le rôle de Client. Il utilise un ClientId & a Secret plus le nom d'utilisateur et le mot de passe d'un utilisateur pour obtenir le jeton.
-L’API ASP.NET Core sera bien sûr le ApiResource. Il utilise un ApiName & Secret plus le jeton d'accès, pour Claims .
-Un Client doit avoir un ApiResource dans leur AllowedScopes liste pour que le serveur Idenity permet l'accès
 
 
 */

using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer
{




    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId()
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("ApiName")
                {
                    ApiSecrets = {new Secret("secret_for_the_api".Sha256())}
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "ConsoleApp_ClientId",
                    ClientSecrets = { new Secret("secret_for_the_consoleapp".Sha256()) },
                    AccessTokenType = AccessTokenType.Reference,

                    //GrantTypes.ResourceOwnerPassword signifie simplement permettre à un client d'envoyer un nom d'utilisateur 
                    //et un mot de passe au service de jetons et de récupérer un jeton d'accès représentant cet utilisateur
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    // la liste des api auquelles il peut accéder 
                    AllowedScopes = { "ApiName" },
                    
                }
            };
        }



        /*
            toutes les méthodes de Config.cs sont appelées à partir de StartUp.cs 
            lors de la configuration du service, et nous suivrons ce "modèle" pour la TestUsers méthode  
        */

        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>()
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "123",
                    Password = "123".Sha256(),
                    //Claims =
                    //{
                    //    new Claim(JwtClaimTypes.Role, "SomeRole")
                    //}
                }
            };
        }
    }
}