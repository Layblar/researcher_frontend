using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using RestSharp.Authenticators.OAuth2;
using RestSharp;

namespace ResearcherFrontend.Pages
{
    public class EditProjectModel : PageModel
    {
        [BindProperty]
        public string Project { get; set; }

        [BindProperty]
        public string ProjectId { get; set; }
        public IActionResult OnGet(string projectId)
        {
            if (!LoginModel.LoggedIn)
            {
                return RedirectToPage("Login");
            }

            var authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(LoginModel.Token, "Bearer");
            var options = new RestClientOptions(Config.ServerIP)
            {
                Authenticator = authenticator
            };
            var client = new RestClient(options);

            var request = new RestRequest("project/project/" + projectId);
            request.AddHeader("Content-type", "application/json");
            var response = client.ExecuteGet(request);
            if (response != null && response.Content != null)
            {
                try
                {
                    var json = JObject.Parse(response.Content);
                    ProjectId = json.GetValue("projectId").ToString();
                    Project = JObject.Parse(response.Content).ToString();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ModelState.AddModelError("Error", "Error with the datamodel");
                    return RedirectToPage("Index");
                }

                    
                return Page();
            }
            else
            {
                return RedirectToPage("Index");
            }
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
            var request = new RestRequest("project/project/" + ProjectId);
            request.AddHeader("Content-type", "application/json");
            request.AddJsonBody(json.ToString());
            var response = client.ExecutePut(request);
            if (response != null && response.Content != null)
            {
                System.Threading.Thread.Sleep(1500);
                return RedirectToPage("Index");
            }
            else
            {
                ModelState.AddModelError("Error", "Edit Project was not successful");
                return Page();
            }
        }
    }
}
