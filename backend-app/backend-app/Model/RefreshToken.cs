using System.ComponentModel.DataAnnotations;

namespace backend_app.Model
{
    public class RefreshToken
    {
       
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }

    
}

