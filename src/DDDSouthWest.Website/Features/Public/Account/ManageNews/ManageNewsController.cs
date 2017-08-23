﻿using System.Linq;
using System.Threading.Tasks;
using DDDSouthWest.Domain.Features.Account.ManageNews.CreateNews;
using DDDSouthWest.Domain.Features.Account.ManageNews.ListNews;
using DDDSouthWest.Domain.Features.Account.ManageNews.UpdateExistingNews;
using DDDSouthWest.Domain.Features.Account.ManageNews.ViewNewsDetail;
using DDDSouthWest.Domain.Features.Account.ManagePages.CreatePage;
using DDDSouthWest.Website.Features.Public.Account.ManageEvents;
using DDDSouthWest.Website.Features.Public.Account.ManagePages;
using DDDSouthWest.Website.Framework;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DDDSouthWest.Website.Features.Public.Account.ManageNews
{
    [Authorize(Policy = AccessPolicies.OrganiserAccessPolicy)]
    public class ManageNewsController : Controller
    {
        private readonly IMediator _mediator;

        public ManageNewsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("/account/news/", Name = RouteNames.NewsManage)]
        public async Task<IActionResult> Index()
        {
            var result = await _mediator.Send(new ListAllNews.Query());
            
            return View(new NewsListViewModel
            {
                News = result.News
            });
        }

        [Route("/account/news/create", Name = RouteNames.NewsCreate)]
        public IActionResult Create()
        {
            return View(new ManageNewsViewModel());
        }

        [HttpPost]
        [Route("/account/news/create")]
        public async Task<IActionResult> Create(CreateNews.Command command)
        {
            CreateNews.Response result;

            try
            {
                result = await _mediator.Send(command);
            }
            catch (ValidationException e)
            {
                return View(new ManageNewsViewModel
                {
                    Errors = e.Errors.ToList(),
                    Title = command.Title,
                    Filename = command.Filename,
                    Body = command.Body,
                    IsLive = command.IsLive
                });
            }

            return RedirectToAction("Edit", result.Id);
        }

        [Route("/account/news/edit/{id}", Name = RouteNames.NewsEdit)]
        public async Task<IActionResult> Edit(ViewNewsDetail.Query query)
        {
            var model = await _mediator.Send(query);

            return View(new ManageNewsViewModel
            {
                Id = model.Id,
                Filename = model.Filename,
                Title = model.Title,
                Body = model.Body,
                DatePosted = model.DatePosted,
                IsLive = model.IsLive
            });
        }

        [HttpPost]
        [Route("/account/news/edit")]
        public async Task<IActionResult> Update(UpdateExistingNews.Command command)
        {
            try
            {
                await _mediator.Send(command);
            }
            catch (ValidationException e)
            {
                return View("Edit", new ManageNewsViewModel
                {
                    Errors = e.Errors.ToList(),
                    Title = command.Title,
                    Filename = command.Filename,
                    DatePosted = command.DatePosted,
                    Body = command.Body,
                    IsLive = command.IsLive
                });
            }

            return RedirectToRoute(RouteNames.NewsManage);
        }
    }
}