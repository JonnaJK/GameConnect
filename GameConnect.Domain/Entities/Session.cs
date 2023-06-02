namespace GameConnect.Domain.Entities
{
    public class Session
    {
        public int Id { get; set; }
        public string CreatorId { get; set; }
        public virtual ICollection<User>? Participants { get; set; }
        public virtual ICollection<ChatMessage>? ChatMessages { get; set; }
    }
}
