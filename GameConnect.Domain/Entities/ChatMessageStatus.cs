namespace GameConnect.Domain.Entities
{
    public class ChatMessageStatus
    {
        public int Id { get; set; }
        public int ChatMessageId { get; set; }
        public int SessionId { get; set; }
        public virtual Session? Session { get; set; }
        public string RecipientId { get; set; }
        public bool IsRead { get; set; }
    }
}
