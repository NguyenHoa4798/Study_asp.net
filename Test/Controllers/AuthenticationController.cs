using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using LoggerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Xml.Linq;
using Test.ActionFilters;

namespace Test.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAuthenticationManager _authManager;
        public AuthenticationController(ILoggerManager logger, 
                                        IMapper mapper,
                                        UserManager<User> userManager, 
                                        IAuthenticationManager authManager,
                                        IServiceProvider serviceProvider,
                                        RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _authManager = authManager;
            _roleManager = roleManager;
        }
        //Previous action
        [HttpPost("login")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto
       user)
        {
            if (!await _authManager.ValidateUser(user))
            {
                _logger.LogWarn($"{nameof(Authenticate)}: Authentication failed. Wrong user name or password.");
            return Unauthorized();
            }
            return Ok(new { Token = await _authManager.CreateToken() });
        }
        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
        {
            var user = _mapper.Map<User>(userForRegistration);
            var result = await _userManager.CreateAsync(user, userForRegistration.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            await _userManager.AddToRolesAsync(user, userForRegistration.Roles);
            return StatusCode(201);
        }
        [HttpPost("role"), Authorize("Administrator")]
        public async Task<IActionResult> RegisterRole(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                return Ok();
            }
            var role = new IdentityRole(roleName);
            await _roleManager.CreateAsync(role);

            return Ok();
        }
    }
}
