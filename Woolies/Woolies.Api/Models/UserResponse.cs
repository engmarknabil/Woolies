using System;

namespace Woolies.Api.Models
{
    public class UserResponse
    {
        public string Name { get; set; }
        public Guid Token { get; set; }
    }
}