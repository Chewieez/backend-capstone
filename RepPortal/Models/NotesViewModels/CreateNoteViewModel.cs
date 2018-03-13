using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepPortal.Models.NotesViewModels
{
    public class CreateNoteViewModel
    {
        public Note Note { get; set; }

        public string ToUserId { get; set; }
    }
}
