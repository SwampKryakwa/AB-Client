using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Data;
using System.Runtime.InteropServices;
using System.Timers;
using static System.Collections.Specialized.BitVector32;

namespace AB_Client
{
    internal static class Display
    {
        public static char[] alphabet = new char[]
        {
            'A',
            'B',
            'C',
            'D',
            'E',
            'F',
            'G',
            'H',
            'I',
            'J',
            'K',
            'L',
            'M',
            'N',
            'O',
            'P',
            'Q',
            'S',
            'T',
            'U',
            'V',
            'W',
            'X',
            'Y',
            'Z'
        };

        public static Game Game;
        public static string CurrentEffect;

        public static (int X, int Y) ScreenSize = (Console.BufferWidth, Console.BufferHeight);
        static ConsoleKeyInfo? CurrentKey;
        public static List<string> activeAbilities = new();
        public static Exception Error;

        public static string? IOType;
        public static string Prompt;
        static string[] options;
        static int CurrentOption;
        static (int X, int Y) CurrentGate = (-1, -1);
        static bool inputting = true;
        static string currentInput;

        static string RoomKey;
        public static string[] UserNames;
        public static bool[] UsersReady;
        public static bool RoomStarted;

        public static bool Updating = false;
        public static bool Drawing = false;

        public static List<string> Events = new();

        public static System.Timers.Timer UpdateListener = new(50);

        public static void ScreenUpdateCheck(object? sender, ElapsedEventArgs args)
        {
            if (ScreenSize != (Console.BufferWidth, Console.BufferHeight))
            {
                ScreenSize = (Console.BufferWidth, Console.BufferHeight);
                FullRedraw();
            }
        }

        public static void ReadIO(object? sender, ElapsedEventArgs args)
        {
            if (Console.KeyAvailable)
            {
                CurrentKey = Console.ReadKey(true);
            }
        }

        public static void Init()
        {
            UpdateListener.Elapsed += ScreenUpdateCheck;
            UpdateListener.Elapsed += ReadIO;
            UpdateListener.AutoReset = true;
            UpdateListener.Enabled = true;
            FullRedraw();
        }

