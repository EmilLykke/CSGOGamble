using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSGOGamble.Models
{
    public class PostError
    {
        public bool error = true;
        public string title { get; set; }
        public string message { get; set; }
        public PostError(string title, string message)
        {
            this.title = title;
            this.message = message;
        }

    }
}