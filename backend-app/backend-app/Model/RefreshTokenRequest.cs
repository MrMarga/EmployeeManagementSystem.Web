﻿namespace backend_app.Model
{
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
        public string DeviceId { get; set; }
    }

}