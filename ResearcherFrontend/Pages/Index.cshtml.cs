using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators.OAuth2;

namespace ResearcherFrontend.Pages
{
    public class IndexModel : PageModel
    {

        
        public List<JObject>? AllProjects { get; set; }
        public IActionResult OnGet()
        {

            if(!LoginModel.LoggedIn)
            {
                return RedirectToPage("Login");
            }

            var authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(LoginModel.Token, "Bearer");
            var options = new RestClientOptions(Config.ServerIP)
            {
                Authenticator = authenticator
            };
            var client = new RestClient(options);

            var request = new RestRequest("project/project?researcherId=" + LoginModel.ResearcherId);
            request.AddHeader("Content-type", "application/json");
            var response = client.ExecuteGet(request);
            if (response != null && response.Content != null)
            {
                try
                {  
                    JArray projects = JArray.Parse(response.Content);
                    AllProjects = [.. projects.Cast<JObject>()];                 
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ModelState.AddModelError("Error", "Check Credentials");
                    return Page();
                }
                return Page();
            }
            else
            {
                return Page();
            }
        }

    }

    public static class Config
    {
        public static readonly string ServerIP = "http://35.219.229.135";
    }
}
