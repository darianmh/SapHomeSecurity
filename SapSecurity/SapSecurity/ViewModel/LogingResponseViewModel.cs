using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapSecurity.ViewModel
{
    public class LoginResponseViewModel
    {
        public LoginResponseViewModel(string token, string userId)
        {
            Token = token;
            UserId = userId;
        }

        public string Token { get; set; }
        public string UserId { get; set; }
    }
}
