using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog3.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string TextBody { get; set; }
        public string AuthorId { get; set; }
        public int PostId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public string ReasonUpdated { get; set; }

        public virtual ApplicationUser Author { get; set; }
        public virtual Post Post { get; set; }
    }
}