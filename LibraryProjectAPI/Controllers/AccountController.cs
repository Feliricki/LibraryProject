using LibraryProjectAPI.Constants;
using LibraryProjectAPI.DTO.Account;
using LibraryProjectAPI.Models;
using LibraryProjectAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.IdentityModel.Tokens.Jwt;

namespace LibraryProjectAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtService _jwtService;


        public AccountController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager, 
            JwtService jwtService
            )
        {
            _context = context;
            _userManager = userManager;
            _jwtService = jwtService;
        }

        [HttpGet]
        [Authorize(Roles = ApplicationRoles.Librarian)]
        public async Task<IActionResult> LibrarianOnly()
        {
            await Task.CompletedTask;
            try
            {
                return Ok("User is authorized");
            } catch (Exception ex)
            {
                return Unauthorized("User is not authorized " + ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = $"{ApplicationRoles.Librarian}, {ApplicationRoles.User}")]
        public async Task<IActionResult> UsersOnly()
        {
            await Task.CompletedTask;
            try
            {
                return Ok("User is registered.");
            } catch (Exception)
            {
                return Unauthorized("User is not registered");
            }
        }

        [HttpPost(Name = "Signup")]
        public async Task<ActionResult<SignupResult>> Signup(SignupRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(GetFirstModelStateError(ModelState));
                }
                if (await _userManager.FindByNameAsync(request.UserName) != null)
                {
                    throw new Exception("Username is already in use.");
                }
                if (await _userManager.FindByEmailAsync(request.Email) != null)
                {
                    throw new Exception("Email is already in use.");
                }

                // At present, any user can register as a librarian
                var newUser = new ApplicationUser()
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };

                var identityResult = await _userManager.CreateAsync(newUser, request.Password);
                if (!identityResult.Succeeded)
                {
                    throw new Exception(identityResult.Errors.First().Description);
                }

                if (request.IsLibrarian)
                {
                    await _userManager.AddToRoleAsync(newUser, ApplicationRoles.Librarian);
                }
                await _userManager.AddToRoleAsync(newUser, ApplicationRoles.User);

                var sessionToken = await _jwtService.GetTokenAsync(newUser);
                var jwt = new JwtSecurityTokenHandler().WriteToken(sessionToken);
                return Ok(new SignupResult
                {
                    Success = true,
                    Message = "Signup has succeeded",
                    Role = request.IsLibrarian ? ApplicationRoles.Librarian : ApplicationRoles.User,
                    Token = jwt
                });


            } catch (Exception ex)
            {
                return BadRequest(new SignupResult
                {
                    Success = false,
                    Message = ex.Message,
                });
            }
        }

        // TODO: Test this method.
        [HttpPost(Name = "Login")]
        public async Task<ActionResult<LoginResult>> Login(LoginRequest loginRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(GetFirstModelStateError(ModelState));
                }

                var findByUsername = await _userManager.FindByNameAsync(loginRequest.UserName);
                var findByEmail = await _userManager.FindByEmailAsync(loginRequest.UserName);

                if (findByUsername == null && findByEmail == null)
                {
                    throw new Exception("User does not exists");
                }

                ApplicationUser foundUser = findByUsername ?? findByEmail!;

                if (await _userManager.CheckPasswordAsync(foundUser, loginRequest.Password))
                {
                    var sessionToken = await _jwtService.GetTokenAsync(foundUser);
                    var jwt = new JwtSecurityTokenHandler().WriteToken(sessionToken);
                    return Ok(new LoginResult
                    {
                        Success = true,
                        Message = "Login Successful",
                        Role = await _userManager.IsInRoleAsync(foundUser, ApplicationRoles.Librarian) ? ApplicationRoles.Librarian : ApplicationRoles.User,
                        Token = jwt
                    });
                }
                else
                {
                    throw new Exception("Invalid password");
                }
            }
            catch (Exception ex) 
            {
                return Unauthorized(new LoginResult
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        static string GetFirstModelStateError(ModelStateDictionary dictionary)
        {
            return dictionary.Values.First().Errors.First().ErrorMessage ?? "Invalid credentials";
        } 
    }
}
