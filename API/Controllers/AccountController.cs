using System.Security.Claims;
using API.Dtos;
using API.Errors;
using API.Extensions;
using AutoMapper;
using Core.Entities.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, ITokenService tokenService,
            IMapper mapper)
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        /// <summary>
        /// The client sends an HTTP GET request to "account". That HTTP GET also contains a token. 
        /// This method then returns the user that corresponds to that token. 
        [Authorize]
        [HttpGet] // no route: put it at the top of the other methods
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            // After authentication, HttpContext contains data about the user.

            // Option 1: Get email from HttpContext and then user from UserManager
            // string email = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
            // AppUser user = await _userManager.FindByEmailAsync(email);

            // Option 2: Get email from the ClaimsPrincipal named User and then user from UserManager
            // string email = User.FindFirstValue(ClaimTypes.Email);
            // AppUser user = await _userManager.FindByEmailAsync(email);
            
            // Option 3: Same as option 2, but hidden in our own extension methods
            AppUser user = await _userManager.FindByEmailFromClaimsPrincipalAsync(User);
            
            return new UserDto 
            {
                Email = user.Email,
                Token = _tokenService.CreateToken(user),
                DisplayName = user.DisplayName
            };
        }
        
        /// <summary>
        /// Used by the client to asynchronously validate that the email they entered exists.
        /// </summary>
        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetUserAdress()
        {
            // We need to eagerly load the nav properties of AppUser
            AppUser user = await _userManager.FindUserByClaimsPrincipalWithAddressAsync(User);
            return _mapper.Map<Address, AddressDto>(user.Address);
        }

        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto address)
        {
            // If the AppUser.Address navigation already exists, this will load it from the Identity DB. Otherwise it will be null.
            AppUser user = await _userManager.FindUserByClaimsPrincipalWithAddressAsync(HttpContext.User);

            // This will create a new Address with Address.Id = 0, Address.User=null and Address.UserId=null (UserId is a string).
            user.Address = _mapper.Map<AddressDto, Address>(address);

            // This will delete the original user.Address from the Addresses table and add a new record for the new user.Address.
            // At this point, the new user.Address will get an new Address.Id (based on the previous PKs of the table) and its missing navigation properties will be set to Address.User=user and Address.UserId=user.Id
            //TODO: How can we update the existing Address object, instead of creating a new one
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(_mapper.Map<Address, AddressDto>(user.Address));
            }
            return BadRequest("Problem updating the user.");
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email); //returns null, if it does not find a user with this email in the DB

            if (user == null) 
            {
                return Unauthorized(new ApiResponse(401));
            }

            Microsoft.AspNetCore.Identity.SignInResult result 
                = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new ApiResponse(401));
            }
            return new UserDto 
            {
                Email = user.Email,
                Token = _tokenService.CreateToken(user),
                DisplayName = user.DisplayName
            };
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Email
            };

            IdentityResult result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse(400));
            }
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = _tokenService.CreateToken(user)
            };
        }
    }
}