namespace backend_app.DTO
{
    public class ResetPasswordRequest
    {
        public required string Email { get; set; }
        public required string Token { get; set; }
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