        static void AlignedPrint(string toPrint, int X, ref int Y)
        {
            Console.SetCursorPosition(X, Y);
            int a = 0;
            foreach (string line in toPrint.Split('\n'))
            {
                foreach (string word in line.Split('@'))
                {
                    if (a != 0)
                    {
                        bool hasBg = false;
                        switch (word[0])
                        {
                            case 'r':
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;
                            case 'g':
                                Console.ForegroundColor = ConsoleColor.Green;
                                break;
                            case 'b':
                                Console.ForegroundColor = ConsoleColor.Blue;
                                break;
                            case 'c':
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                break;
                            case 'y':
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                break;
                            case 'm':
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                break;
                            case 'l':
                                Console.ForegroundColor = ConsoleColor.Black;
                                break;
                            case 'w':
                                Console.ForegroundColor = ConsoleColor.White;
                                break;
                            case 'd':
                                switch (word[1])
                                {
                                    case 'r':
                                        Console.ForegroundColor = ConsoleColor.DarkRed;
                                        break;
                                    case 'g':
                                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                                        break;
                                    case 'b':
                                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                                        break;
                                    case 'c':
                                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                                        break;
                                    case 'y':
                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                        break;
                                    case 'm':
                                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                                        break;
                                }
                                break;
                            case 'u':
                                hasBg = true;
                                switch (word[1])
                                {
                                    case 'g':
                                        Console.BackgroundColor = ConsoleColor.DarkGray;
                                        break;
                                    case 'b':
                                        Console.BackgroundColor = ConsoleColor.Black;
                                        break;
                                }
                                break;
                        }
                        if (hasBg)
                            Console.Write(word[2..]);
                        else
                            Console.Write(word[1..]);
                    }
                    else
                        Console.Write(word);
                    a++;
                }

                Y++;
                Console.SetCursorPosition(X, Y);
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        static void AlignedPrint(string toPrint, int X, int Y)
        {
            Console.SetCursorPosition(X, Y);
            int a = 0;
            foreach (string line in toPrint.Split('\n'))
            {
                foreach (string word in line.Split('@'))
                {
                    if (a != 0)
                    {
                        bool hasBg = false;
                        switch (word[0])
                        {
                            case 'r':
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;
                            case 'g':
                                Console.ForegroundColor = ConsoleColor.Green;
                                break;
                            case 'b':
                                Console.ForegroundColor = ConsoleColor.Blue;
                                break;
                            case 'c':
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                break;
                            case 'y':
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                break;
                            case 'm':
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                break;
                            case 'l':
                                Console.ForegroundColor = ConsoleColor.Black;
                                break;
                            case 'w':
                                Console.ForegroundColor = ConsoleColor.White;
                                break;
                            case 'd':
                                switch (word[1])
                                {
                                    case 'r':
                                        Console.ForegroundColor = ConsoleColor.DarkRed;
                                        break;
                                    case 'g':
                                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                                        break;
                                    case 'b':
                                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                                        break;
                                    case 'c':
                                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                                        break;
                                    case 'y':
                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                        break;
                                    case 'm':
                                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                                        break;
                                }
                                break;
                            case 'u':
                                hasBg = true;
                                switch (word[1])
                                {
                                    case 'g':
                                        Console.BackgroundColor = ConsoleColor.DarkGray;
                                        break;
                                    case 'b':
                                        Console.BackgroundColor = ConsoleColor.Black;
                                        break;
                                }
                                break;
                        }
                        if (hasBg)
                            Console.Write(word[2..]);
                        else
                            Console.Write(word[1..]);
                    }
                    else
                        Console.Write(word);
                    a++;
                }

                Console.SetCursorPosition(X, Y);
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        static void ClearBlock(int X1, int Y1, int X2, int Y2)
        {
            int width = X2 - X1;

            for (int i = Y1; i <= Y2; i++)
            {
                Console.SetCursorPosition(X1, i);
                Console.Write(new string(' ', width));
            }
        }

        public static string WrapFormat(string text, int lineLength)
        {
            string wrapped = "";

            foreach (string line in text.Split('\n'))
            {
                while (true)
                {
                    if (text.Length > lineLength)
                    {
                        wrapped += text;
                        break;
                    }
                    wrapped += text[..(lineLength - 1)];
                    text = text[(lineLength - 1)..];
                }
                wrapped += '\n';
            }

            return wrapped[..^1];
        }

        public static void FullRedraw()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            DrawVersion(); DrawField(); DrawEffects();
            DrawInfo(); DrawIO(); DrawEvents();
            DrawError();
        }

        public static void DrawField()
        {
            if (Game == null) return;
            int X1 = Console.BufferWidth / 3 + 1;
            int Y1 = 0;
            int X2 = Console.BufferWidth - X1 - 1;
            int Y2 = Console.BufferHeight / 3;
            int line = Y1 + Game.Field.GetLength(0);

            ClearBlock(X1, Y1, X2, Y2);

            if (Game != null)
            {
                string toPrint;
                Console.CursorTop = 0;
                for (int i = 0; i < Game.Field.GetLength(0); i++)
                {
                    Console.CursorLeft = (Console.BufferWidth - (Game.Field.GetLength(1) * 3) - 1) / 2;
                    for (int j = 0; j < Game.Field.GetLength(1); j++)
                    {
                        if (Game.Field[i, j] != null)
                            if (Game.Field[i, j].Owner == 0) Console.ForegroundColor = ConsoleColor.Cyan;
                            else if (Game.Field[i, j].Owner == 1) Console.ForegroundColor = ConsoleColor.Red;

                        if ((i, j) == CurrentGate) Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.Write($"{alphabet[j]}{i + 1}");
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(' ');
                    }
                    Console.WriteLine();
                }

                for (int i = 0; i < Game.Field.GetLength(0); i++)
                {
                    for (int j = 0; j < Game.Field.GetLength(1); j++)
                    {
                        if (Game.Field[i, j].Bakugans.Count != 0)
                        {
                            toPrint = $"{alphabet[j]}{i + 1}: ";
                            for (int k = 0; k < Game.Field[i, j].Bakugans.Count; k++)
                            {
                                Bakugan bakugan = Game.Field[i, j].Bakugans[k];
                                if (bakugan.Owner == 0) toPrint += "@c";
                                else if (bakugan.Owner == 1) toPrint += "@r";
                                toPrint += $"{bakugan.GetFullName()}";
                                if (k != Game.Field[i, j].Bakugans.Count - 1)
                                {
                                    toPrint += "@w, ";
                                }
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                            if (Game.Field[i, j].ActiveBattle) toPrint += (" - идёт бой");
                            else if (Game.Field[i, j].BattleFrozen) toPrint += (" - битва заморожена");
                            AlignedPrint(toPrint, (Console.BufferWidth - (toPrint.Length)) / 2 + toPrint.Count(x => x == '@'), ref line);
                        }
                    }
                }
            }
        }

        public static void DrawVersion()
        {
            ClearBlock(0, 0, ScreenSize.X / 3, ScreenSize.Y / 3);
            Console.SetCursorPosition(0, 0);
            Console.Write(Program.VERSION);
        }

        public static void DrawEffects()
        {
            ClearBlock(ScreenSize.X * 2 / 3, 0, ScreenSize.X, ScreenSize.Y / 3);
            for (int i = 0; i < activeAbilities.Count; i++)
            {
                Console.SetCursorPosition(Console.BufferWidth - activeAbilities[i].Length, i);
                Console.Write(Locales.Loc[activeAbilities[i]]);
            }
        }

        public static void DrawInfo()
        {
            ClearBlock(0, ScreenSize.Y / 3, ScreenSize.X / 3, ScreenSize.Y * 2 / 3);
            if (CurrentEffect == null) return;
            string toPrint = WrapFormat(Locales.Loc[CurrentEffect], Console.BufferWidth / 3);
            int height = toPrint.Count(x => x == '\n');
            AlignedPrint(toPrint, 0, (Console.BufferHeight - height) / 2);
        }

        public static void DrawError()
        {
            if (Error == null) return;
            UpdateListener.Stop();

            Console.Clear();
            Console.WriteLine(Error);
        }

        public static void DrawEvents()
        {
            if (Game == null) return;

            int bottom = ScreenSize.Y / 3 * 2;
            ClearBlock(ScreenSize.X * 2 / 3, ScreenSize.Y / 3, ScreenSize.X, ScreenSize.Y * 2 / 3);

            int maxWidth = ScreenSize.X / 3;

            List<string> lines = new();
            foreach (string e in Events)
            {
                if (e.Length >= maxWidth)
                {
                    string[] oldEvent = e.Split(' ');
                    string newEventLine = "";
                    foreach (string w in oldEvent)
                    {
                        if ((newEventLine.Length + w.Length) > maxWidth)
                        {
                            lines.Add(newEventLine.Trim(' '));
                            newEventLine = "";
                        }
                        newEventLine += w + " ";
                    }
                    lines.Add(newEventLine.Trim(' '));
                }
                else lines.Add(e);
            }

            Console.CursorTop = ScreenSize.Y / 3;
            Console.CursorLeft = ScreenSize.X - Locales.Loc["display_label_log"].Length;
            Console.Write(Locales.Loc["display_label_log"]);
            int cutoff = Math.Min(ScreenSize.Y / 3 - 1, lines.Count);

            Console.CursorTop = bottom;
            for (int i = 0; i < cutoff; i++)
            {
                Console.CursorLeft = Console.BufferWidth - lines[^(i + 1)].Length;
                Console.Write(lines[^(i + 1)]);
                Console.CursorTop--;
            }
        }

        public static void DrawIO()
        {
            Console.ForegroundColor = ConsoleColor.White;
            ClearBlock(ScreenSize.X / 3, ScreenSize.Y / 3, ScreenSize.X * 2 / 3, ScreenSize.Y * 2 / 3);

            if (IOType == null) return;

            switch (IOType)
            {
                case "choice":
                    DrawChoiceIO();
                    return;
                case "gate":
                    DrawWaitIO();
                    DrawField();
                    return;
                case "text":
                    DrawTextIO();
                    return;
                case "wait":
                    DrawWaitIO();
                    return;
                case "room":
                    DrawRoomIO();
                    return;
            }
        }

        public static void DrawWaitIO()
        {
            int maxWidth = Console.BufferWidth / 3;

            List<string> lines = new();
            if (Prompt.Length >= maxWidth)
            {
                string[] oldPrompt = Prompt.Split(' ');
                string newPromptLine = "";
                foreach (string w in oldPrompt)
                {
                    if (newPromptLine.Length + w.Length > maxWidth)
                    {
                        lines.Add(newPromptLine.Trim(' ') + "\n");
                        newPromptLine = "";
                    }
                    newPromptLine += w + " ";
                }
                lines.Add(newPromptLine.Trim(' '));
            }
            else lines.Add(Prompt);

            for (int i = 0; i < lines.Count; i++)
            {
                Console.SetCursorPosition((Console.BufferWidth - lines[i].Length) / 2, (Console.BufferHeight - options.Length) / 2 + i);
                Console.Write(lines[i]);
            }
        }

        public static void DrawChoiceIO()
        {
            int maxWidth = Console.BufferWidth / 3;

            List<string> lines = new();
            if (Prompt.Length >= maxWidth)
            {
                string[] oldPrompt = Prompt.Split(' ');
                string newPromptLine = "";
                foreach (string w in oldPrompt)
                {
                    if (newPromptLine.Length + w.Length > maxWidth)
                    {
                        lines.Add(newPromptLine.Trim(' ') + "\n");
                        newPromptLine = "";
                    }
                    newPromptLine += w + " ";
                }
                lines.Add(newPromptLine.Trim(' '));
            }
            else lines.Add(Prompt);

            for (int i = 0; i < lines.Count; i++)
            {
                Console.SetCursorPosition((Console.BufferWidth - lines[i].Length) / 2, (Console.BufferHeight - options.Length) / 2 - lines.Count + i);
                Console.Write(lines[i]);
            }
            for (int i = 0; i < options.Length; i++)
            {
                if (CurrentOption == i) Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.SetCursorPosition((Console.BufferWidth - options[i].Length) / 2, (Console.BufferHeight - options.Length) / 2 + i);
                Console.Write(options[i]);
                if (CurrentOption == i) Console.BackgroundColor = ConsoleColor.Black;
            }
        }

        public static void DrawTextIO()
        {
            int maxWidth = Console.BufferWidth / 3;

            List<string> lines = new();
            if (Prompt.Length >= maxWidth)
            {
                string[] oldPrompt = Prompt.Split(' ');
                string newPromptLine = "";
                foreach (string w in oldPrompt)
                {
                    if (newPromptLine.Length + w.Length > maxWidth)
                    {
                        lines.Add(newPromptLine.Trim(' ') + "\n");
                        newPromptLine = "";
                    }
                    newPromptLine += w + " ";
                }
                lines.Add(newPromptLine.Trim(' '));
            }
            else lines.Add(Prompt);

            for (int i = 0; i < lines.Count; i++)
            {
                Console.SetCursorPosition((Console.BufferWidth - lines[i].Length) / 2, (Console.BufferHeight - options.Length) / 2 - lines.Count + i);
                Console.Write(lines[i]);
            }
            //Console.SetCursorPosition((Console.BufferWidth - Prompt.Length) / 2, (Console.BufferHeight - options.Length) / 2 - 1);
            //Console.Write(Prompt);
            if (inputting)
            {
                Console.BackgroundColor = ConsoleColor.DarkGray;
                if (currentInput.Length == 0)
                {
                    Console.SetCursorPosition((Console.BufferWidth - 11) / 2, (Console.BufferHeight - options.Length) / 2);
                    Console.Write(new string(' ', 11));
                }
            }
            Console.SetCursorPosition((Console.BufferWidth - currentInput.Length) / 2, (Console.BufferHeight - options.Length) / 2);
            Console.Write(currentInput);
            if (inputting) Console.BackgroundColor = ConsoleColor.Black;
            else Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.SetCursorPosition((Console.BufferWidth - Locales.Loc["button_return"].Length) / 2, (Console.BufferHeight - options.Length) / 2 + 1);
            Console.Write(Locales.Loc["button_return"]);
            if (!inputting) Console.BackgroundColor = ConsoleColor.Black;
        }

        public static void DrawRoomIO()
        {
            while (Updating) ;
            Drawing = true;

            ClearBlock(ScreenSize.X / 3, ScreenSize.Y / 3, ScreenSize.X * 2 / 3, ScreenSize.Y * 2 / 3);
            Console.CursorTop = Console.BufferHeight / 2 - 2;
            Console.CursorLeft = (Console.BufferWidth - (Locales.Loc["room_key_label"] + RoomKey).Length) / 2;
            Console.WriteLine(Locales.Loc["room_key_label"] + RoomKey);

            if (CurrentOption == 0) Console.BackgroundColor = ConsoleColor.DarkGray;
            else Console.BackgroundColor = ConsoleColor.Black;
            Console.CursorLeft = (Console.BufferWidth - Locales.Loc["room_deck_selection"].Length) / 2;
            Console.WriteLine(Locales.Loc["room_deck_selection"]);

            if (CurrentOption == 1) Console.BackgroundColor = ConsoleColor.DarkGray;
            else Console.BackgroundColor = ConsoleColor.Black;
            Console.CursorLeft = (Console.BufferWidth - Locales.Loc["room_ready_confirm"].Length) / 2;
            Console.WriteLine(Locales.Loc["room_ready_confirm"]);

            if (CurrentOption == 2) Console.BackgroundColor = ConsoleColor.DarkGray;
            else Console.BackgroundColor = ConsoleColor.Black;
            Console.CursorLeft = (Console.BufferWidth - Locales.Loc["room_start"].Length) / 2;
            Console.WriteLine(Locales.Loc["room_start"]);

            Console.BackgroundColor = ConsoleColor.Black;
            for (int i = 0; i < UsersReady.Length; i++)
            {
                Console.SetCursorPosition((Console.BufferWidth - (UserNames[i] + "  ").Length) / 2 - 1, Console.CursorTop);
                if (UsersReady[i])
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("V ");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("X ");
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(UserNames[i]);
            }

            Console.WriteLine();
            if (CurrentOption == 3) Console.BackgroundColor = ConsoleColor.DarkGray;
            else Console.BackgroundColor = ConsoleColor.Black;
            Console.CursorLeft = (Console.BufferWidth - Locales.Loc["menu_button_exit"].Length) / 2;
            Console.WriteLine(Locales.Loc["menu_button_exit"]);
            Console.BackgroundColor = ConsoleColor.Black;
            Drawing = false;
        }

        public static string GetFullName(dynamic bakugan)
        {
            return Locales.Loc[bakugan["attribute"]] + " " + Locales.Loc[bakugan["name"]] + ", " + Locales.Loc[bakugan["power"]];
        }

        public static void Wait(string msg)
        {
            Prompt = msg;
            IOType = "wait";
        }

        public static bool BoolPrompt(string prompt)
        {
            Prompt = prompt;
            options = [Locales.Loc["answer_yes"], Locales.Loc["answer_no"]];
            CurrentOption = 0;
            IOType = "choice";
            DrawIO();
            while (true)
            {
                if (CurrentKey == null) continue;
                if (CurrentKey?.Key == ConsoleKey.Enter) break;
                else if (CurrentKey?.Key == ConsoleKey.DownArrow && CurrentOption < options.Length) CurrentOption++;
                else if (CurrentKey?.Key == ConsoleKey.UpArrow && CurrentOption > 0) CurrentOption--;
                DrawIO();
                CurrentKey = null;
            }
            CurrentKey = null;
            IOType = null;
            return CurrentOption == 0;
        }

        public static (bool, string) TextPrompt(string prompt, int maxLength)
        {
            Prompt = prompt;
            currentInput = "";
            IOType = "text";
            DrawIO();
            while (true)
            {
                if (CurrentKey == null) continue;

                if (!inputting && CurrentKey?.Key == ConsoleKey.UpArrow) inputting = true;
                else if (!inputting && CurrentKey?.Key == ConsoleKey.Enter) return (false, "");
                else if (!inputting) continue;
                else if (CurrentKey?.Key == ConsoleKey.Enter) break;
                else if (inputting && CurrentKey?.Key == ConsoleKey.DownArrow) inputting = false;
                else if (CurrentKey?.Key == ConsoleKey.Backspace && currentInput.Length != 0) currentInput = currentInput[..^1];
                else if (CurrentKey?.Modifiers == ConsoleModifiers.Control && CurrentKey?.Key == ConsoleKey.V) currentInput += TextCopy.ClipboardService.GetText();
                else if (currentInput.Length < maxLength && !char.IsControl((char)CurrentKey?.KeyChar))
                    currentInput += CurrentKey?.KeyChar;

                DrawIO();
                CurrentKey = null;
            }
            CurrentKey = null;
            IOType = null;
            return (true, currentInput);
        }

        public static bool RoomPrompt(string roomKey, int playerCount)
        {
            IOType = "room";
            RoomKey = roomKey;
            CurrentOption = 0;
            UserNames = new string[playerCount];
            UsersReady = new bool[playerCount];
            bool meReady = false;
            RoomStarted = false;
            DrawIO();

            for (int i = 0; i < playerCount; i++)
            {
                UserNames[i] = "";
                UsersReady[i] = false;
            }

            while (true)
            {
                if (RoomStarted) return true;
                if (CurrentKey == null) continue;

                if (CurrentKey?.Key == ConsoleKey.DownArrow && CurrentOption < 3)
                {
                    if (++CurrentOption == 2 && UsersReady.Contains(false)) CurrentOption++;
                }
                else if (CurrentKey?.Key == ConsoleKey.UpArrow && CurrentOption > 0)
                {
                    if (--CurrentOption == 2 && UsersReady.Contains(false)) CurrentOption--;
                }
                else if (CurrentKey?.Key == ConsoleKey.Enter) switch (CurrentOption)
                    {
                        case 0:
                            int thisSelection = CurrentOption;
                            DeckSelection();
                            CurrentOption = thisSelection;
                            IOType = "room";
                            break;
                        case 1:
                            meReady = !meReady;
                            ServerTalker.UpdateReady(roomKey, meReady);
                            break;
                        case 2:
                            ServerTalker.StartRoom(roomKey);
                            ServerTalker.StartGame(roomKey, playerCount);
                            break;
                        case 3:
                            return false;
                    }

                CurrentKey = null;
                DrawIO();
            }
        }

        public static void DeckSelection()
        {
            string[] deckfiles = Directory.GetFiles("Decks");
            List<JObject> decks = new List<JObject>();
            foreach (string d in deckfiles)
            {
                decks.Add(JObject.Parse(File.ReadAllText(d)));
            }

            Console.Clear();
            Console.WriteLine("huh");
            Console.ReadKey(true);

            Program.Settings.deck = deckfiles[SelectionPrompt(Locales.Loc["select_deck"], decks.Select(x => x["name"].ToString()).ToArray())].Split("\\")[^1];
        }

        public static int SelectionPrompt(string prompt, params string[] options)
        {
            Prompt = prompt;
            Display.options = options;
            CurrentOption = 0;
            IOType = "choice";
            DrawIO();
            while (true)
            {
                if (CurrentKey == null) continue;
                if (CurrentKey?.Key == ConsoleKey.Enter) break;
                else if (CurrentKey?.Key == ConsoleKey.DownArrow && CurrentOption < (options.Length - 1)) CurrentOption++;
                else if (CurrentKey?.Key == ConsoleKey.UpArrow && CurrentOption > 0) CurrentOption--;
                else if ((int)CurrentKey?.Key > 48 && (int)CurrentKey?.Key < 58) CurrentOption = (int)CurrentKey?.Key - 49;
                else if ((int)CurrentKey?.Key > 96 && (int)CurrentKey?.Key < 106) CurrentOption = (int)CurrentKey?.Key - 97;
                DrawIO();
                CurrentKey = null;
            }
            CurrentKey = null;
            IOType = null;
            return CurrentOption;
        }

        public static (int X, int Y) GatePrompt(string prompt, bool selectFilled)
        {
            Prompt = prompt;
            CurrentGate = (0, 0);
            IOType = "gate";

            /*while ((Game.Field[CurrentGate.X, CurrentGate.Y].Owner == -1) == selectFilled)
            {
                CurrentGate.X++;
                if (CurrentGate.X == Game.Field.GetLength(0))
                {
                    CurrentGate.X = 0;
                    CurrentGate.Y++;
                }
            }*/
            DrawIO();

            while (true)
            {
                if (CurrentKey == null) continue;
                if (CurrentKey?.Key == ConsoleKey.Enter) break;
                else if (CurrentKey?.Key == ConsoleKey.DownArrow && CurrentGate.X < Game.Field.GetLength(0) - 1)
                {
                    do
                    {
                        if (++CurrentGate.X == Game.Field.GetLength(0))
                        {
                            CurrentGate.X = 0;
                            CurrentGate.Y++;
                            if (CurrentGate.Y == Game.Field.GetLength(1))
                            {
                                CurrentGate.Y = 0;
                            }
                        }
                    }
                    while ((Game.Field[CurrentGate.X, CurrentGate.Y].Owner == -1) == selectFilled);
                }
                else if (CurrentKey?.Key == ConsoleKey.UpArrow && CurrentGate.X > 0)
                {
                    do
                    {
                        if (--CurrentGate.X == -1)
                        {
                            CurrentGate.X = Game.Field.GetLength(0) - 1;
                            CurrentGate.Y--;
                            if (CurrentGate.Y == -1)
                            {
                                CurrentGate.Y = Game.Field.GetLength(1) - 1;
                            }
                        }
                    }
                    while ((Game.Field[CurrentGate.X, CurrentGate.Y].Owner == -1) == selectFilled);
                }
                else if (CurrentKey?.Key == ConsoleKey.RightArrow && CurrentGate.Y < Game.Field.GetLength(1) - 1)
                {
                    do
                    {
                        if (++CurrentGate.Y == Game.Field.GetLength(1))
                        {
                            CurrentGate.Y = 0;
                            CurrentGate.X++;
                            if (CurrentGate.X == Game.Field.GetLength(0))
                            {
                                CurrentGate.X = 0;
                            }
                        }
                    }
                    while ((Game.Field[CurrentGate.X, CurrentGate.Y].Owner == -1) == selectFilled);
                }
                else if (CurrentKey?.Key == ConsoleKey.LeftArrow && CurrentGate.Y > 0)
                {
                    do
                    {
                        if (--CurrentGate.Y == -1)
                        {
                            CurrentGate.Y = Game.Field.GetLength(1) - 1;
                            CurrentGate.X--;
                            if (CurrentGate.X == -1)
                            {
                                CurrentGate.X = Game.Field.GetLength(0) - 1;
                            }
                        }
                    }
                    while ((Game.Field[CurrentGate.X, CurrentGate.Y].Owner == -1) == selectFilled);
                }
                DrawIO();
                CurrentKey = null;
            }

            CurrentKey = null;
            IOType = null;
            (int, int) toReturn = (CurrentGate.X, CurrentGate.Y);
            CurrentGate = (-1, -1);
            return toReturn;
        }
    }
}
