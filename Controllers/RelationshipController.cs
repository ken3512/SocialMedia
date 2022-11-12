using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Services.RelationshipService;

namespace SocialMedia.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class RelationshipController : ControllerBase
    {
        private readonly IRelationshipService _relationshipService;
        public RelationshipController(IRelationshipService relationshipService)
        {
            _relationshipService = relationshipService;
            
        }
    }
}