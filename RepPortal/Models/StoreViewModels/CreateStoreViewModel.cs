using Microsoft.AspNetCore.Identity;
using RepPortal.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepPortal.Models.StoreViewModels
{
    public class CreateStoreViewModel
    {
        public List<ApplicationUser> Users { get; set; }

        public Store Store { get; set; }
    }
}
