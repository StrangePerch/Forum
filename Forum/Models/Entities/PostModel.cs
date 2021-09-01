using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Forum.Models
{
    public class PostModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string AuthorId { get; set; }
        public ForumUser Author { get; set; }
        public DateTime Created { get; set; }
        
        public DateTime Edited { get; set; }
        public int ThreadId { get; set; }
        public ThreadModel Thread { get; set; }
        
        public int? QuoteId { get; set; }
        public PostModel Quote { get; set; }
        public string AttachmentsPaths { get; set; }
    }
}