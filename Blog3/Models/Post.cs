using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog3.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [AllowHtml]
        public string TextBody { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public bool Published { get; set; }
        public string MediaUrl { get; set; }
        public string Slug { get; set; }

        public Post()
        {
            Comments = new HashSet<Comment>();
        }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}