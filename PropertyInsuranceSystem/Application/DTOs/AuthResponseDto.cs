using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public string Role { get; set; }

        public DateTime Expiration { get; set; }
    }
}
