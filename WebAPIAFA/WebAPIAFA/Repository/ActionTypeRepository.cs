using WebAPIAFA.Entity;
using WebAPIAFA.Models;
using WebAPIAFA.Repository.IRepository;

namespace WebAPIAFA.Repository
{
    public class ActionTypeRepository : IActionTypeRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ActionTypeRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public ActionType GetActionType(int idActionType)
        {
            return dbContext.ActionTypes.FirstOrDefault(aT => aT.IdActionType == idActionType);
        }

        public ICollection<ActionType> GetActionTypes()
        {
            return dbContext.ActionTypes.OrderBy(aT => aT.IdActionType).ToList();
        }
    }
}
