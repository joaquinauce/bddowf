using Microsoft.EntityFrameworkCore;
using WebAPIAFA.Entity;
using WebAPIAFA.Models;
using WebAPIAFA.Repository.IRepository;

namespace WebAPIAFA.Repository
{
    public class ClubStaffRepository: IClubStaffRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ClubStaffRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IList<ClubStaff>> GetClubStaffs()
        {
            return await dbContext.ClubStaffs.OrderBy(g => g.IdClubStaff).ToListAsync();
        }

        public async Task<ClubStaff> GetClubStaff(string name)
        {
            return await dbContext.ClubStaffs.FirstOrDefaultAsync(g => g.Name.Equals(name));
        }

        public async Task<ClubStaff> GetClubStaff(int idClubStaff)
        {
            return await dbContext.ClubStaffs.FirstOrDefaultAsync(g => g.IdClubStaff.Equals(idClubStaff));
        }

        public async Task<bool> ClubStaffExistId(int idClubStaff)
        {
            return await dbContext.ClubStaffs.AnyAsync(u => u.IdClubStaff == idClubStaff);
        }
    }
}

