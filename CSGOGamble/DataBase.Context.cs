﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CSGOGamble
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CsgoBettingEntities1 : DbContext
    {
        public CsgoBettingEntities1()
            : base("name=CsgoBettingEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<bets> bets { get; set; }
        public virtual DbSet<roundkeys> roundkeys { get; set; }
        public virtual DbSet<rounds> rounds { get; set; }
        public virtual DbSet<users> users { get; set; }
        public virtual DbSet<messages> messages { get; set; }
    }
}
