using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ResearcherFrontend.Pages
{
    public class LoginModel : PageModel
    {
        public static bool LoggedIn { get; set; } = false;
        public static string Token { get; set; }
        public static string ResearcherId { get; set; }

        [BindProperty]
        public String? Password { get; set; }

        [BindProperty]
        public String? Email { get; set; }

        public void OnGet()
        {
            Logout();
        }

        public IActionResult OnPost()
        {
            var client = new RestClient(Config.ServerIP);
            var request = new RestRequest("auth/login");
            request.AddHeader("Content-type", "application/json");
            request.AddJsonBody(
                new
                {
                    email = Email,
                    password = Password
                }
                );
            var response = client.ExecutePost(request);
            if (response != null)
            {
                JObject json = null;
                try
                {
                    json = JObject.Parse(response.Content);
                }catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ModelState.AddModelError("Error", "Check Credentials");
                    return Page();
                }
                if (json != null)
                {
                    //TODO: store researcherId
                    Token = (string)json.GetValue("token");
                    ResearcherId = json["account"]["researcherId"].ToString();
                    LoggedIn = true;
                }
                return RedirectToPage("Index");
            }

            if (LoggedIn)
            {
                return RedirectToPage("Index");
            }
            else
            {
                ModelState.AddModelError("Error", "Check Credentials");
                return Page();
            }
        }

        public void Logout()
        {
            LoggedIn = false;
            Token = null;
        }
    }
}
