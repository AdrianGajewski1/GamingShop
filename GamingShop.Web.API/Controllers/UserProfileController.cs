﻿using GamingShop.Data.Models;
using GamingShop.Service;
using GamingShop.Web.API.Models.Response;
using GamingShop.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AutoMapper;
using GamingShop.Data.DbContext;

namespace GamingShop.Web.API.Controllers
{
    /// <summary>
    /// Controller to handle User Profile actions
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private ApplicationDbContext _dbContext;
        private readonly ApplicationDbContextFactory _dbContextFactory;
        private readonly ApplicationOptions _options;
        private readonly IMapper _mapper;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="userManager">The user manager</param>
        /// <param name="context">Database context</param>
        /// <param name="options">Application options</param>
        /// <param name="imageService">Image service</param>
        public UserProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContextFactory contextFactory, 
            IOptions<ApplicationOptions> options, IMapper mapper)
        {
            _userManager = userManager;
            _dbContextFactory = contextFactory;
            _options = options.Value;
            _mapper = mapper;
        }

        /// <summary>
        /// Action to get user profile details
        /// </summary>
        /// <returns>User profile details</returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<ApplicationUserResponseModel>> GetUserProfile()
        {

                string userID = User.Claims.First(c => c.Type == "UserID").Value;

                var user = await _userManager.FindByIdAsync(userID);

                if (user != null)
                {
                    var response = _mapper.Map<ApplicationUserResponseModel>(user);

                    return response;
                }
                else
                    return BadRequest(new { message = "Cannot get user profile" });

        }

        #region Updating user profile details

        /// <summary>
        /// Updates user username
        /// </summary>
        /// <param name="username">A new username</param>
        /// <returns>200 Ok result if updating was successfull</returns>
        [HttpPost("UpdateUsername/{username}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> UpdateUsername(string username)
        {
                string userID = User.Claims.First(c => c.Type == "UserID").Value;

                var user = await _userManager.FindByIdAsync(userID);

                user.UserName = username;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return Ok();
                }

                return BadRequest();
        }

        /// <summary>
        /// Updates user email
        /// </summary>
        /// <param name="username">A new email</param>
        /// <returns>200 Ok result if updating was successfull</returns>
        [HttpPost("UpdateEmail/{email}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> UpdateEmail(string email)
        {
                string userID = User.Claims.First(c => c.Type == "UserID").Value;

                var user = await _userManager.FindByIdAsync(userID);

                user.Email = email;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return Ok();
                }

                return BadRequest();
        }

        /// <summary>
        /// Updates user phone number
        /// </summary>
        /// <param name="username">A new phone number</param>
        /// <returns>200 Ok result if updating was successfull</returns>
        [HttpPost("UpdatePhoneNumber/{phoneNumber}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> UpdatePhoneNumber(string phoneNumber)
        {
                string userID = User.Claims.First(c => c.Type == "UserID").Value;

                var user = await _userManager.FindByIdAsync(userID);

                user.PhoneNumber = phoneNumber;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return Ok();
                }

                return BadRequest();
        }

        #endregion

        /// <summary>
        /// An action to confirm user email 
        /// </summary>
        /// <param name="userID">User ID with to confirm email</param>
        /// <param name="token">A confirmation token</param>
        /// <returns></returns>
        [Route("ConfirmEmail/{userID}")]
        public async Task<IActionResult> ConfirmEmail(string userID, string token)
        {
            using (_dbContext = _dbContextFactory.CreateDbContext())
            {

                var user = await _userManager.FindByIdAsync(userID);

                await _userManager.ConfirmEmailAsync(user, token);

                await _dbContext.SaveChangesAsync();

                OpenUrl(_options.ClientURL + "/EmailConfirmation");

                return new NoContentResult();
            }
        }

        /// <summary>
        /// Opens browser with specified url
        /// </summary>
        /// <param name="url">The url to open</param>
        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
      
    }
}
