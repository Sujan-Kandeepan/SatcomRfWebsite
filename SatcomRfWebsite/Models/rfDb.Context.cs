﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SatcomRfWebsite.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class rfDbEntities : DbContext
    {
        public rfDbEntities()
            : base("name=rfDbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tblATEOutput> tblATEOutput { get; set; }
        public virtual DbSet<tblCalData> tblCalData { get; set; }
        public virtual DbSet<tblKLYTestParameters> tblKLYTestParameters { get; set; }
        public virtual DbSet<tblKLYTestResults> tblKLYTestResults { get; set; }
        public virtual DbSet<tblModelNames> tblModelNames { get; set; }
        public virtual DbSet<tblMonitorData> tblMonitorData { get; set; }
        public virtual DbSet<tblOCCalHeaders> tblOCCalHeaders { get; set; }
        public virtual DbSet<tblProductTypes> tblProductTypes { get; set; }
        public virtual DbSet<tblPSCalHeaders> tblPSCalHeaders { get; set; }
        public virtual DbSet<tblSerialNumbers> tblSerialNumbers { get; set; }
        public virtual DbSet<tblSSPATestParameters> tblSSPATestParameters { get; set; }
        public virtual DbSet<tblSSPATestResults> tblSSPATestResults { get; set; }
        public virtual DbSet<tblTWTTestParameters> tblTWTTestParameters { get; set; }
        public virtual DbSet<tblTWTTestResults> tblTWTTestResults { get; set; }
    }
}
