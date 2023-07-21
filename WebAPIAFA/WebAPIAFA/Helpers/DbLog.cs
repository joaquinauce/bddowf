using WebAPIAFA.Entity;
using WebAPIAFA.Models;

namespace WebAPIAFA.Helpers
{
    public class DbLog
    {
        public static bool SaveLog(string errorCode, string platform, string errorDescription, string errorModuleName)
        {
            try
            {
                using ApplicationDbContext context = new();

                ErrorLog newError = new()
                {
                    Description = errorDescription,
                    Module = errorModuleName,
                    ErrorCode = errorCode,
                    Project = platform,
                    CreateDate = DateTime.Now
                };

                context.ErrorLogs.Add(newError);
                context.SaveChanges();

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
