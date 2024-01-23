using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using RestSharp.Authenticators.OAuth2;
using RestSharp;

namespace ResearcherFrontend.Pages
{
    public class CreateProjectModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (!LoginModel.LoggedIn)
            {
                return RedirectToPage("Login");
            }
           
            return Page();
        }

        public IActionResult OnPost()
        {

            var json = Request.Form["json"];

            var authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(LoginModel.Token, "Bearer");
            var options = new RestClientOptions(Config.ServerIP)
            {
                Authenticator = authenticator
            };
            var client = new RestClient(options);
            var request = new RestRequest("project/project");
            request.AddHeader("Content-type", "application/json");
            request.AddJsonBody(json.ToString());
            var response = client.ExecutePost(request);
            if (response != null && response.Content != null)
            {
                System.Threading.Thread.Sleep(1500);
                return RedirectToPage("Index");
            }
            else
            {
                ModelState.AddModelError("Error", "Create Project was not successful");
                return Page();
            }
        }
    }
}
