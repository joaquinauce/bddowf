using Newtonsoft.Json;
using System.Linq;
using System.Text;
using WebAPIAFA.Models;

namespace WebAPIAFA.Helpers.Menu
{
    public class Menu : IMenu
    {
        private readonly IWebHostEnvironment env;

        public Menu(IWebHostEnvironment env)
        {
            this.env = env;
        }
        public List<MenuOptions> GetMenuOptions(List<string> roles)
        {
            string path = env.ContentRootPath;
            path = Path.Combine(path, "Helpers", "Menu", "", "menu.json");
            string json = System.IO.File.ReadAllText(path, Encoding.UTF8);

            //convierto el objeto json en una lista de MenuOption.
            List<MenuOptions> lstMenuOption = JsonConvert.DeserializeObject<List<MenuOptions>>(json);

            //filtro la lista por los que tienen el rol del usuario logueado (si el item no tiene rol configurado, lo devuelve siempre).
            lstMenuOption = lstMenuOption.FindAll(x => x.roles.Count == 0 || x.roles.Intersect(roles).Any());

            //filtro para cada objeto de la lista objects, los hijos con los roles.
            foreach (MenuOptions itemOption in lstMenuOption)
            {
                itemOption.items = itemOption.items.FindAll(x => x.roles.Count == 0 || x.roles.Intersect(roles).Any());
            }

            //devuelvo la lista del men ú.                        
            return lstMenuOption;
        }
    }
}
