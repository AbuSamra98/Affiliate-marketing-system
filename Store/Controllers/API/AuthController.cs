//using System;
//using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using Store.Base.Models;
//using Store.Base.Models.ViewModels;
//using Store.Base.Utility;
//using Store.DataAccess.Data;

//namespace Store.Controllers.API
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthController : ControllerBase
//    {

//        private UserManager<ApplicationUser> _userManager;
//        private RoleManager<IdentityRole> _roleManager;
//        private readonly ApplicationDbContext _db;


//        public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
//        {
//            _userManager = userManager;
//            _roleManager = roleManager;
//            _db = db;
//        }



//        [HttpPost]
//        [Route("register")]
//        public async Task<IActionResult> Register(RegisterViewModel RegisterUser)
//        {
//            if (ModelState.IsValid)
//            {
//                var user = new ApplicationUser { Email = RegisterUser.Email, UserName = RegisterUser.Email, FullName = RegisterUser.FullName, LockoutEnd = DateTime.Now.AddYears(1000) };
//                var result = await _userManager.CreateAsync(user, RegisterUser.Password);
//                if (result.Succeeded)
//                {
//                    if (RegisterUser.UserType == 1)
//                    {
//                        await _userManager.AddToRoleAsync(user, SD.VendorEndUser);
//                        _db.Vendors.Add(new Vendor() { User = user });
//                    }
//                    else
//                    {
//                        await _userManager.AddToRoleAsync(user, SD.MarketerEndUser);
//                        _db.Marketers.Add(new Marketer() { User = user });
//                    }
//                    await _db.SaveChangesAsync();
//                    //await _signInManager.SignInAsync(user, isPersistent: false);
//                    return Ok("Register Successfully, wait for approve");
//                    //return RedirectToAction("Index","Home");
//                }
//            }
//            return BadRequest();
//        }



//        [HttpPost]
//        [Route("login")]
//        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
//        {
//            var user = await _userManager.FindByEmailAsync(model.Email);
//            if(user!=null)
//            {
//                if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
//                {
//                    return Unauthorized();
//                }
//                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
//                {
//                    var claims = new[]                  //save the data(the user that login) and communicate with it in JWT instead get the data from dataBase
//                    {
//                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//                        new Claim(ClaimTypes.Name, user.UserName),
//                    };

//                    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySuperSecurityKey"));
//                    var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512);

//                    var token = new JwtSecurityToken(
//                        expires: DateTime.UtcNow.AddHours(7),
//                        claims: claims,
//                        signingCredentials: creds
//                        );
//                    return Ok(new
//                    {
//                        token = new JwtSecurityTokenHandler().WriteToken(token),
//                        expiration = token.ValidTo
//                    });


//                }
//            }
            
//            return Unauthorized();
//        }
//    }
//}