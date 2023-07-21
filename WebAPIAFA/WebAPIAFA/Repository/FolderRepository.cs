using Microsoft.EntityFrameworkCore;
using WebAPIAFA.Entity;
using WebAPIAFA.Models;
using WebAPIAFA.Models.Dtos.FolderDtos;
using WebAPIAFA.Models.Dtos.PlayerDtos;
using WebAPIAFA.Repository.IRepository;

namespace WebAPIAFA.Repository
{
    public class FolderRepository : IFolderRepository
    {
        private readonly ApplicationDbContext dbContext;

        public FolderRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<FolderGetDto> GetFolderGetDto(int idFolder)
        {
            return await dbContext.Folders.Select(f => new FolderGetDto
            {
                IdFolder = f.IdFolder,//se agrega para poder agregar la condicion del firstordefault
                Name = f.Name,
                CreationDate = f.CreationDate,
                UserCuil = f.AffectedUser.Cuil,
                UserName = f.AffectedUser.LastName + " " + f.AffectedUser.Name,
            }).FirstOrDefaultAsync(f => f.IdFolder == idFolder);
        }

        public async Task CreateFolder(Folder folder)
        {
            await dbContext.Folders.AddAsync(folder);
        }

        public async Task<List<FolderTrayGetDto>> GetFolderReceived(int idUser, FolderFilterDto filters)
        {
            List<FolderTrayGetDto> listFolderGeneral = await dbContext.Steps
               .Include(s => s.Folder.AffectedUser).Include(s => s.Folder.AffectedUser.Club)
               .Where(s => s.IdUser == idUser && s.IsCurrent && !s.IsDone)
               .Select(s => new FolderTrayGetDto
               {
                   IdFolder = s.IdFolder,
                   PlayerCUIL = s.Folder.AffectedUser.Cuil,
                   PlayerName = s.Folder.AffectedUser.LastName + " " + s.Folder.AffectedUser.Name,
                   Name = s.Folder.Name,
                   CreationDate = s.Folder.CreationDate,
                   LogoClub = s.Folder.AffectedUser.Club.LogoClub
               }).ToListAsync();

            if (filters.Name != "" && filters.Name != null)
            {
                listFolderGeneral = listFolderGeneral.Where(f => f.Name.ToLower().Contains(filters.Name.ToLower())).ToList();
            }

            if (filters.DateFrom != null)
            {
                listFolderGeneral = listFolderGeneral.Where(f => f.CreationDate.Date >= filters.DateFrom).ToList();
            }

            if(filters.DateUntil != null)
            {
                listFolderGeneral = listFolderGeneral.Where(f => f.CreationDate.Date <= filters.DateUntil).ToList();
            }

            return listFolderGeneral;
        }

        public async Task<List<FolderTrayGetDto>> GetFolderConsult(int idUser, FolderFilterDto filters)
        { 
            List<FolderTrayGetDto> listFolderGeneral = await dbContext.Steps
                .Include(s => s.Folder.AffectedUser).Include(s => s.Folder.AffectedUser.Club)
                .Where(s => s.IdUser == idUser && s.IsDone)
                .Select(s => new FolderTrayGetDto
                {
                    IdFolder = s.IdFolder,
                    PlayerCUIL = s.Folder.AffectedUser.Cuil,
                    PlayerName = s.Folder.AffectedUser.LastName + " " + s.Folder.AffectedUser.Name,
                    Name = s.Folder.Name,
                    CreationDate = s.Folder.CreationDate,
                    LogoClub = s.Folder.AffectedUser.Club.LogoClub
                }).ToListAsync();           

            if (filters.Name != "" && filters.Name != null)
                { 
                    listFolderGeneral = listFolderGeneral.Where(f => f.Name.ToLower().Contains(filters.Name.ToLower())).ToList();
                }

            if (filters.DateFrom != null)
            {
                listFolderGeneral = listFolderGeneral.Where(f => f.CreationDate.Date >= filters.DateFrom).ToList();
            }

            if (filters.DateUntil != null)
            {
                listFolderGeneral = listFolderGeneral.Where(f => f.CreationDate.Date <= filters.DateUntil).ToList();
            }

            return listFolderGeneral;
        }

