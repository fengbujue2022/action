﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    public  class Config
    {
        public IEnumerable<string> ToEmailAddressList { get; set; }
        public string EmailAccount { get; set; }
        public string EmailKey { get; set; }
    }
}
