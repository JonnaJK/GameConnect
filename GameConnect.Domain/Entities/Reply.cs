using System.ComponentModel.DataAnnotations.Schema;

namespace GameConnect.Domain.Entities
{
    public class Reply
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public virtual Post? Post { get; set; }
        public int? ReplyId { get; set; }
        public virtual ICollection<Reply>? Replies { get; set; }
        public string Text { get; set; }
        public string? ImageUrl { get; set; }
        public int? Votes { get; set; }
        public bool IsReported { get; set; }
    }
}
