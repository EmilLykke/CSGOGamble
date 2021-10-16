using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSGOGamble.Models
{
    public class PostError
    {
        public bool error = true;
        public string tile { get; set; }
        public string message { get; set; }
        public PostError(string title, string message)
        {
            this.tile = title;
            this.message = message;
        }

    }
}