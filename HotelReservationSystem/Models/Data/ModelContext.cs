using Microsoft.EntityFrameworkCore;

namespace HotelReservationSystem.Models.Data
{
    //class=>db
    //ctor=>options
    //
    public class ModelContext : DbContext
    {
        public ModelContext(DbContextOptions<ModelContext> options) : base(options)//options
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Testimonial>()
    .HasOne(t => t.room)
    .WithMany(r => r.testimonials)
    .HasForeignKey(t => t.roomId)
    .OnDelete(DeleteBehavior.NoAction); // or .OnDelete(DeleteBehavior.NoAction)


            base.OnModelCreating(modelBuilder);
        }

        //dbSet
        // BankAccount-Hotel-PageContent-Rev-Re-Roo-Tes-UserTR
        public DbSet<User> users { get; set; }
        public DbSet<BankAccount> bankAccounts { get; set; }
        public DbSet<Hotel> hotels { get; set; }
        public DbSet<PageContent> pageContent { get; set; }
        public DbSet<Reservation> reservations { get; set; }
        public DbSet<Resident> residents { get; set; }
        public DbSet<Room> room { get; set; }
        public DbSet<Testimonial> testimonials { get; set; }
        public DbSet<UserTransactions> userTransactions { get; set; }
        public DbSet<ContactUs> contactUs { get; set; }

    }
}
