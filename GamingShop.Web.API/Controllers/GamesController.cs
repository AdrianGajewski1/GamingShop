﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GamingShop.Data.Models;
using GamingShop.Web.Data;
using GamingShop.Service;
using GamingShop.Web.API.Models.Response;
using Microsoft.AspNetCore.Identity;
using GamingShop.Service.Services;
using GamingShop.Web.API.Models;
using Microsoft.AspNetCore.Authorization;

namespace GamingShop.Web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGame _gamesService;
        private readonly IImage _imageService;

        public GamesController(ApplicationDbContext context, IGame gameService, UserManager<ApplicationUser> userManager,IImage image)
        {
            _context = context;
            _gamesService = gameService;
            _userManager = userManager;
            _imageService = image;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<GameIndexResponseModel>>> GetGames()
        {
            List<Game> games = new List<Game>();

            await Task.Run(() => 
            {
                games = _gamesService.GetAllAvailable().ToList();
            });

            var response = games.Select(game => new GameIndexResponseModel 
            {
                ID = game.ID,
                ImageUrl =  _imageService.GetImageNameForGame(game.ID),
                Platform = game.Platform,
                Price = game.Price,
                Producent = game.Producent,
                Title = game.Title
            });

            return response.ToArray();
        }

        [HttpGet("Search/{searchQuery}")]
        public async Task<ActionResult<IEnumerable<GameIndexResponseModel>>> GetBySearchQuery(string searchQuery)
        {
            if (searchQuery == string.Empty)
                return NotFound();

            List<Game> games = new List<Game>();

            await Task.Run(() =>
            {
                 games = _gamesService.GetAllBySearchQuery(searchQuery).ToList();
            }); 


            var response = games.Select(game => new GameIndexResponseModel
            {
                ID = game.ID,
                ImageUrl = _imageService.GetImageNameForGame(game.ID),
                Platform = game.Platform,
                Price = game.Price,
                Producent = game.Producent,
                Title = game.Title
            });

            return response.ToArray();
        }

        [HttpGet("GetGame/{id}")]
        public async Task<ActionResult<GameDetailsResponseModel>> GetGame(int id)
        {
            var game = _gamesService.GetByID(id);

            if (game == null)
            {
                return NotFound();
            }

            string ownerUsername = string.Empty;

            if (!string.IsNullOrEmpty(game.OwnerID))
            {
                var owner = await _userManager.FindByIdAsync(game.OwnerID);
                ownerUsername = owner.UserName;

            }
            else
            {
                ownerUsername = "Unknown";
            }

            var respone = new GameDetailsResponseModel
            {
                BestSeller = game.BestSeller,
                Description = game.Description,
                ImageUrl = _imageService.GetImageNameForGame(game.ID),
                LaunchDate = $"{game.DayOfLaunch}/{game.MonthOfLaunch}/{game.YearOfLaunch}",
                OwnerUsername = ownerUsername,
                Pegi = game.Pegi,
                Platform = game.Platform,
                Price = game.Price,
                Producent = game.Producent,
                Title = game.Title,
                Type = game.Type,
                Sold = game.Sold
            };

            return respone;
        }

        [HttpPut("UpdateGame/{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> PutGame(int id, [FromBody] UpdateGameModel game)
        {
            var userID = User.FindFirst(c => c.Type == "UserID").Value;

            var dayOfLaunch = game.LaunchDate.Split("/")[0];
            var monthOfLaunch = game.LaunchDate.Split("/")[1];
            var yearOfLaunch = game.LaunchDate.Split("/")[2];

            game.DayOfLaunch = dayOfLaunch;
            game.MonthOfLaunch = monthOfLaunch;
            game.YearOfLaunch = yearOfLaunch;
            game.OwnerID = userID;
            game.ImageUrl = _imageService.GetImageNameForGame(game.ID);

            if (id != game.ID)
            {
                return BadRequest();
            }

            _context.Entry(game).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        [HttpDelete("DeleteGame/{id}")]
        public async Task<ActionResult<Game>> DeleteGame(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.ID == id);
        }
    }
}
