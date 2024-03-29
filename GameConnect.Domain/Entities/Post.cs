﻿namespace GameConnect.Domain.Entities
{
    public class Post
    {
        public int Id { get; set; }

        public virtual User? User { get; set; }
        public string UserId { get; set; }

        public virtual Tag? Tag { get; set; }
        public int? TagId { get; set; }

        public virtual Category? Category { get; set; }
        public int? CategoryId { get; set; }

        public virtual ICollection<Reply>? Replies { get; set; }

        public string Title { get; set; }
        public string? Text { get; set; }
        public DateTime Date { get; set; }
        public string? ImageUrl { get; set; }
        public int Votes { get; set; }
        public bool IsReported { get; set; }
    }
}
