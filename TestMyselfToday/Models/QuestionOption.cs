//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TestMyselfToday.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class QuestionOption
    {
        public long Id { get; set; }
        public long QuestionId { get; set; }
        public string OptionText { get; set; }
        public int Score { get; set; }
        public Nullable<int> SortOrder { get; set; }
    
        public virtual Question Question { get; set; }
    }
}
