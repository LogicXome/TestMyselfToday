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
    
    public partial class Test
    {
        public Test()
        {
            this.Questions = new HashSet<Question>();
            this.RelatedTests = new HashSet<RelatedTest>();
            this.RelatedTests1 = new HashSet<RelatedTest>();
            this.TestResults = new HashSet<TestResult>();
            this.TestStats = new HashSet<TestStat>();
        }
    
        public long Id { get; set; }
        public string Title { get; set; }
        public string ImagePath { get; set; }
        public Nullable<long> SectionId { get; set; }
        public Nullable<long> LanguageId { get; set; }
        public Nullable<System.DateTime> DateAndTime { get; set; }
        public Nullable<int> SortOrder { get; set; }
    
        public virtual Language Language { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<RelatedTest> RelatedTests { get; set; }
        public virtual ICollection<RelatedTest> RelatedTests1 { get; set; }
        public virtual ICollection<TestResult> TestResults { get; set; }
        public virtual ICollection<TestStat> TestStats { get; set; }
        public virtual Section Section { get; set; }
    }
}
