using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepPortal.Models.StoreNotesViewModels
{
    public class StoreNoteViewModel
    {
        public ApplicationUser CurrentUser { get; set; }

        public StoreNote StoreNote { get; set; }

        public int CurrentStoreId { get; set; }

        public List<StoreNote> AllNotesForStore { get; set; }
    }
}
