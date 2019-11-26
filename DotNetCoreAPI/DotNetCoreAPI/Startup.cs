using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityServerTest.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.

        /*Immédiatement après l' AddMvcCore ajout du service d'autorisation à la collection de services avec AddAuthorization () .
         Vous pouvez également utiliser la AuthorizationOptions pour configurer une autorisation basée sur des revendications ou une stratégie .*/
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvcCore(options =>
                {
                    // demanderl'autorisation pour utiliser chaque fonction du controller 
                    options.Filters.Add(new AuthorizeFilter());
                }) 
                .AddJsonFormatters()
                .AddAuthorization()
                .SetCompatibilityVersion(CompatibilityVersion.Latest);
            /*
                Après cette configuration de base, vous devez configurer l'authentification. Vous faites cela avec la méthode AddAuthentication() .
                Étant donné que nous allons utiliser un jeton porteur OAuth 2.0 , 
                nous devons également le transmettre à la méthode. Ensuite, 
                nous ajoutons la configuration de l’authentification du serveur d’identité avec la méthode
                AddIdentityServerAuthentication() , dans laquelle nous devons définir l’URL de IdentityServer, le ApiName et bien sûr le secret
             */

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                    .AddIdentityServerAuthentication(options =>
                    {
                        options.Authority = "http://localhost:5000";
                        options.RequireHttpsMetadata = false;

                        options.ApiName = "ApiName";
                        options.ApiSecret = "secret_for_the_api";
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            /*
                Étape la plus facile! Ajoutez simplement AuthenticationMiddleware dans le pipeline,
                mais faites attention où! Ce doit être avant tous les autres middlewares, qui est la première dans le pipeline
                pipeLine : la ou on met les middlwars qui gèrent les requettes HTTP
             */
            app.UseAuthentication();



            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
