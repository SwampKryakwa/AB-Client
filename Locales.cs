using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AB_Client
{
    internal static class Locales
    {
        public static Dictionary<string, string> Loc = new();

        public static void LoadTranslation(string file)
        {
            Loc = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("Locales/" + file));
        }
    }
}
