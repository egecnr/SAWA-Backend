﻿using ExtraFunction.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraFunction.Repository_.Interface
{
    public interface IDisclaimersRepository
    {
        public Task<Disclaimers> GetDisclaimers(Disclaimers disclaimers);
        public Task<Disclaimers> UpdateDisclaimers(Disclaimers disclaimers);
    }
}
