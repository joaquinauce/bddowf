using WebAPIAFA.Models;

namespace WebAPIAFA.Repository.IRepository
{
    public interface IActionTypeRepository
    {
        public ICollection<ActionType> GetActionTypes();
        public ActionType GetActionType(int idActionType);        
    }
}
