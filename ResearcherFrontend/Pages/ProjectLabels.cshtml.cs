using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using RestSharp.Authenticators.OAuth2;
using RestSharp;

namespace ResearcherFrontend.Pages
{
    public class ProjectLabelsModel : PageModel
    {
        [BindProperty]
        public string ProjectId { get; set; }
        [BindProperty]
        public List<JObject> AllLabels { get; set; }
        [BindProperty]
        public string ProjectName { get; set; }
        public IActionResult OnGet(string projectId, string projectName)
        {
            if (!LoginModel.LoggedIn)
            {
                return RedirectToPage("Login");
            }

            ProjectId = projectId;
            ProjectName = projectName;


            var authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(LoginModel.Token, "Bearer");
            var options = new RestClientOptions(Config.ServerIP)
            {
                Authenticator = authenticator
            };
            var client = new RestClient(options);

            // sampleLabeledDataId
            // labeledDataId -> queryparam
            var request = new RestRequest("project/project/" + projectId + "/data?pageIndex=0");
            //var request = new RestRequest("project/project/" + projectId + "/data?pageIndex=0&pageSize=5000&labeledDataId=sampleLabeledDataId"); --> für chart
            request.AddHeader("Content-type", "application/json");
            var response = client.ExecuteGet(request);
            if (response != null && response.Content != null)
            {
                try
                {
                    JArray labels = JArray.Parse(response.Content);
                    AllLabels = [.. labels.Cast<JObject>()];

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ModelState.AddModelError("Error", "Error with the datamodel");
                    return RedirectToPage("Index");
                }


                request = new RestRequest("project/project/" + projectId);
                request.AddHeader("Content-type", "application/json");
                response = client.ExecuteGet(request);
                if (response != null && response.Content != null)
                {
                    try
                    {
                        JObject json = JObject.Parse(response.Content);
                        ProjectName = json["projectName"].ToString();
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
            else
            {
                return RedirectToPage("Index");
            }

        }
    }
}
