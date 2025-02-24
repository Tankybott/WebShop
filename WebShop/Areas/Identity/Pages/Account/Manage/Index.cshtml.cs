using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Utility.Common.Interfaces;

public class IndexModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRetriver _userRetriver;

    public IndexModel(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IUnitOfWork unitOfWork,
        IUserRetriver userRetriver)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _unitOfWork = unitOfWork;
        _userRetriver = userRetriver;
    }

    public string? Username { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Phone]
        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }

        public string? Name { get; set; }
        public string? StreetAdress { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
    }

    private async Task LoadAsync()
    {
        var userId = _userRetriver.GetCurrentUserId();
        var applicationUser = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == userId);

        if (applicationUser == null)
        {
            throw new InvalidOperationException($"User with ID '{userId}' not found.");
        }

        Username = applicationUser.UserName; 

        Input = new InputModel
        {
            PhoneNumber = applicationUser.PhoneNumber, 
            Name = applicationUser.Name,
            StreetAdress = applicationUser.StreetAdress,
            City = applicationUser.City,
            Region = applicationUser.Region,
            PostalCode = applicationUser.PostalCode,
            Country = applicationUser.Country
        };
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        var userId = _userRetriver.GetCurrentUserId();
        var applicationUser = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == userId);

        if (applicationUser == null)
        {
            return NotFound($"User with ID '{userId}' not found.");
        }

        // Update user properties
        applicationUser.Name = Input.Name;
        applicationUser.StreetAdress = Input.StreetAdress;
        applicationUser.City = Input.City;
        applicationUser.Region = Input.Region;
        applicationUser.PostalCode = Input.PostalCode;
        applicationUser.Country = Input.Country;
        applicationUser.PhoneNumber = Input.PhoneNumber;

        _unitOfWork.ApplicationUser.Update(applicationUser);
        await _unitOfWork.SaveAsync();

        StatusMessage = "Your profile has been updated";
        return RedirectToPage();
    }
}
