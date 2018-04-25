using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMyAppAncket.Models.AncketVievModels
{
    public class AncketFormResult
    {
        public int Id { get; set; }
        public string NameForm { get; set; }
        public FormType FormType { get; set; }
        public string StringData { get; set; }
        public long IntData { get; set; }
        public bool CheckBoxData { get; set; }

        public int AncketResultId { get; set; }
        public virtual AncketResult AncketResult { get; set; }
        

    }
}
