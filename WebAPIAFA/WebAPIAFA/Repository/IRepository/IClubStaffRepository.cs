using WebAPIAFA.Models;

namespace WebAPIAFA.Repository.IRepository
{
    public interface IClubStaffRepository
    {
        public Task<IList<ClubStaff>> GetClubStaffs();
        public Task<bool> ClubStaffExistId(int idClubStaff);
        public Task<ClubStaff> GetClubStaff(string name);
        public Task<ClubStaff> GetClubStaff(int idClubStaff);


    }
}
