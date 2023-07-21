using WebAPIAFA.Models;

namespace WebAPIAFA.Helpers.Menu
{
    public interface IMenu
    {
        public List<MenuOptions> GetMenuOptions(List<string> roles);
    }
}
