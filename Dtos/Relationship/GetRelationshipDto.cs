using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocialMedia.Dtos.User;

namespace SocialMedia.Dtos.Relationship
{
    public class GetRelationshipDto
    {
        public int Id { get; set; }
        public bool pending { get; set; }
        public GetUserDto Sender { get; set; }
        public GetUserDto Receiver { get; set; }

    }
}