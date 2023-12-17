using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;

namespace AB_Client
{
    internal class Room
    {
        string roomKey;
        bool[] ready;
        bool myReady = false;
        string?[] players;
        Timer updateChecker = new(250);
        int pos = 0;
        ConsoleKeyInfo key = new ConsoleKeyInfo();

        public Room(string roomKey, int players)
        {
            this.roomKey = roomKey;
            ready = new bool[players];
            this.players = new string?[players];
            updateChecker.Elapsed += CheckUpdates;
            updateChecker.AutoReset = true;
            updateChecker.Enabled = true;
            updateChecker.Start();
            Update();
            KeyReader();
        }

        void Update()
        {
            Console.Clear();
            Console.SetCursorPosition(Console.CursorTop, (Console.BufferHeight - 4) / 2);
            Display.FormattedPrintCenter(Locales.Loc["room_key_label"] + roomKey);
            Console.WriteLine();

            if (pos == 0) Console.BackgroundColor = ConsoleColor.DarkGray;
            else Console.BackgroundColor = ConsoleColor.Black;
            Display.FormattedPrintCenter(Locales.Loc["room_deck_selection"]);
            if (pos == 1) Console.BackgroundColor = ConsoleColor.DarkGray;
            else Console.BackgroundColor = ConsoleColor.Black;
            Display.FormattedPrintCenter(Locales.Loc["room_ready_confirm"]);
            if (pos == 2) Console.BackgroundColor = ConsoleColor.DarkGray;
            else Console.BackgroundColor = ConsoleColor.Black;
            Display.FormattedPrintCenter(Locales.Loc["room_start"]);
            Console.BackgroundColor = ConsoleColor.Black;

            for (int i = 0; i < ready.Length; i++)
            {
                if (ready[i])
                    Display.FormattedPrintDCenter("@gV@w " + players[i]);
                else
                    Display.FormattedPrintDCenter("@rX@w " + players[i]);
            }
        }

        public void KeyReader()
        {
            while (updateChecker.AutoReset)
            {
                if (!Console.KeyAvailable) continue;

                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    switch (pos)
                    {
                        case 0:
                            updateChecker.Enabled = false;
                            DeckSelection();
                            updateChecker.Enabled = true;
                            break;
                        case 1:
                            ServerTalker.UpdateReady(roomKey, !myReady);
                            myReady = !myReady;
                            break;
                        case 2:
                            if (!ready.Contains(false))
                            {
                                ServerTalker.StartRoom(roomKey);
                                ServerTalker.StartGame(roomKey, ready.Length);
                            }

                            break;
                    }
                }
                else if ((!ready.Contains(false) && key.Key == ConsoleKey.DownArrow && pos != 2) || (key.Key == ConsoleKey.DownArrow && pos != 1)) pos += 1;
                else if (key.Key == ConsoleKey.UpArrow && pos != 0) pos -= 1;
                key = new ConsoleKeyInfo();
                Thread.Sleep(50);
                Update();
            }
        }

        private void DeckSelection()
        {
            string[] deckfiles = Directory.GetFiles("Decks");
            List<JObject> decks = new List<JObject>();
            foreach (string d in deckfiles)
            {
                decks.Add(JObject.Parse(File.ReadAllText(d)));
            }

            Program.Settings.deck = deckfiles[Display.Prompt(Locales.Loc["select_deck"], decks.Select(x => x["name"].ToString()).ToArray())].Split("\\")[^1];
        }

        private void CheckUpdates(object source, ElapsedEventArgs e)
        {
            var readyinfo = ServerTalker.GetAllReady(roomKey);
            var playersinfo = ServerTalker.GetAllPlayers(roomKey);
            if (ServerTalker.CheckStarted(roomKey))
            {
                updateChecker.AutoReset = false;
                updateChecker.Enabled = false;
            }

            bool changed = false;

            if (readyinfo != null)
                for (int i = 0; i < readyinfo.Length; i++)
                {
                    if (ready[i] != readyinfo[i]) changed = true;
                    ready[i] = readyinfo[i];
                }
            if (playersinfo != null)
                for (int i = 0; i < playersinfo.Length; i++)
                {
                    if (players[i] != playersinfo[i]) changed = true;
                    players[i] = playersinfo[i];
                }
            if (changed) Update();
        }
    }
}
