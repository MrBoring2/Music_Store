using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Music_Store.Data
{
    public partial class MusicStoreContext : DbContext
    {
        public MusicStoreContext()
            : base("name=MusicStoreContext")
        {
        }

        public virtual DbSet<Delivery> Delivery { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<Genre> Genre { get; set; }
        public virtual DbSet<MusicRecord> MusicRecord { get; set; }
        public virtual DbSet<MusicRecordInDelivery> MusicRecordInDelivery { get; set; }
        public virtual DbSet<MusicRecordInOrder> MusicRecordInOrder { get; set; }
        public virtual DbSet<MusicRecordType> MusicRecordType { get; set; }
        public virtual DbSet<MusicRecordsInTheHall> MusinRecordsInTheHall { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Supplier> Supplier { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Delivery>()
                .HasMany(e => e.MusicRecordInDelivery)
                .WithRequired(e => e.Delivery)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.FullName)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.PhoneNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.Login)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Delivery)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Order)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Genre>()
                .Property(e => e.Title)
                .IsUnicode(false);

            modelBuilder.Entity<Genre>()
                .HasMany(e => e.MusicRecord)
                .WithMany(e => e.Genre)
                .Map(m => m.ToTable("MusicRecordGeneres").MapLeftKey("GenreId").MapRightKey("MusicRecordId"));

            modelBuilder.Entity<MusicRecord>()
                .Property(e => e.Title)
                .IsUnicode(false);

            modelBuilder.Entity<MusicRecord>()
                .Property(e => e.Label)
                .IsUnicode(false);

            modelBuilder.Entity<MusicRecord>()
                .Property(e => e.Performers)
                .IsUnicode(false);

            modelBuilder.Entity<MusicRecord>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<MusicRecord>()
                .Property(e => e.TreckList)
                .IsUnicode(false);

            modelBuilder.Entity<MusicRecord>()
                .HasMany(e => e.MusicRecordInDelivery)
                .WithRequired(e => e.MusicRecord)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MusicRecord>()
                .HasMany(e => e.MusicRecordInOrder)
                .WithRequired(e => e.MusicRecord)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MusicRecord>()
                .HasOptional(e => e.MusicRecordsInTheHall)
                .WithRequired(e => e.MusicRecord);

            modelBuilder.Entity<MusicRecordType>()
                .Property(e => e.Title)
                .IsUnicode(false);

            modelBuilder.Entity<MusicRecordType>()
                .HasMany(e => e.MusicRecord)
                .WithRequired(e => e.MusicRecordType)
                .HasForeignKey(e => e.TypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>()
                .HasMany(e => e.MusicRecordInOrder)
                .WithRequired(e => e.Order)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Role>()
                .Property(e => e.Title)
                .IsUnicode(false);

            modelBuilder.Entity<Role>()
                .HasMany(e => e.Employee)
                .WithRequired(e => e.Role)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Supplier>()
                .Property(e => e.Title)
                .IsUnicode(false);

            modelBuilder.Entity<Supplier>()
                .HasMany(e => e.Delivery)
                .WithRequired(e => e.Supplier)
                .WillCascadeOnDelete(false);
        }
    }
}
