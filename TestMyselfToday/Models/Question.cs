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
    
    public partial class Question
    {
        public Question()
        {
            this.QuestionOptions = new HashSet<QuestionOption>();
        }
    
        public long Id { get; set; }
        public long TestId { get; set; }
        public string Statement { get; set; }
        public string ImagePath { get; set; }
        public Nullable<int> SortOrder { get; set; }
    
        public virtual ICollection<QuestionOption> QuestionOptions { get; set; }
        public virtual Test Test { get; set; }
    }
}
