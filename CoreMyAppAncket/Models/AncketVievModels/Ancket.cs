using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMyAppAncket.Models.AncketVievModels
{
    public class Ancket
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool IsSendMail { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        
        public virtual List<AncketForm> AncketForms { get; set; }

        

    }
}
