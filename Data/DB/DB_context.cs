using Microsoft.EntityFrameworkCore;
using Titan_Fitness.Domain.Entites;
using Titan_Fitness.Domain.Value_object;
using Titan_Fitness.Domain.Enums;

namespace Titan_Fitness.Data.DB
{
    public class DB_context : DbContext
    {
        public DB_context(DbContextOptions<DB_context> options) : base(options) { }

        // DbSets للكيانات المستقلة
        public DbSet<Branch> Branches => Set<Branch>();
        public DbSet<Studio> Studios => Set<Studio>();
        public DbSet<Member> Members => Set<Member>();
        public DbSet<Plan> Plans => Set<Plan>();
        public DbSet<Membership> Memberships => Set<Membership>();
        public DbSet<Freeze> Freezes => Set<Freeze>();
        public DbSet<GuestPass> GuestPasses => Set<GuestPass>();
        public DbSet<CheckIn> CheckIns => Set<CheckIn>();
        public DbSet<Trainer> Trainers => Set<Trainer>();
        public DbSet<ClassSession> ClassSessions => Set<ClassSession>();
        public DbSet<Booking> Bookings => Set<Booking>();

        // 🛑 تم حذف DbSet<Agreed_Terms> لأنه ComplexProperty مدمج داخل Membership

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Branch Constraints
            modelBuilder.Entity<Branch>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Name).IsRequired().HasMaxLength(50);

                entity.ComplexProperty(b => b.Address, a =>
                {
                    a.Property(p => p.Value).HasColumnName("Address").HasMaxLength(200);
                });

