using Microsoft.Identity.Client;
using Microsoft.Identity.Client.AppConfig;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DemoAuthXamAAD
{
    public partial class App : Application
    {
        public static IPublicClientApplication PCA = null;

        /// <summary>
        /// The ClientID is the Application ID found in the portal (https://apps.dev.microsoft.com). 
        /// You can use the below id however if you create an app of your own you should replace the value here.
        /// </summary>
        public static string ClientID = "dec9e4cd-17c0-4fe3-a832-7d73c9b118f4";

        public static string[] Scopes = { "User.Read" };

        public static string[] Scopes1 = { "https://azureadmitredemo.onmicrosoft.com/BackEndApiDemo/AnotherScope" };
        //public static string[] Scopes1 = { "9fc8aa0e-e265-4967-9522-4b54e4a1325a/AnotherScope" };

        public static string Username = string.Empty;

        public static string TenantID = "52dd3814-4288-429d-8815-15feec88f8de";

        public static object ParentWindow { get; set; }
        public static IPlatformParameters _platformParameters;


        public App()
        {
            //This was the initial component constructor
            //InitializeComponent();
            //MainPage = new MainPage();

            PCA = PublicClientApplicationBuilder.Create(ClientID)
                .WithRedirectUri($"msal{App.ClientID}://auth")
                .WithTenantId(App.TenantID)
                .Build();
            MainPage = new NavigationPage(new DemoAuthXamAAD.MainPage());

        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
