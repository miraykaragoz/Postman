using Microsoft.AspNetCore.Mvc;
using Postman.Models;
using RestSharp;

namespace Postman.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var client = new RestClient("https://dummyjson.com/");
        var request = new RestRequest("posts");
        var response = client.Get<PostsList>(request);
        return View(response);
    }

    public IActionResult Details(int id)
    {
        var client = new RestClient("https://dummyjson.com/");
        var request = new RestRequest($"posts/{id}");
        var response = client.Execute<Post>(request);
        return View(response.Data);
    }
    
    public IActionResult Admin()
    {
        var client = new RestClient("https://dummyjson.com/");
        var request = new RestRequest($"posts");
        var response = client.Get<PostsList>(request);
        return View(response);
    }

    public IActionResult Create()
    {
        return View(new Post());
    }

    [HttpPost]
    public IActionResult Create(Post post)
    {
        if (!ModelState.IsValid)
        {
            return View(post);
        }

        var captchaToken = Request.Form["g-recaptcha-response"];

        if (!VerifyCaptcha(captchaToken))
        {
            ViewBag.CaptchaError = true;
            return View(post);
        }

        var client = new RestClient("https://dummyjson.com/");
        var request = new RestRequest("posts/add", Method.Post);
        request.AddJsonBody(post);
        var response = client.Post<Post>(request);
        return Json(response);
    }

    public IActionResult Update(int id)
    {
        var client = new RestClient("https://dummyjson.com/");
        var request = new RestRequest($"posts/{id}");
        var response = client.Execute<Post>(request);
        return View(response.Data);
    }

    [HttpPost]
    public IActionResult Update(Post post)
    {
        var client = new RestClient("https://dummyjson.com/");
        var request = new RestRequest($"posts/{post.Id}", Method.Put);
        request.AddJsonBody(post);
        var response = client.Put<Post>(request);
        return Json(response);
    }

    public IActionResult Delete(int id)
    {
        var client = new RestClient("https://dummyjson.com/");
        var request = new RestRequest($"posts/{id}");
        var response = client.Delete<PostsList>(request);
        return Ok(new
        {
            message = "Post Silindi.",
        }) ;
    }
    
    public bool VerifyCaptcha(string captchaToken)
    {
        var client = new RestClient("https://www.google.com/recaptcha");
        var request = new RestRequest("api/siteverify", Method.Post);
        request.AddParameter("secret", "");
        request.AddParameter("response", captchaToken);

        var response = client.Execute<CaptchaResponse>(request);

        if (response.Data.Success)
        {
            return true;
        }
        return false;
    }
}