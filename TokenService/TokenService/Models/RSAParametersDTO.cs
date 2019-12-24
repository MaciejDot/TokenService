using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokenService.Models
{
    public class RSAParametersDTO
    {
        public byte[] Exponent { get; set; }
        public byte[] Modulus { get; set; }
    }
}
