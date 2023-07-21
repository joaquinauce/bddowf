using WebAPIAFA.Models;

namespace WebAPIAFA.Repository.IRepository
{
    public interface IPassStepRepository
    {
        public Task<PassStep> GetCurrentPassStep(int idPass);
        public PassStep GetCurrentPassStepSync(int idPass);
        public Task<PassStep> GetPassStep(int idStep);
        public void UpdateSteps(List<PassStep> steps);
        public PassStep GetPassStepSync(int idNextStep);
        public Task CreatePassStep(PassStep passStep);
        public Task<List<PassStep>> GetPassStepsByIdPass(int idPass);
    }
}
