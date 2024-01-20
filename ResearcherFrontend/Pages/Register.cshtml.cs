using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestSharp;

namespace ResearcherFrontend.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public String? Password { get; set; }

        [BindProperty]
        public String? Email { get; set; }

        [BindProperty]
        public String? FirstName { get; set; }

        [BindProperty]
        public String? LastName { get; set; }

        [BindProperty]
        public String? Organization { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {           

            var client = new RestClient(Config.ServerIP);
            var request = new RestRequest("auth/register/researcher");
            request.AddHeader("Content-type", "application/json");
            request.AddJsonBody(
                new
                {
                    firstName = FirstName, 
                    lastName = LastName, 
                    email = Email, 
                    password = Password,
                    organization = Organization
                }
                );
            var response = client.ExecutePost( request );
            if ( response != null )
            {
                Console.WriteLine( response );
            }

            return RedirectToPage("Login");
        }
    }
}
