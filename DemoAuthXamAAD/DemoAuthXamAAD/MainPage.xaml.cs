using Microsoft.Identity.Client;

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DemoAuthXamAAD
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }


        async void OnSignInSignOut(object sender, EventArgs e)
        {
            AuthenticationResult authResult = null;
            IEnumerable<IAccount> accounts = await App.PCA.GetAccountsAsync();
            try
            {
                if (btnSignInSignOut.Text == "Sign in")
                {
                    // let's see if we have a user in our belly already
                    try
                    {
                        IAccount firstAccount = accounts.FirstOrDefault();
                        authResult = await App.PCA.AcquireTokenSilent(App.Scopes, firstAccount)
                                              .ExecuteAsync();
                        await RefreshUserDataAsync(authResult.AccessToken).ConfigureAwait(false);
                        await WebAPICall(authResult.AccessToken,App.Scopes1).ConfigureAwait(false);
                        Device.BeginInvokeOnMainThread(() => { btnSignInSignOut.Text = "Sign out"; });
                        
                    }
                    catch (MsalUiRequiredException ex)
                    {
                        try
                        {
                            //var authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext("https://login.windows.net/azureadmitredemo.onmicrosoft.com/");

                            //var authADALResult = await authContext.AcquireTokenAsync("5aba3729-cbc0-4fbe-9b38-38c863468a85", "dec9e4cd-17c0-4fe3-a832-7d73c9b118f4",new Uri("msaldec9e4cd-17c0-4fe3-a832-7d73c9b118f4"), App._platformParameters);

                            //var connectedUser = authADALResult.UserInfo;
                            //var _accessToken = authADALResult.AccessToken;
                            authResult = await App.PCA.AcquireTokenInteractive(App.Scopes, App.ParentWindow)
                                                      .ExecuteAsync();

                            await RefreshUserDataAsync(authResult.AccessToken);
                            await WebAPICall(authResult.AccessToken,App.Scopes1);
                            Device.BeginInvokeOnMainThread(() => { btnSignInSignOut.Text = "Sign out"; });
                        }
                        catch (Exception ex2)
                        {

                        }
                    }
                }
                else
                {
                    while (accounts.Any())
                    {
                        await App.PCA.RemoveAsync(accounts.FirstOrDefault());
                        accounts = await App.PCA.GetAccountsAsync();
                    }

                    slUser.IsVisible = false;
                    Device.BeginInvokeOnMainThread(() => { btnSignInSignOut.Text = "Sign in"; });
                }
            }
            catch (Exception ex)
            {
                var excep = ex.ToString();
                Device.BeginInvokeOnMainThread(() => { btnSignInSignOut.Text = "Sign out"; });
            }
        }

        public async Task RefreshUserDataAsync(string token)
        {
            //get data from API
            HttpClient client = new HttpClient();
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
            message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);            
            HttpResponseMessage response = await client.SendAsync(message);
            string responseString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                JObject user = JObject.Parse(responseString);

                slUser.IsVisible = true;

                Device.BeginInvokeOnMainThread(() =>
                {

                    lblDisplayName.Text = user["displayName"].ToString();
                    lblGivenName.Text = user["givenName"].ToString();
                    lblId.Text = user["id"].ToString();
                    lblSurname.Text = user["surname"].ToString();
                    lblUserPrincipalName.Text = user["userPrincipalName"].ToString();
                    lblServiceDataInvoked.Text = "";
                    // just in case
                    btnSignInSignOut.Text = "Sign out";
                });
            }
            else
            {
                await DisplayAlert("Something went wrong with the API call", responseString, "Dismiss");
            }
            
        }
        public async Task WebAPICall(string token, string [] scopes) {
            IEnumerable<IAccount> accounts = await App.PCA.GetAccountsAsync();
            IAccount firstAccount = accounts.FirstOrDefault();
            var authResult = await App.PCA.AcquireTokenSilent(scopes, firstAccount).ExecuteAsync();
            token = authResult.AccessToken;
            HttpClient client2 = new HttpClient();
            client2.BaseAddress = new Uri("https://webapidemo110419.azurewebsites.net/api/");
            client2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
            HttpRequestMessage message2 = new HttpRequestMessage(HttpMethod.Get, "values");
            //message2.Content = new StringContent("", Encoding.UTF8, "application/json");
            message2.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
            
            HttpResponseMessage response2 = await client2.SendAsync(message2);
            string responseString2 = await response2.Content.ReadAsStringAsync();
            if (response2.IsSuccessStatusCode)
            {               

                Device.BeginInvokeOnMainThread(() =>
                {
                    lblServiceDataInvoked.Text = responseString2.ToString();
                });
            }
            else
            {
                await DisplayAlert("Something went wrong with the API call", responseString2, "Dismiss");
            }
        }
    }
}
