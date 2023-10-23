using System;
using ES6WebApi.Models.Responses;
using ES6WebApi.Providers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

using ES6WebApi.Models;

namespace ES6WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class GroupController : ControllerBase
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        /// <summary>Gets the server time.</summary>
        /// <returns>The server time.</returns>
        public GroupController(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        private static List<Group> groups = new List<Group>
        {
            new Group("Group 1"),
            new Group("Group 2"),
            // Add more entities as needed
        };


        [HttpGet]
        [Route("groups")]
        public ActionResult<IEnumerable<Group>> GetGroups()
        {
            return groups;
        }

        [HttpPost]
        [Route("group")]
        public ActionResult<Group> PostGroups(Group group)
        {

            groups.Add(group);

            return group;
        }
    }
}

