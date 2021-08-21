using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Forum.Models
{
    public class PostModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        
        public int AuthorId { get; set; }
        
        public int ThreadId { get; set; }
        public ThreadModel Thread { get; set; }
        
        public string AttachmentsPaths { get; set; }
    }
}