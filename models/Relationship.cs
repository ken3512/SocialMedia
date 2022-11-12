using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.models
{
    public class Relationship
    {
        public int Id { get; set; }
        public bool pending { get; set; } = true;
        public User Sender { get; set; }
        public User Receiver { get; set; }

        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
    }
}