namespace BlazingChat.Shared.Models
{
    public class MessagePack
    {
        public string ToUserId { get; set; }
        public string FromUserId { get; set; }
        public string Message { get; set; }
    }
}