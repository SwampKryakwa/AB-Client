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
        };

        public static Game Game;

        public static string GetFullName(dynamic bakugan)
        {
            return Locales.Loc[bakugan["attribute"]] + " " + Locales.Loc[bakugan["name"]] + ", " + Locales.Loc[bakugan["power"]];
        }

        public static void DisplayField()
        {
            Console.Clear();

            Console.SetCursorPosition(0, 0);
            if (Game.PID == 0)
                FormattedPrint($"{Locales.Loc["you_are"]}@c{Locales.Loc["player"]} 1@w");
            else if (Game.PID == 1)
                FormattedPrint($"{Locales.Loc["you_are"]}@r{Locales.Loc["player"]} 2@w");

            string toPrint;
            for (int i = 0; i < Game.Field.GetLength(0); i++)
            {
                toPrint = "";
                for (int j = 0; j < Game.Field.GetLength(1); j++)
                {
                    char color = 'w';
                    if (Game.Field[i, j] != null)
                    {
                        if (Game.Field[i, j].Owner == 0) color = 'c';
                        else if (Game.Field[i, j].Owner == 1) color = 'r';
                    }
                    toPrint += $"@{color}{alphabet[j]}{i + 1} ";
                }
                FormattedPrintCenter(toPrint);
            }
            Console.WriteLine();

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
                        FormattedPrintCenter(toPrint);
                    }
                }
            }
            Console.WriteLine();
        }

        public static void DisplayField(int[] selection)
        {
            Console.Clear();
            string toPrint;
            for (int i = 0; i < Game.Field.GetLength(0); i++)
            {
                toPrint = "";
                for (int j = 0; j < Game.Field.GetLength(1); j++)
                {
                    char color = 'w';
                    char bgcolor = 'b';
                    if (i == selection[0] && j == selection[1])
                    {
                        bgcolor = 'g';
                    }
                    if (Game.Field[i, j] != null)
                    {
                        if (Game.Field[i, j].Owner == 0) color = 'c';
                        else if (Game.Field[i, j].Owner == 1) color = 'r';
                    }
                    toPrint += $"@u{bgcolor}@{color}{alphabet[j]}{i + 1}@ub ";
                }
                FormattedPrintCenter(toPrint);
            }
            Console.WriteLine();

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
                        FormattedPrintCenter(toPrint);
                    }
                }
            }

            Console.WriteLine();
        }

        public static bool BoolPrompt(string prompt)
        {
            int selection = 0;
            string[] options = new string[] { "Да", "Нет" };
            while (true)
            {
                FormattedPrint(prompt);
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selection) { Console.BackgroundColor = ConsoleColor.DarkGray; FormattedPrintD(options[i]); }
                    else FormattedPrint(options[i]);
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                var newSelection = Console.ReadKey(true);
                if (newSelection.Key == ConsoleKey.Enter) break;
                else if (newSelection.Key == ConsoleKey.DownArrow && selection < options.Length) selection++;
                else if (newSelection.Key == ConsoleKey.UpArrow && selection > 0) selection--;
            }
            return selection == 0;
        }

        public static (bool, string) TextPrompt(string prompt)
        {
            bool inputting = true;
            string input = "";
            while (true)
            {
                Console.Clear();
                Console.SetCursorPosition(Console.CursorTop, (Console.BufferHeight - 4) / 2);
                FormattedPrintCenter(prompt);
                Console.WriteLine();
                if (inputting)
                {
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    FormattedPrintCenter(input);
                    Console.BackgroundColor = ConsoleColor.Black;
                    FormattedPrintCenter(Locales.Loc["button_return"]);
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    FormattedPrintCenter(input);
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    FormattedPrintCenter(Locales.Loc["button_return"]);
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                var newSelection = Console.ReadKey(true);
                if (newSelection.Key == ConsoleKey.Enter)
                {
                    if (inputting)
                        break;
                    else
                    {
                        return (false, "");
                    }
                }
                else if (newSelection.Key == ConsoleKey.DownArrow && inputting) inputting = false;
                else if (newSelection.Key == ConsoleKey.UpArrow && !inputting) inputting = true;
                else if (inputting && newSelection.Key == ConsoleKey.Backspace && input.Length != 0) input = input[..^1];
                else if (newSelection.Modifiers == ConsoleModifiers.Control && newSelection.Key == ConsoleKey.V) input += TextCopy.ClipboardService.GetText();
                else if (inputting) input += newSelection.KeyChar;
            }
            return (true, input);
        }

        public static int GatePrompt(string prompt, params string[] options)
        {
            int selection = 0;
            while (true)
            {
                DisplayField();
                Console.SetCursorPosition(Console.CursorTop, (Console.BufferHeight - options.Length - 3) / 2);
                FormattedPrintCenter(prompt);
                Console.WriteLine();
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selection) { Console.BackgroundColor = ConsoleColor.DarkGray; FormattedPrintDCenter(options[i]); }
                    else FormattedPrintCenter(options[i]);
                    Console.BackgroundColor = ConsoleColor.Black;
                }

                var newSelection = Console.ReadKey(true);
                if (newSelection.Key == ConsoleKey.Enter) break;
                else if (newSelection.Key == ConsoleKey.DownArrow && selection < options.Length - 1) selection++;
                else if (newSelection.Key == ConsoleKey.UpArrow && selection > 0) selection--;
                else if ((int)newSelection.Key > 48 && (int)newSelection.Key < 58) selection = (int)newSelection.Key - 49;
                else if ((int)newSelection.Key > 96 && (int)newSelection.Key < 106) selection = (int)newSelection.Key - 97;
            }
            return selection;
        }

        public static int Prompt(string prompt, params string[] options)
        {
            int selection = 0;
            while (true)
            {
                Console.Clear();
                Console.SetCursorPosition(Console.CursorTop, (Console.BufferHeight - options.Length - 3) / 2);
                FormattedPrintCenter(prompt);
                Console.WriteLine();
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selection) { Console.BackgroundColor = ConsoleColor.DarkGray; FormattedPrintDCenter(options[i]); }
                    else FormattedPrintCenter(options[i]);
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                var newSelection = Console.ReadKey(true);
                if (newSelection.Key == ConsoleKey.Enter) break;
                else if (newSelection.Key == ConsoleKey.DownArrow && selection < options.Length - 1) selection++;
                else if (newSelection.Key == ConsoleKey.UpArrow && selection > 0) selection--;
                else if ((int)newSelection.Key > 48 && (int)newSelection.Key < 58) selection = (int)newSelection.Key - 49;
                else if ((int)newSelection.Key > 96 && (int)newSelection.Key < 106) selection = (int)newSelection.Key - 97;
            }
            return selection;
        }

        public static (int X, int Y) SelectGate(string prompt, bool selectFilled)
        {
            int[] selection = new int[] { 0, 0 };

            while ((Game.Field[selection[0], selection[1]].Owner == -1) == selectFilled)
            {
                selection[0]++;
                if (selection[0] == Game.Field.GetLength(0))
                {
                    selection[0] = 0;
                    selection[1]++;
                    if (selection[1] == Game.Field.GetLength(1))
                    {
                        selection[1] = 0;
                    }
                }
            }

            while (true)
            {
                DisplayField(selection);
                FormattedPrintCenter(prompt);
                var newSelection = Console.ReadKey(true);
                if (newSelection.Key == ConsoleKey.Enter) break;
                else if (newSelection.Key == ConsoleKey.DownArrow && selection[0] < Game.Field.GetLength(0) - 1)
                {
                    do
                    {
                        selection[0]++;
                        if (selection[0] == Game.Field.GetLength(0))
                        {
                            selection[0] = 0;
                            selection[1]++;
                            if (selection[1] == Game.Field.GetLength(1))
                            {
                                selection[1] = 0;
                            }
                        }
                    }
                    while ((Game.Field[selection[0], selection[1]].Owner == -1) == selectFilled);
                }
                else if (newSelection.Key == ConsoleKey.UpArrow && selection[0] > 0)
                {
                    do
                    {
                        selection[0]--;
                        if (selection[0] == -1)
                        {
                            selection[0] = Game.Field.GetLength(0) - 1;
                            selection[1]--;
                            if (selection[1] == -1)
                            {
                                selection[1] = Game.Field.GetLength(1) - 1;
                            }
                        }
                    }
                    while ((Game.Field[selection[0], selection[1]].Owner == -1) == selectFilled);
                }
                else if (newSelection.Key == ConsoleKey.RightArrow && selection[1] < Game.Field.GetLength(1) - 1)
                {
                    do
                    {
                        selection[1]++;
                        if (selection[1] == Game.Field.GetLength(1))
                        {
                            selection[1] = 0;
                            selection[0]++;
                            if (selection[0] == Game.Field.GetLength(0))
                            {
                                selection[0] = 0;
                            }
                        }
                    }
                    while ((Game.Field[selection[0], selection[1]].Owner == -1) == selectFilled);
                }
                else if (newSelection.Key == ConsoleKey.LeftArrow && selection[1] > 0)
                {
                    do
                    {
                        selection[1]--;
                        if (selection[1] == -1)
                        {
                            selection[1] = Game.Field.GetLength(1) - 1;
                            selection[0]--;
                            if (selection[0] == -1)
                            {
                                selection[0] = Game.Field.GetLength(0) - 1;
                            }
                        }
                    }
                    while ((Game.Field[selection[0], selection[1]].Owner == -1) == selectFilled);
                }
            }

            return (selection[0], selection[1]);
        }

        public static void FormattedPrint(string toPrint)
        {
            Console.ForegroundColor = ConsoleColor.White;
            int a = 0;
            foreach (string word in toPrint.Split('@'))
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
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        public static void FormattedPrintCenter(string toPrint)
        {
            string[] tp = toPrint.Split('\n');
            foreach (string line in tp)
            {
                string[] data = line.Split('@');
                int offset = data.Length * 2 + data[1..].Count(x => x[1] == 'u');
                Console.SetCursorPosition((Console.BufferWidth - line.Length + offset) / 2, Console.GetCursorPosition().Top);
                FormattedPrint(line);
            }
        }

        public static void FormattedPrintDCenter(string toPrint)
        {
            string[] tp = toPrint.Split('\n');
            foreach (string line in tp)
            {
                string[] data = line.Split('@');
                int offset = data.Length * 2 + data[1..].Count(x => x[1] == 'u');
                Console.SetCursorPosition((Console.BufferWidth - line.Length + offset) / 2, Console.GetCursorPosition().Top);
                FormattedPrintD(line);
            }
        }

        public static void FormattedPrintD(string toPrint)
        {
            Console.ForegroundColor = ConsoleColor.White;
            int a = 0;
            foreach (string word in toPrint.Split('@'))
            {
                if (a != 0)
                {
                    switch (word[0])
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
                        case 'l':
                            Console.ForegroundColor = ConsoleColor.Black;
                            break;
                        case 'w':
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case 'u':
                            switch (word[1])
                            {
                                case 'r':
                                    Console.BackgroundColor = ConsoleColor.DarkRed;
                                    break;
                                case 'g':
                                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                                    break;
                                case 'b':
                                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                                    break;
                                case 'c':
                                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                                    break;
                                case 'y':
                                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                                    break;
                                case 'm':
                                    Console.BackgroundColor = ConsoleColor.DarkMagenta;
                                    break;
                                case 'l':
                                    Console.BackgroundColor = ConsoleColor.Black;
                                    break;
                                case 'w':
                                    Console.BackgroundColor = ConsoleColor.White;
                                    break;
                            }
                            break;
                    }
                    Console.Write(word[1..]);
                }
                else
                    Console.Write(word);
                a++;
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        public static int Prompt(string prompt, string[] options, int[] returnIDs)
        {
            return returnIDs[Prompt(prompt, options)];
        }
    }
}