        public async Task<Folder> GetFolder(int idFolder)
        {
            return await dbContext.Folders.FirstOrDefaultAsync(f => f.IdFolder.Equals(idFolder));
        }

        public async Task<Folder> GetFolderByIdUser(int idUser)
        {
            return await dbContext.Folders.OrderByDescending(f => f.CreationDate).Where(x=>x.CancelationDate==null).FirstOrDefaultAsync(f => f.IdAffectedUser.Equals(idUser));
        }

        public async Task<IList<Folder>> GetFoldersByIdAffectedUser(int idUser)
        {
            return await dbContext.Folders.OrderBy(f => f.CreationDate).Where(f => f.IdAffectedUser.Equals(idUser)).ToListAsync();
        }

        public async Task<IList<Folder>> GetFoldersByIdAffectedUserAndFilters(int idUser, FolderFilterDto filters)
        {
            List<Folder> listFolderGeneral = await dbContext.Folders.OrderBy(f => f.CreationDate).Where(f => f.IdAffectedUser.Equals(idUser)).ToListAsync();

            if (filters.Name != "" && filters.Name != null)
            {
                listFolderGeneral = listFolderGeneral.Where(f => f.Name.ToLower().Contains(filters.Name.ToLower())).ToList();
            }

            if (filters.DateFrom != null)
            {
                listFolderGeneral = listFolderGeneral.Where(f => f.CreationDate.Date >= filters.DateFrom).ToList();
            }

            if (filters.DateUntil != null)
            {
                listFolderGeneral = listFolderGeneral.Where(f => f.CreationDate.Date <= filters.DateUntil).ToList();
            }

            if(filters.IsCurrent != null)
            {
                if(filters.IsCurrent == true)
                {
                    listFolderGeneral = listFolderGeneral
                        .Where(f => ((DateTime)f.EndDate).Date >= DateTime.Now 
                        && (f.ExpireDate == null || ((DateTime)f.ExpireDate).Date >= DateTime.Now)
                        && (f.CancelationDate == null || ((DateTime)f.CancelationDate).Date >= DateTime.Now)).ToList();
                }
                else
                {
                    listFolderGeneral = listFolderGeneral
                        .Where(f => ((DateTime)f.EndDate).Date <= DateTime.Now
                        || (f.ExpireDate != null && ((DateTime)f.ExpireDate).Date <= DateTime.Now)
                        || (f.CancelationDate != null && ((DateTime)f.CancelationDate).Date <= DateTime.Now)).ToList();
                }
            }

            return listFolderGeneral;
        }

        public async Task<Folder> GetLastFolderByIdUser(int idUser)
        {
            return await dbContext.Folders.OrderByDescending(f => f.BeginDate).Where(x => x.BeginDate != null).Where(x => x.EndDate != null).Where(x => x.CancelationDate == null).FirstOrDefaultAsync(f => f.IdAffectedUser.Equals(idUser));
        }

        public async Task<IList<PlayerContractHistoryGetDto>> GetPlayerContractHistory(int idUser)
        {
            return await (from folders in dbContext.Folders
                          join users in dbContext.Users on folders.IdAffectedUser equals users.IdUser
                          join clubs in dbContext.Clubs on folders.IdClub equals clubs.IdClub
                          orderby folders.BeginDate
                          where folders.BeginDate != null && folders.IdAffectedUser.Equals(idUser)
                          select new PlayerContractHistoryGetDto
                          {
                              IdFolder = folders.IdFolder,
                              BeginDate = folders.BeginDate,
                              EndDate = folders.EndDate,
                              PlayerName = users.Name,
                              PlayerLastName = users.LastName,
                              ClubName = clubs.BussinessName,
                              ClubLogo = clubs.LogoClub
                          }).ToListAsync();
        }
    }
}
