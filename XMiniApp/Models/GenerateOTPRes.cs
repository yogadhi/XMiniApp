using System;
using System.Collections.Generic;
using System.Text;

namespace XMiniApp.Models
{
    public class GenerateOTPRes
    {
        public string TOTP { get; set; }
        public DateTime DateReq { get; set; }
        public DateTime DateExp { get; set; }
        public string ErrMsg { get; set; }
    }
}
