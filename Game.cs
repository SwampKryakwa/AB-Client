using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Timers;

namespace AB_Client
{
    internal class Game
    {
        public GateCard[,] Field = new GateCard[2, 3];

        public static string[] AbilityNames =
        [
            "firejudge",
            "firetornado",
            "backfire",
            "rapidfire",
            "rapidlight",
            "clayarmor",
            "magmasurface",
            "desertvortex",
            "spiritcanyon",
            "lighthelix",
            "lightningtornado",
            "haosfreeze",
            "blindingbrilliance",
            "colourfuldeath",
            "cyclingmadness",
            "chainsdes",
            "judgementnight",
            "uptake",
            "tornadowall",
            "blindjudge",
            "tsunami"
        ];

        public static string[] SpecialThrows = [];

        long GID;

        public int TurnPlayer = -1;
        public int ActivePlayer;
        public int PID;
        public int PlayerCount;
        public string[] PlayerNames;
        JArray updates = new();

        bool setFirstGate = false;
        bool running = true;

        public Game(long GID, int PID, int playerCount)
        {
            Display.Game = this;
            this.GID = GID;
            this.PID = PID;
            PlayerCount = playerCount;

            for (int i = 0; i < Field.GetLength(0); i++)
                for (int j = 0; j < Field.GetLength(1); j++)
                    Field[i, j] = new GateCard(-1, null, -1, -1);
            Display.DrawField();
            File.WriteAllText("battlelog.txt", $"Your PID: {PID}\n\n");

            while (running)
            {
                foreach (JObject j in ServerTalker.GetUpdates(GID, this.PID))
                {
                    updates.Add(j);
                }
                if (!updates.Any()) continue;
                int X, Y;
                string GateName;
                JObject move;
                bool prevStart = false;
                /*int c = 0;
                foreach (dynamic update in updates)
                {
                    Console.WriteLine(update.Type);
                    c++;
                }
                if (c != 0)
                    Console.ReadKey();*/
                bool needPrint = false;
                List<JToken> cloneUpdates = new();
                int stop = updates.Count;
                for (int i = 0; i < stop; i++)
                {
                    cloneUpdates.Add(updates[i]);
                }
                foreach (JToken j in cloneUpdates)
                {
                    updates.Remove(j);
                }

                bool didTurnStart = false;
                foreach (dynamic update in cloneUpdates)
                {
                    try
                    {
                        IEnumerable<Bakugan> Selection;
                        List<Bakugan> bakugans;
                        List<GateCard> gatecards;
                        int selectedGate;
                        string message;
                        int selectedBakugan;
                        int boost;
                        Bakugan targetBakugan;
                        File.AppendAllText("battlelog.txt", update.ToString() + "\n\n");
                        try
                        {
                            switch (update["Type"].ToString())
                            {
                                case "PlayerNamesInfo":
                                    JArray data = update["Info"] as JArray;
                                    PlayerNames = new string[data.Count];
                                    for (int i = 0; i < PlayerNames.Length; i++)
                                        PlayerNames[i] = data[i].ToString();
                                    break;
                                case "PickGateEvent":
                                    if (setFirstGate) break;
                                    needPrint = true;
                                    GateCard[] gates = new GateCard[update["Gates"].Count];

                                    for (int i = 0; i < update["Gates"].Count; i++)
                                        gates[i] = GateCard.FromJson(update["Gates"][i], this.PID);
                                    int pick = Display.SelectionPrompt(Locales.Loc[update["Prompt"].ToString()], gates.Select(x => x.GetFullName()).ToArray());
                                    JObject answer = new JObject { { "gate", pick } };
                                    ServerTalker.Answer(answer, GID, PID);
                                    Field[this.PID, 1] = gates[pick];
                                    for (int i = 0; i < playerCount; i++)
                                    {
                                        if (i != this.PID)
                                            Field[i, 1] = new GateCard(-1, null, i, -1);
                                    }
                                    Display.DrawField();
                                    Display.Wait(Locales.Loc["awaiting_other_choice_start"]);
                                    Display.DrawIO();
                                    prevStart = true;
                                    setFirstGate = true;
                                    break;

                                case "PlayerTurnStart":
                                    if (TurnPlayer != (int)update.PID)
                                        Display.Events.Add(Locales.Loc["player_turn_event"].Replace("%", PlayerNames[(int)update.PID]));
                                    Display.DrawEvents();

                                    TurnPlayer = (int)update.PID;

                                    if (PID != TurnPlayer)
                                    {
                                        Display.Wait(Locales.Loc["other_turn_start"].Replace("%", PlayerNames[(int)update.PID]));
                                        Display.DrawIO();
                                    }
                                    else
                                    {
                                        prevStart = true;
                                        MakeTurn();
                                    }

                                    break;

                                case "GateSetEvent":
                                    X = (int)update["PosX"];
                                    Y = (int)update["PosY"];
                                    GateName = Display.alphabet[Y].ToString() + (X + 1);
                                    Field[X, Y] = GateCard.FromJson(update.GateData, (int)update.CID, (int)update.Owner);
                                    if (TurnPlayer != this.PID)
                                    {
                                        Display.DrawField();
                                        needPrint = true;
                                    }
                                    Display.Events.Add(Locales.Loc["player_placed_gate"].Replace("%", PlayerNames[(int)update.PID]) + GateName);
                                    Display.DrawEvents();
                                    break;

                                case "BakuganThrownEvent":
                                    X = (int)update.PosX;
                                    Y = (int)update.PosY;
                                    GateName = Display.alphabet[Y].ToString() + (X + 1);
                                    Bakugan thrownBakugan = Bakugan.FromJson(update.Bakugan, (int)update.Owner);
                                    Field[X, Y].Bakugans.Add(thrownBakugan);

                                    if (TurnPlayer != this.PID)
                                    {
                                        Display.DrawField();
                                        needPrint = true;
                                    }
                                    Display.Events.Add(Locales.Loc["player_thrown_bakugan"].Replace("%", PlayerNames[(int)update.Owner]).Replace("&", thrownBakugan.GetShortName()) + GateName);
                                    Display.DrawEvents();
                                    break;

                                case "BakuganAddedEvent":
                                    X = (int)update.PosX;
                                    Y = (int)update.PosY;
                                    GateName = Display.alphabet[Y].ToString() + (X + 1);
                                    Bakugan addedBakugan = Bakugan.FromJson(update.Bakugan, (int)update.Owner);
                                    Field[X, Y].Bakugans.Add(addedBakugan);

                                    if (TurnPlayer != this.PID)
                                    {
                                        Display.DrawField();
                                        needPrint = true;
                                    }
                                    Display.Events.Add(Locales.Loc["added_bakugan"].Replace("&", addedBakugan.GetShortName()) + GateName);
                                    Display.DrawEvents();
                                    break;

                                case "BakuganMovedEvent":
                                    X = (int)update.PosX;
                                    Y = (int)update.PosY;
                                    GateName = Display.alphabet[Y].ToString() + (X + 1);
                                    Bakugan movedBakugan = Field.Cast<GateCard>().SelectMany(x => x.Bakugans.Where(x => x.BID == (int)update.Bakugan.BID)).First();
                                    Field.Cast<GateCard>().First(x => x.Bakugans.Contains(movedBakugan)).Bakugans.Remove(movedBakugan);
                                    Field[X, Y].Bakugans.Add(movedBakugan);

                                    if (TurnPlayer != this.PID)
                                    {
                                        Display.DrawField();
                                        needPrint = true;
                                    }
                                    Display.Events.Add(Locales.Loc["moved_bakugan"].Replace("&", movedBakugan.GetShortName()) + GateName);
                                    Display.DrawEvents();
                                    break;

                                case "AbilityActivateEffect":
                                    Selection = Field.Cast<GateCard>().SelectMany(x => x.Bakugans.Where(y => y.BID == (int)update.UserID));
                                    Bakugan userBakugan = null;
                                    if (Selection.Count() != 0)
                                        userBakugan = Selection.First();
                                    else
                                        userBakugan = Bakugan.FromJson(Selection, -1);
                                    Display.DrawField();
                                    needPrint = true;
                                    Display.Events.Add(Locales.Loc["use_ability_event"].Replace("&", userBakugan.GetMidName()).Replace("$", Locales.Loc[AbilityNames[(int)update.Card]]));
                                    Display.DrawEvents();

                                    break;

                                case "BakuganBoostedEvent":
                                    Selection = Field.Cast<GateCard>().SelectMany(x => x.Bakugans.Where(y => y.BID == (int)update.Bakugan.BID));
                                    targetBakugan = null;
                                    if (Selection.Count() != 0)
                                        userBakugan = Selection.First();
                                    else
                                        userBakugan = Bakugan.FromJson(update.Bakugan, -1);

                                    boost = (int)update.Boost;

                                    userBakugan.Power += boost;

                                    if (boost >= 0)
                                        Display.Events.Add(Locales.Loc["bakugan_boosted"].Replace("&", userBakugan.GetMidName()).Replace("$", boost.ToString()));
                                    else
                                        Display.Events.Add(Locales.Loc["bakugan_deboosted"].Replace("&", userBakugan.GetMidName()).Replace("$", boost.ToString()));
                                    Display.DrawEvents();

                                    break;

                                case "BakuganPermaBoostedEvent":
                                    Selection = Field.Cast<GateCard>().SelectMany(x => x.Bakugans.Where(y => y.BID == (int)update.Bakugan.BID));
                                    targetBakugan = null;
                                    if (Selection.Count() != 0)
                                        userBakugan = Selection.First();
                                    else
                                        userBakugan = Bakugan.FromJson(update.Bakugan, -1);

                                    boost = (int)update.Boost;

                                    userBakugan.Power += boost;

                                    if (boost >= 0)
                                        Display.Events.Add(Locales.Loc["bakugan_permaboosted"].Replace("&", userBakugan.GetMidName()).Replace("$", boost.ToString()));
                                    else
                                        Display.Events.Add(Locales.Loc["bakugan_permadeboosted"].Replace("&", userBakugan.GetMidName()).Replace("$", boost.ToString()));
                                    Display.DrawEvents();

                                    break;

                                case "StartSelection":
                                    prevStart = true;
                                    switch (update.SelectionType.ToString())
                                    {
                                        case "B":
                                            bakugans = new();
                                            foreach (dynamic b in update.SelectionBakugans as JArray)
                                                bakugans.Add(Bakugan.FromJson(b, (int)b.Owner));

                                            message = Locales.Loc[update.Message.ToString()].Replace("$", Locales.Loc[AbilityNames[(int)update.Ability]]);

                                            selectedBakugan = Display.SelectionPrompt(message, bakugans.Select(x => x.GetFullName()).ToArray());

                                            move = new() { { "bakugan", bakugans[selectedBakugan].BID }, { "AbilityAnswer", "" } };

                                            ServerTalker.Answer(move, GID, PID);

                                            break;
                                        case "G":
                                            gatecards = new();
                                            foreach (dynamic g in update.SelectionGates as JArray)
                                                gatecards.Add(Field[(int)g.Pos / 10, (int)g.Pos % 10]);

                                            message = Locales.Loc[update.Message.ToString()].Replace("$", Locales.Loc[AbilityNames[(int)update.Ability]]);

                                            selectedGate = Display.SelectionPrompt(message, gatecards.Select(x => x.GetFullName() + " (" + Display.alphabet[(int)update.Pos / 10 + 1] + ((int)update.Pos % 10).ToString() + ")").ToArray());

                                            move = new() { { "gate", gatecards[selectedGate].CID } };

                                            ServerTalker.Answer(move, GID, PID);

                                            break;
                                    }
                                    foreach (dynamic upd in updates.Cast<JObject>())
                                    {
                                        if (upd.Type == "PlayerTurnStart") updates.Remove(upd);
                                    }

                                    break;

                                case "StartSelectionArr":
                                    prevStart = true;
                                    move = new() { { "array", new JArray() } };
                                    foreach (dynamic sel in update.Selections as JArray)
                                        try
                                        {
                                            switch (sel.SelectionType.ToString())
                                            {
                                                case "B":
                                                    bakugans = new();
                                                    foreach (dynamic b in sel.SelectionBakugans as JArray)
                                                        bakugans.Add(Bakugan.FromJson(b, (int)b.Owner));

                                                    message = Locales.Loc[sel.Message.ToString()].Replace("$", Locales.Loc[AbilityNames[(int)sel.Ability]]);

                                                    selectedBakugan = Display.SelectionPrompt(message, bakugans.Select(x => x.GetFullName()).ToArray());

                                                    (move["array"] as JArray).Add(new JObject { { "Type", "throw" }, { "bakugan", bakugans[selectedBakugan].BID }, { "AbilityAnswer", "" } });

                                                    break;
                                                case "B?":
                                                    switch (sel.SelectionRange.ToString())
                                                    {
                                                        case "SGE":
                                                            int BID = (int)(move["array"] as JArray)[0]["bakugan"];
                                                            GateCard gate = Field.Cast<GateCard>().First(x => x.Bakugans.Any(x => x.BID == BID));
                                                            bakugans = gate.Bakugans.Where(x => x.Owner != PID).ToList();

                                                            message = Locales.Loc[sel.Message.ToString()].Replace("$", Locales.Loc[AbilityNames[(int)sel.Ability]]);

                                                            selectedBakugan = Display.SelectionPrompt(message, bakugans.Select(x => x.GetFullName()).ToArray());

                                                            (move["array"] as JArray).Add(new JObject { { "Type", "throw" }, { "bakugan", bakugans[selectedBakugan].BID }, { "AbilityAnswer", "" } });

                                                            break;
                                                    }

                                                    break;

                                                case "G":
                                                    gatecards = new();
                                                    foreach (dynamic g in sel.SelectionGates as JArray)
                                                    {
                                                        gatecards.Add(GateCard.FromJson(g, -1));
                                                        gatecards[^1].Pos = (int)g.Pos;
                                                    }

                                                    message = Locales.Loc[sel.Message.ToString()].Replace("$", Locales.Loc[AbilityNames[(int)sel.Ability]]);

                                                    selectedGate = Display.SelectionPrompt(message, gatecards.Select(x => x.GetShortName() + " (" + Display.alphabet[(int)x.Pos / 10 + 1] + ((int)x.Pos % 10).ToString() + ")").ToArray());

                                                    (move["array"] as JArray).Add(new JObject() { { "gate", gatecards[selectedGate].CID } });

                                                    break;

                                                case "G?":
                                                    switch (sel.SelectionRange.ToString())
                                                    {
                                                        case "TGHE":
                                                            int BID = (int)(move["array"] as JArray)[0]["bakugan"];
                                                            GateCard gate = Field.Cast<GateCard>().First(x => x.Bakugans.Any(x => x.BID == BID));
                                                            (X, Y) = ExtraMethods.CoordinatesOf(Field, gate);

                                                            gatecards = new();

                                                            List<string> poses = new();

                                                            if (X - 1 >= 0)
                                                                if (Field[X - 1, Y].Bakugans.Any(x => x.Owner != PID))
                                                                {
                                                                    gatecards.Add(Field[X - 1, Y]);
                                                                    poses.Add($"{Display.alphabet[X]}{Y}");
                                                                }
                                                            if (X + 1 < Field.GetLength(0))
                                                                if (Field[X + 1, Y].Bakugans.Any(x => x.Owner != PID))
                                                                {
                                                                    gatecards.Add(Field[X + 1, Y]);
                                                                    poses.Add($"{Display.alphabet[X + 2]}{Y}");
                                                                }
                                                            if (Y - 1 >= 0)
                                                                if (Field[X, Y - 1].Bakugans.Any(x => x.Owner != PID))
                                                                {
                                                                    gatecards.Add(Field[X, Y - 1]);
                                                                    poses.Add($"{Display.alphabet[X]}{Y - 1}");
                                                                }
                                                            if (Y + 1 < Field.GetLength(1))
                                                                if (Field[X, Y + 1].Bakugans.Any(x => x.Owner != PID))
                                                                {
                                                                    gatecards.Add(Field[X, Y + 1]);
                                                                    poses.Add($"{Display.alphabet[X]}{Y + 1}");
                                                                }

                                                            message = Locales.Loc[sel.Message.ToString()].Replace("$", Locales.Loc[AbilityNames[(int)sel.Ability]]);

                                                            selectedGate = Display.SelectionPrompt(message, poses.ToArray());

                                                            (X, Y) = ExtraMethods.CoordinatesOf(Field, gatecards[selectedGate]);

                                                            (move["array"] as JArray).Add(new JObject() { { "gate", gatecards[selectedGate].CID }, { "pos", X.ToString() + Y } });

                                                            break;
                                                    }
                                                    break;

                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Console.Clear();
                                            Console.WriteLine(sel.ToString());
                                            throw;
                                        }
                                    ServerTalker.Answer(move, GID, PID);
                                    foreach (dynamic upd in updates.Cast<JObject>())
                                    {
                                        if (upd.Type == "PlayerTurnStart") updates.Remove(upd);
                                    }

                                    break;

                                case "GateRemoved":
                                    X = (int)update.PosX;
                                    Y = (int)update.PosY;
                                    Field[X, Y] = new GateCard(-1, null, -1, -1);
                                    Display.DrawField();
                                    if (PID != ActivePlayer)
                                        Display.Events.Add(Locales.Loc["other_turn_start"].Replace("%", PlayerNames[TurnPlayer]));
                                    break;

                                case "BattleOver":
                                    Display.DrawField();
                                    if ((bool)update.IsDraw)
                                    {
                                        Display.Events.Add(Locales.Loc["battle_end_draw"]);
                                    }
                                    else
                                    {
                                        Display.Events.Add(Locales.Loc["battle_end"].Replace("%", PlayerNames[(int)update.Victor]));
                                    }
                                    break;

                                case "BattleOverAllLoser":
                                    Display.DrawField();
                                    Display.Events.Add(Locales.Loc["battle_end_all_lose"]);
                                    break;

                                case "BakuganRemoved":
                                    for (int i = 0; i < Field.GetLength(0); i++)
                                        for (int j = 0; j < Field.GetLength(1); j++)
                                            if (Field[i, j].Bakugans.Any(x => x.BID == (int)update.Bakugan.BID))
                                            {
                                                Field[i, j].Bakugans.RemoveAll(x => x.BID == (int)update.Bakugan.BID);
                                                break;
                                            }
                                    break;

                                case "GameOver":
                                    Display.DrawField();
                                    Display.Events.Add(Locales.Loc["game_over"].Replace("%", PlayerNames[(int)update.Victor]));

                                    Console.ReadKey(true);
                                    running = false;
                                    ServerTalker.Leave(GID, PID);
                                    break;
                            }
                        }
                        catch
                        {
                            if (!Directory.Exists("error_logs")) Directory.CreateDirectory("error_logs");
                            File.WriteAllText($"error_logs/data-{DateTime.Now.ToString().Replace(':', '-')}.txt", update.ToString());
                            throw;
                        }
                    }
                    catch (Exception e)
                    {
                        if (!Directory.Exists("error_logs")) Directory.CreateDirectory("error_logs");
                        File.WriteAllText($"error_logs/log-{DateTime.Now.ToString().Replace(':', '-')}.txt", e.ToString());
                        Environment.Exit(-1);
                    }
                }

                if (updates.Count > 0)
                {
                    Display.FullRedraw();
                    if (TurnPlayer != this.PID && !prevStart && needPrint)
                    {
                        Display.Events.Add(Locales.Loc["other_turn_start"].Replace("%", PlayerNames[TurnPlayer]));
                    }
                }
            }
        }

        void CheckUpdates()
        {
            updates.Add(ServerTalker.CheckTurnStart(GID, PID) as JObject);
        }

        void MakeTurn()
        {
            try
            {
                Display.DrawField();
                dynamic moves = ServerTalker.GetMoves(GID, PID);
                File.AppendAllText("battlelog.txt", moves.ToString() + "\n\n");
                List<string> options = new();
                List<int> positions = new();

                if ((bool)moves["CanSetGate"])
                {
                    options.Add(Locales.Loc["set_gate_move"]);
                    positions.Add(0);
                }
                if ((bool)moves["CanOpenGate"])
                {
                    options.Add(Locales.Loc["open_gate_move"]);
                    positions.Add(1);
                }
                if ((bool)moves["CanThrowBakugan"])
                {
                    options.Add(Locales.Loc["throw_bakugan_move"]);
                    positions.Add(2);
                }
                if ((bool)moves["CanActivateAbility"])
                {
                    options.Add(Locales.Loc["activate_ability_move"]);
                    positions.Add(3);
                }
                if ((bool)moves["CanEndTurn"])
                {
                    if ((bool)moves.IsASkip)
                        options.Add(Locales.Loc["skip_turn_move"]);
                    else
                        options.Add(Locales.Loc["end_turn_move"]);
                    positions.Add(4);
                }
                if ((bool)moves["CanEndBattle"])
                {
                    if ((bool)moves.IsAPass)
                        options.Add(Locales.Loc["pass_battle_move"]);
                    else
                        options.Add(Locales.Loc["end_battle_move"]);
                    positions.Add(5);
                }
;
                int X, Y;
                switch (positions[Display.SelectionPrompt(Locales.Loc["your_turn_start"], options.ToArray())])
                {
                    //setting gate card
                    case 0:
                        List<GateCard> gates = new();
                        foreach (dynamic gate in (JArray)moves["SettableGates"])
                            gates.Add(GateCard.FromJson(gate, (int)gate.CID, PID));

                        int selectedGate = Display.SelectionPrompt(Locales.Loc["gate_set_choice"], gates.Select(x => x.GetFullName()).ToArray());
                        (X, Y) = Display.GatePrompt(Locales.Loc["gate_pos_choice"], false);

                        ServerTalker.Move(new() { { "Type", "set" }, { "gate", gates[selectedGate].CID }, { "posX", X }, { "posY", Y } }, GID, PID);
                        foreach (dynamic update in updates.Cast<JObject>())
                        {
                            if (update.Type == "PlayerTurnStart") updates.Remove(update);
                        }

                        break;

                    //opening gate card
                    case 1:
                        List<GateCard> openableGates = new();
                        foreach (dynamic gate in (JArray)moves["OpenableGates"])
                            openableGates.Add(Field.Cast<GateCard>().First(x => x.CID == (int)gate.CID));

                        int openedGate = Display.SelectionPrompt(Locales.Loc["gate_open_choice"], openableGates.Select(x => x.GetFullName()).ToArray());

                        ServerTalker.Move(new() { { "Type", "open" }, { "gate", openableGates[openedGate].CID } }, GID, PID);
                        foreach (dynamic update in updates.Cast<JObject>())
                        {
                            if (update.Type == "PlayerTurnStart") updates.Remove(update);
                        }

                        break;

                    //throwing bakugan
                    case 2:
                        List<Bakugan> bakugans = new();
                        foreach (dynamic bakugan in (JArray)moves["ThrowableBakugan"])
                            bakugans.Add(Bakugan.FromJson(bakugan, PID));

                        int selectedBakugan = Display.SelectionPrompt(Locales.Loc["bakugan_throw_choice"], bakugans.Select(x => x.GetFullName()).ToArray());
                        (X, Y) = Display.GatePrompt(Locales.Loc["throw_pos_choice"], true);

                        ServerTalker.Move(new() { { "Type", "throw" }, { "bakugan", bakugans[selectedBakugan].BID }, { "posX", X }, { "posY", Y } }, GID, PID);
                        foreach (dynamic update in updates.Cast<JObject>())
                        {
                            if (update.Type == "PlayerTurnStart") updates.Remove(update);
                        }

                        break;

                    //using ability
                    case 3:
                        string[] abilities = (moves.ActivateableAbilities as JArray).Select(x => Locales.Loc[AbilityNames[(int)x["Type"]]]).ToArray();
                        int[] ids = (moves.ActivateableAbilities as JArray).Select(x => (int)x["cid"]).ToArray();
                        int selectedAbility = Display.SelectionPrompt(Locales.Loc["use_ability_choice"], abilities);

                        ServerTalker.Move(new() { { "Type", "activate" }, { "ability", ids[selectedAbility] } }, GID, PID);
                        foreach (dynamic update in updates.Cast<JObject>())
                        {
                            if (update.Type == "PlayerTurnStart") updates.Remove(update);
                        }

                        break;

                    //ending turn
                    case 4:
                        ServerTalker.Move(new() { { "Type", "end" } }, GID, PID);
                        foreach (dynamic update in updates.Cast<JObject>())
                        {
                            if (update.Type == "PlayerTurnStart") updates.Remove(update);
                        }
                        break;

                    //ending battle
                    case 5:
                        ServerTalker.Move(new() { { "Type", "pass" } }, GID, PID);
                        foreach (dynamic update in updates.Cast<JObject>())
                        {
                            if (update.Type == "PlayerTurnStart") updates.Remove(update);
                        }
                        break;
                }
            }
            catch
            {

            }
        }
    }

    internal static class ExtraMethods
    {
        public static Tuple<int, int> CoordinatesOf<T>(this T[,] matrix, T value)
        {
            int w = matrix.GetLength(0); // width
            int h = matrix.GetLength(1); // height

            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    if (matrix[x, y].Equals(value))
                        return Tuple.Create(x, y);
                }
            }

            return Tuple.Create(-1, -1);
        }
    }
}
