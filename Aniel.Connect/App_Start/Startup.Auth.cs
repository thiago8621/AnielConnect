using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using Aniel.Connect.Models;
using System.Web.Mvc;
using System.Web;
using Aniel.Connect.Core;
using System.Security.Claims;
using System.Web.Helpers;

[assembly: OwinStartupAttribute("AnielConnectConfig", typeof(Aniel.Connect.Startup))]
namespace Aniel.Connect
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString(String.Format("/{0}/Account/Login", CultureHelper.GetDefaultCulture())),                
                CookieSecure = CookieSecureOption.SameAsRequest
            });

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: “”,
            //    clientSecret: “”);

            //app.UseTwitterAuthentication(
            //   consumerKey: “”,
            //   consumerSecret: “”);

            //app.UseFacebookAuthentication(
            //   appId: “”,
            //   appSecret: “”);




            
            //var options = new GoogleOAuth2AuthenticationOptions();
            //options.ClientId = "115198953352747452117";
            //options.ClientSecret = "679ce970bfba856effeb8189f9c881b1db855c50";
            //options.CallbackPath = new PathString("/oauth-redirect/google");

            //options.Scope.Add("https://www.googleapis.com/auth/books");
            //app.UseGoogleAuthentication(options);
            
            
            /*
            app.UseGoogleAuthentication(
                clientId: "115198953352747452117",
                clientSecret: "679ce970bfba856effeb8189f9c881b1db855c50");
            */
        }

        
    }
}