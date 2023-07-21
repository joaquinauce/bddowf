using Microsoft.EntityFrameworkCore;
using WebAPIAFA.Models;

namespace WebAPIAFA.Entity
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {

        }
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        //mantener orden alfabetico
        public DbSet<ActionType> ActionTypes { get; set; }
        public DbSet<Bulletin> Bulletins { get; set; }
        public DbSet<BulletinSubscriber> BulletinSubscribers { get; set; }
        public DbSet<BulletinType> BulletinTypes { get; set; }
        public virtual DbSet<Club> Clubs { get; set; }
        public DbSet<ClubAuthority> ClubAuthorities { get; set; }
        public DbSet<ClubFile> ClubFiles { get; set; }
        public DbSet<ClubFileType> ClubFileTypes { get; set; }
        public DbSet<ClubInformation> ClubInformation { get; set; }
        public DbSet<ClubMandate> ClubMandates { get; set; }
        public DbSet<ClubSponsor> ClubSponsors { get; set; }
        public DbSet<ClubStaff> ClubStaffs { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; } = null!;
    }
}
