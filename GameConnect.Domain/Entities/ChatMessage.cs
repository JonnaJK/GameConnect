namespace GameConnect.Domain.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public virtual User? User { get; set; }
        public int SessionId { get; set; }
        public virtual Session? Session { get; set; }
        public virtual ICollection<User>? Recipients { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
    }
}
