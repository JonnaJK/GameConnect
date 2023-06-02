namespace GameConnect.Domain.Entities
{
    public class Vote
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int? PostId { get; set; }
        public int? ReplyId { get; set; }
        public bool IsUpvote { get; set; }
        public bool IsDownvote { get; set; }
    }
}
