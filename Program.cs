using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace AB_Client
{
    internal class Program
    {
        public const string VERSION = "Alpha 3";

        public static dynamic Settings;

        static void Main()
        {
            try
            {
                using StreamReader file = File.OpenText("settings.json");
                using JsonTextReader reader = new JsonTextReader(file);
                Settings = (JObject)JToken.ReadFrom(reader);
                Locales.LoadTranslation(Settings["locales"].ToString());
                Console.CursorVisible = false;
                Display.Init();
                while (true)
                    switch (Display.SelectionPrompt(Locales.Loc["menu_label"], Locales.Loc["menu_button_play"], Locales.Loc["menu_button_deckbuilding"], Locales.Loc["menu_button_settings"], Locales.Loc["menu_button_exit"]))
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
            catch (Exception e)
            {
                if (!Directory.Exists("error_logs")) Directory.CreateDirectory("error_logs");
                File.WriteAllText($"error_logs/log-{DateTime.Now.ToString().Replace(':', '-')}.txt", e.ToString());
                Display.Error = e;
                Display.DrawError();
                Console.ReadKey(false);
            }
        }

        static void Room()
        {
            string roomKey = "";
            Room room;
            switch (Display.SelectionPrompt(Locales.Loc["room_label"], Locales.Loc["room_button_join"], Locales.Loc["room_button_host"], Locales.Loc["button_return"]))
            {
                //Joining a room
                case 0:
                    (bool, string) answer = Display.TextPrompt(Locales.Loc["input_room_key"], 8);
                    if (!answer.Item1)
                    {
                        return;
                    }
                    roomKey = answer.Item2;
                    while (!ServerTalker.JoinRoom(roomKey))
                    {
                        answer = Display.TextPrompt(Locales.Loc["input_room_key_retry"], 8);
                        if (!answer.Item1)
                        {
                            return;
                        }
                        roomKey = answer.Item2;
                    }
                    room = new Room(roomKey, 2);
                    if (!room.IsMember) return;
                    break;

                //Creating a room
                case 1:
                    roomKey = ServerTalker.CreateRoom(2);
                    room = new Room(roomKey, 2);
                    if (!room.IsMember)
                    {
                        Console.Clear();
                        Console.WriteLine("bruh");
                        Console.ReadKey();
                        return;
                    }
                    break;

                //Back to menu
                case 2:
                    return;
            }

            long GID = ServerTalker.GetGID(roomKey);
            if (GID == 0)
            {
                Console.Clear();
                Console.WriteLine("bruh");
                Console.ReadKey();
                return;
            }
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