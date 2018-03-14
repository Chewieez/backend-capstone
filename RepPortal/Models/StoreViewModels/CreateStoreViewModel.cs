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
        [Display(Name="Sales Rep")]
        public string SalesRepId { get; set; }

        [Display(Name="Flag")]
        public string FlagId { get; set; }

        public Store Store { get; set; }
    }
}
