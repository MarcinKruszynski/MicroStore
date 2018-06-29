// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using IdentityService.Models;
using System.Collections.Generic;

namespace IdentityService
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("products", "Products Service"),
                new ApiResource("booking", "Booking Service"),
                new ApiResource("notification", "Notification Service")
            };
        }

        public static IEnumerable<Client> GetClients(Dictionary<string, string> clientsUrl)
        {
            return new[]
            {
                // SPA client using implicit flow
                new Client
                {
                    ClientId = "spa",
                    ClientName = "SPA Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{clientsUrl["Spa"]}/callback.html" },
                    RequireConsent = false,
                    PostLogoutRedirectUris = { $"{clientsUrl["Spa"]}/index.html" },
                    AllowedCorsOrigins = { $"{clientsUrl["Spa"]}" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "products",
                        "booking",
                        "notification"
                    },
                },
                new Client
                {
                    ClientId = "productsswaggerui",
                    ClientName = "Products Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{clientsUrl["Products"]}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{clientsUrl["Products"]}/swagger/" },

                    AllowedScopes =
                    {
                        "products"
                    }
                },
                new Client
                {
                    ClientId = "bookingswaggerui",
                    ClientName = "Booking Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{clientsUrl["Booking"]}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{clientsUrl["Booking"]}/swagger/" },

                    AllowedScopes =
                    {
                        "booking"
                    }
                },
            };
        }
    }
}