using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMyAppAncket.Models.AncketVievModels
{
    public class AncketResult
    {
        public int Id { get; set; }
        public string AncketName { get; set; }
       

        public int AncketId { get; set; }
        public virtual Ancket Ancket { get; set; }

        public virtual List<AncketFormResult> AncketFormResults { get; set; }
    }
}
