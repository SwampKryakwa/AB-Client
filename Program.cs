using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace AB_Client
{
    internal class Program
    {
        public static dynamic Settings;

        static void Main()
        {
            using StreamReader file = File.OpenText("settings.json");
            using JsonTextReader reader = new JsonTextReader(file);
            Settings = (JObject)JToken.ReadFrom(reader);
            Locales.LoadTranslation(Settings["locales"].ToString());
            Console.CursorVisible = false;
            while (true)
                switch (Display.Prompt(Locales.Loc["menu_label"], Locales.Loc["menu_button_play"], Locales.Loc["menu_button_deckbuilding"], Locales.Loc["menu_button_settings"], Locales.Loc["menu_button_exit"]))
                {
                    case 0:
                        Room();
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        Environment.Exit(0);
                        break;
                }
        }

        static void Room()
        {
            string roomKey = "";
            Room room;
            switch (Display.Prompt(Locales.Loc["room_label"], Locales.Loc["room_button_join"], Locales.Loc["room_button_host"], Locales.Loc["button_return"]))
            {
                //Joining a room
                case 0:
                    (bool, string) answer = Display.TextPrompt(Locales.Loc["input_room_key"]);
                    if (!answer.Item1)
                    {
                        break;
                    }
                    roomKey = answer.Item2;
                    while (!ServerTalker.JoinRoom(roomKey))
                    {
                        answer = Display.TextPrompt(Locales.Loc["input_room_key_retry"]);
                        if (!answer.Item1)
                        {
                            break;
                        }
                        roomKey = answer.Item2;
                    }
                    room = new Room(roomKey, 2);
                    break;

                //Creating a room
                case 1:
                    roomKey = ServerTalker.CreateRoom();
                    room = new Room(roomKey, 2);
                    break;

                //Back to menu
                case 2:
                    return;
            }

            long GID = ServerTalker.GetGID(roomKey);
            if (GID == 0) return;
            JObject deck = Deck(Settings["deck"].ToString());
            (int PlayerCount, int PID) = ServerTalker.JoinGame(GID, deck);

            Game game = new(GID, PID, 2);
        }

        static JObject Deck(string deckName)
        {
            using StreamReader file = File.OpenText("Decks/" + deckName);
            using JsonTextReader reader = new JsonTextReader(file);
            return (JObject)JToken.ReadFrom(reader);
        }
    }
}