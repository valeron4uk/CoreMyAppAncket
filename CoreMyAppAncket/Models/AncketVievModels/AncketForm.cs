using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMyAppAncket.Models.AncketVievModels
{
    public class AncketForm
    {
        public int Id { get; set; }
        public string FormName { get; set; }
        public FormType FormType { get; set; }
        public bool IsValid { get; set; }
        public long TextBoxIntData { get; set; }
        public string TextBoxStringData { get; set; }
        public bool CheckBoxData { get; set; }

        public int AncketId { get; set; }
        public virtual Ancket Ancket { get; set; }

    }

    public enum FormType
    {
        TextBoxString,
        TextArea,
        TextBoxInt,
        Email,
        Phone,
        CheckBox,
        ComboBox
    }
}
