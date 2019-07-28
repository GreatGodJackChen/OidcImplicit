using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ImplicitApi
{
    public class Config
    {
        //注册ApiResource，即授权后可访问的Api（PS：ApiResource对应的是OAuth2.0中的Scope）
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api","My Api")
            };
        }
        //注册IdentityResource，即授权后客户端可访问的用户信息（PS：IdentityResource对应的是OpenId Connect中的Scope）
        public static IEnumerable<IdentityResource> GetIdentityResources() => new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email()
        };
        //注册客户端，即可被授权的Client
        public static IEnumerable<Client> GetClients() => new List<Client>
        {
            new Client
            {
                ClientId = "mvc_implicit",
                ClientName = "MVC Client",
                AllowedGrantTypes = GrantTypes.Implicit,                //简化模式
                RequireConsent = true,     //Consent是授权页面，这里我们不进行授权

                RedirectUris = { "http://localhost:55926/signin-oidc" },
                PostLogoutRedirectUris = { "http://localhost:55926/signout-callback-oidc" },

                //授权后可以访问的用户信息（OpenId Connect Scope）与Api（OAuth2.0 Scope）
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "api"
                },

                 //允许返回Access Token
                AllowAccessTokensViaBrowser = true
            }
        };
        public static List<TestUser> GetTestUsers() => new List<TestUser>
        {
            new TestUser()
            {
                SubjectId="1",
                Username="admin",
                Password="admin",
                  Claims = new []
                    {
                        new Claim("name", "小明"),
                        new Claim("website", "https://alice.com")
                    }
            },
             new TestUser()
            {
                SubjectId="1",
                Username="admin",
                Password="admin",
                  Claims = new []
                    {
                        new Claim("name", "红红"),
                        new Claim("website", "https://alice.com")
                    }
            },
              new TestUser()
            {
                SubjectId="1",
                Username="admin",
                Password="admin",
                  Claims = new []
                    {
                        new Claim("name", "www因为"),
                        new Claim("website", "https://alice.com")
                    }
            }
        };
    }
}
