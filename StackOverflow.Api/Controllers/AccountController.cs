using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StackOverflow.Infrastructure.Authentication;
using StackOverflow.Infrastructure.Authorization;

namespace StackOverflow.Api.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<TagUser> _userManager;
    private readonly SignInManager<TagUser> _signInManager;

    public AccountController(UserManager<TagUser> userManager, SignInManager<TagUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IActionResult Login() => View(new LoginVM());

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM loginVM)
    {
        if (!ModelState.IsValid) return View(loginVM);

        var user = await _userManager.FindByEmailAsync(loginVM.EmailAddress);
        if(user != null)
        {
            var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
            if (passwordCheck)
            {
                var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            TempData["Error"] = "Wrong credentials. Please, try again!";
            return View(loginVM);
        }

        TempData["Error"] = "Wrong credentials. Please, try again!";
        return View(loginVM);
    }


    public IActionResult Register() => View(new RegisterVM());

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM registerVM)
    {
        if (!ModelState.IsValid) return View(registerVM);

        var user = await _userManager.FindByEmailAsync(registerVM.EmailAddress);
        if(user != null)
        {
            TempData["Error"] = "This email address is already in use";
            return View(registerVM);
        }

        var newUser = new TagUser()
        {
            Email = registerVM.EmailAddress,
            UserName = registerVM.EmailAddress
        };
        var newUserResponse = await _userManager.CreateAsync(newUser, registerVM.Password);

        if (newUserResponse.Succeeded)
            await _userManager.AddToRoleAsync(newUser, UserRoles.User);

        return View("RegisterCompleted");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied(string ReturnUrl)
    {
        return View();
    }
}
