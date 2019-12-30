﻿using GamingShop.Data.Models;
using GamingShop.Service;
using GamingShop.Web.Data;
using GamingShop.Web.Models;
using GamingShop.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamingShop.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly ICart _cartService;
        private readonly ApplicationDbContext _dbContext;
        private readonly GamingShop.Service.IEmailSender _emailSender;
        private readonly IApplicationUser _userService;

        public OrderController(ICart cartService, ApplicationDbContext context, GamingShop.Service.IEmailSender emailSender, IApplicationUser applicationUser)
        {
            _dbContext = context;
            _cartService = cartService;
            _emailSender = emailSender;
            _userService = applicationUser;
        }


        public IActionResult Index()
        {

            var model = new OrderIndexModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(OrderIndexModel model)
        {
            var user = await _userService.GetUser(User);
            model.Games = _cartService.GetGames(user.CartID).ToList();

            model.CartID = user.CartID;
            model.TotalPrice = CalculateTotalPrice(model.Games);
            _dbContext.Orders.Add(new GamingShop.Data.Models.Order
            {
                CartID = model.CartID,
                City = model.City,
                Country = model.Country,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Street = model.Street,
                TotalPrice = model.TotalPrice
            });

            await _emailSender.SendOrderDetailsEmail(model.Email, "Order", model.Games, new Address { Street = model.Street, City = model.City, Country = model.Country, PhoneNumber = model.PhoneNumber }, model.TotalPrice);

            //TODO: Add this to some kind of users stats/latest orders

            await _cartService.ClearCart(model.CartID);

            return RedirectToAction("Index","Cart", new { id = model.CartID});
        }

        private decimal CalculateTotalPrice(IEnumerable<Game> games)
        {
            decimal price = 0;

            foreach (var game in games)
            {
                price += game.Price;
            }

            return price;
        }
    }
}