using System;
using System.Collections.Generic;

namespace BlazingChat.Server.Models
{
    public partial class RefreshToken
    {
        public long TokenId { get; set; }
        public long UserId { get; set; }
        public string Token { get; set; }
        public byte[] ExpiryDate { get; set; }

        public virtual User User { get; set; }
    }
}