                entity.ComplexProperty(b => b.WorkingHours, t =>
                {
                    t.Property(p => p.OpeningTime).HasColumnName("OpeningTime").IsRequired();
                    t.Property(p => p.ClosingTime).HasColumnName("ClosingTime").IsRequired();
                });
            });

            // 2. Studio Constraints
            modelBuilder.Entity<Studio>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name).IsRequired().HasMaxLength(50);
                entity.Property(s => s.Capacity).IsRequired();

                entity.HasOne<Branch>()
                      .WithMany()
                      .HasForeignKey(s => s.BranchId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 3. Member Constraints
            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.MembershipNumber).IsRequired().HasMaxLength(10);
                entity.HasIndex(m => m.MembershipNumber).IsUnique();
                entity.Property(m => m.FullName).IsRequired().HasMaxLength(100);
                entity.Property(m => m.Email).HasMaxLength(100);
                entity.Property(m => m.JoinedDate).IsRequired();

                entity.ComplexProperty(m => m.Phone, p =>
                {
                    p.Property(x => x.Value).HasColumnName("Phone").HasMaxLength(20);
                });

                entity.ComplexProperty(m => m.Address, a =>
                {
                    a.Property(x => x.Value).HasColumnName("Address").HasMaxLength(200);
                });

                entity.HasOne<Branch>()
                      .WithMany()
                      .HasForeignKey(m => m.HomeBranchId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 4. Plan Constraints
            modelBuilder.Entity<Plan>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(50);
                entity.Property(p => p.Price).HasPrecision(18, 2).IsRequired();
                entity.Property(p => p.DurationInMonths).IsRequired();
                entity.Property(p => p.AccessScope).HasConversion<int>().IsRequired();
                entity.Property(p => p.IsPublished).IsRequired();
            });

            // 5. Membership Constraints
            modelBuilder.Entity<Membership>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.PurchaseDate).IsRequired();
                entity.Property(m => m.StartDate).IsRequired();
                entity.Property(m => m.EndDate).IsRequired();
                entity.Property(m => m.Status).HasConversion<int>().IsRequired();

                entity.HasIndex(m => m.Status).HasDatabaseName("IX_Memberships_Status");

                entity.ComplexProperty(m => m.AgreedTerms, terms =>
                {
                    terms.Property(t => t.PricePaid).HasColumnName("AgreedPricePaid").HasPrecision(18, 2).IsRequired();
                    terms.Property(t => t.DurationInMonths).HasColumnName("AgreedDurationInMonths").IsRequired();
                    terms.Property(t => t.MaxFreezeDays).HasColumnName("AgreedMaxFreezeDays");
                    terms.Property(t => t.MaxNumberOfFreezes).HasColumnName("AgreedMaxNumberOfFreezes");
                    terms.Property(t => t.GuestPassQuota).HasColumnName("AgreedGuestPassQuota");
                    terms.Property(t => t.AccessScope).HasColumnName("AgreedAccessScope").HasConversion<int>().IsRequired();
                });

                entity.HasOne<Member>()
                      .WithMany()
                      .HasForeignKey(m => m.MemberId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Plan>()
                      .WithMany()
                      .HasForeignKey(m => m.PlanId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 7. Freeze Constraints
            modelBuilder.Entity<Freeze>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Property(f => f.StartDate).IsRequired();
                entity.Property(f => f.EndDate).IsRequired();
                entity.Property(f => f.DurationInMonths).IsRequired();
                entity.Property(f => f.Reason).HasConversion<int>().IsRequired();
                entity.Property(f => f.AdditionalNotes).HasMaxLength(200);
                entity.Property(f => f.RequestedOn).IsRequired();

                entity.HasOne<Membership>()
                      .WithMany()
                      .HasForeignKey(f => f.MembershipId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 8. Guest Pass Constraints
            modelBuilder.Entity<GuestPass>(entity =>
            {
                entity.HasKey(g => g.Id);
                entity.Property(g => g.IssuedOn).IsRequired();
                entity.Property(g => g.GuestName).HasMaxLength(100);

                entity.HasOne<Membership>()
                      .WithMany()
                      .HasForeignKey(g => g.MembershipId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 9. Check-In Constraints
            modelBuilder.Entity<CheckIn>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.CheckInDateTime).IsRequired();
                entity.Property(c => c.Result).HasConversion<int>().IsRequired();
                entity.Property(c => c.RefusalReason).HasMaxLength(100);

                entity.HasIndex(c => c.CheckInDateTime).HasDatabaseName("IX_CheckIns_CheckInDateTime");

                entity.HasOne<Member>()
                      .WithMany()
                      .HasForeignKey(c => c.MemberId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Branch>()
                      .WithMany()
                      .HasForeignKey(c => c.BranchId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 10. Trainer Constraints
            modelBuilder.Entity<Trainer>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Name).IsRequired().HasMaxLength(100);
                entity.Property(t => t.Email).HasMaxLength(100);
                entity.Property(t => t.IsActive).IsRequired();

                entity.HasIndex(t => t.IsActive).HasDatabaseName("IX_Trainers_IsActive");

                entity.ComplexProperty(t => t.Phone, p =>
                {
                    p.Property(x => x.Value).HasColumnName("Phone").HasMaxLength(20);
                });
            });

            // 11. Class Session Constraints
            modelBuilder.Entity<ClassSession>(entity =>
            {
                entity.HasKey(cs => cs.Id);
                entity.Property(cs => cs.ClassName).IsRequired().HasMaxLength(100);
                entity.Property(cs => cs.SessionDate).IsRequired();
                entity.Property(cs => cs.StartTime).IsRequired();
                entity.Property(cs => cs.DurationInMinutes).IsRequired();
                entity.Property(cs => cs.CapacityLimit).IsRequired();
                entity.Property(cs => cs.Status).HasConversion<int>().IsRequired();
                entity.Property(cs => cs.Description).HasMaxLength(500);

                entity.HasIndex(cs => cs.SessionDate).HasDatabaseName("IX_ClassSessions_SessionDate");

                entity.HasOne<Branch>().WithMany().HasForeignKey(cs => cs.BranchId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<Studio>().WithMany().HasForeignKey(cs => cs.StudioId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<Trainer>().WithMany().HasForeignKey(cs => cs.TrainerId).OnDelete(DeleteBehavior.Restrict);
            });

            // 12. Booking Constraints
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.BookedOn).IsRequired();
                entity.Property(b => b.Status).HasConversion<int>().IsRequired();
                entity.Property(b => b.NotesForTrainer).HasMaxLength(500);

                entity.HasOne<ClassSession>().WithMany().HasForeignKey(b => b.SessionId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<Member>().WithMany().HasForeignKey(b => b.MemberId).OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}