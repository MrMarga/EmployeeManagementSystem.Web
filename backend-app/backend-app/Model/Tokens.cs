namespace backend_app.Model
{
    public class Tokens
    {
         
        public int Id { get; set; }
        public string Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpirationTime { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}

