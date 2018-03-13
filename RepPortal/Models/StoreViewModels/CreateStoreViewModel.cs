using Microsoft.AspNetCore.Identity;
using RepPortal.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RepPortal.Models.StoreViewModels
{
    public class CreateStoreViewModel
    {
        [Display(Name="To User")]
        public string SalesRepId { get; set; }

        public Store Store { get; set; }
    }
}
