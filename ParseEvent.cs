using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AB_Client
{
    internal static class Event
    {
        public static Game game;

        public static void Parse(dynamic Event)
        {
            //don't forget to write event parsing
            switch (Event.Type as string)
            {
                case "StartTurn":
                    break;
                case "MakeTurn":
                    break;
                case "MakeSelection":
                    break;
                case "SetGate":
                    SetGate((int)Event.type, (int)Event.owner, (int)Event.posX, (int)Event.posY, Event);
                    break;
                case "RemoveGate":
                    RemoveGate((int)Event.posX, (int)Event.posY);
                    break;
                case "OpenGate":
                    OpenGate((int)Event.type, (int)Event.owner, (int)Event.posX, (int)Event.posY, Event);
                    break;
                case "DiscardGate":
                    break;
                case "RetractGate":
                    break;
                case "ActivateAbility":
                    break;
                case "NegateAbility":
                    break;
                case "RestoreAbility":
                    break;
                case "DiscardAbility":
                    break;
                case "ThrowBakugan":
                    break;
                case "AddBakugan":
                    break;
                case "DestroyBakugan":
                    break;
                case "RemoveBakugan":
                    break;
                case "RetractBakugan":
                    break;
                case "BoostedBakugan":
                    break;
                case "BaseboostedBakugan":
                    break;
                case "SpecialThrow":
                    break;
                case "ContEffectStart":
                    break;
                case "ContEffectEnd":
                    break;
                case "GateBattleStart":
                    break;
                case "DistantAttack":
                    break;
            }

            Display.DrawEffects();
            Display.DrawEvents();
            Display.DrawField();
        }

        static void StartTurn()
        {

        }

        static void MakeTurn()
        {

        }

        static void MakeSelection()
        {

        }

        static void SetGate(int type, int owner, int posX, int posY, dynamic extra)
        {
            game.Field[posX, posY] = new GateCard(type, extra, owner, -1);
            Display.Events.Add(Locales.Loc["gate_set_event"].Replace("%", owner.ToString()).Replace("$", Display.alphabet[posY].ToString() + posX));
        }

        static void RemoveGate(int posX, int posY)
        {
            game.Field[posX, posY] = null;
            Display.Events.Add(Locales.Loc["gate_removed_event"].Replace("$", Display.alphabet[posY].ToString() + posX));
        }

        static void OpenGate(int type, int owner, int posX, int posY, dynamic extra)
        {
            game.Field[posX, posY].Type = GateCard.Types[type];
            game.Field[posX, posY].ExtraData = extra;
            game.Field[posX, posY].isOpen = true;
            Display.Events.Add(Locales.Loc["gate_open_event"].Replace("&", Locales.Loc[GateCard.Types[type]]).Replace("%", owner.ToString()).Replace("$", Display.alphabet[posY].ToString() + posX));
        }

        static void DiscardGate(int type, int owner)
        {
            Display.Events.Add(Locales.Loc["gate_discard_event"].Replace("&", Locales.Loc[GateCard.Types[type]]).Replace("%", owner.ToString()));
        }

        static void RetractGate(int type, int owner, int posX, int posY)
        {
            Display.Events.Add(Locales.Loc["gate_retract_event"].Replace("&", Locales.Loc[GateCard.Types[type]]).Replace("%", owner.ToString()).Replace("$", Display.alphabet[posY].ToString() + posX));
        }

        static void ActivateAbility(int type, int owner)
        {
            Display.Events.Add(Locales.Loc["ability_activate_event"].Replace("&", Locales.Loc[Game.AbilityNames[type]]).Replace("%", owner.ToString()));
        }

        static void NegateAbility(int type, int owner)
        {
            Display.Events.Add(Locales.Loc["ability_negate_event"].Replace("&", Locales.Loc[Game.AbilityNames[type]]).Replace("%", owner.ToString()));
        }

        static void DiscardAbility(int type, int owner)
        {
            Display.Events.Add(Locales.Loc["ability_discard_event"].Replace("&", Locales.Loc[GateCard.Types[type]]).Replace("%", owner.ToString()));
        }

        static void RestoreAbility(int type, int owner)
        {
            Display.Events.Add(Locales.Loc["ability_restore_event"].Replace("&", Locales.Loc[GateCard.Types[type]]).Replace("%", owner.ToString()));
        }

        static void ThrowBakugan(int type, int power, int attribute, int treatment, int id, int owner, int posX, int posY)
        {
            Bakugan bakugan = new Bakugan(type, power, attribute, treatment, owner, id);
            game.Field[posX, posY].Bakugans.Add(bakugan);
            Display.Events.Add(Locales.Loc["bakugan_throw_event"].Replace("%", owner.ToString()).Replace("&", bakugan.GetFullName()));
        }

        static void AddBakugan(int type, int power, int attribute, int treatment, int id, int owner, int posX, int posY)
        {
            Bakugan bakugan = new Bakugan(type, power, attribute, treatment, owner, id);
            game.Field[posX, posY].Bakugans.Add(bakugan);
            Display.Events.Add(Locales.Loc["bakugan_add_event"].Replace("%", owner.ToString()).Replace("&", bakugan.GetFullName()));
        }

        static void DestroyBakugan(int type, int power, int attribute, int treatment, int owner)
        {
            Bakugan bakugan = new Bakugan(type, power, attribute, treatment, owner, 0);
            Display.Events.Add(Locales.Loc["bakugan_destroy_event"].Replace("%", owner.ToString()).Replace("&", bakugan.GetFullName()));
        }

        static void RemoveBakugan(int id, int posX, int posY, bool silent)
        {
            Bakugan bakugan = game.Field[posX, posY].Bakugans.First(x => x.BID == id);
            game.Field[posX, posY].Bakugans.Remove(bakugan);
            if (!silent)
                Display.Events.Add(Locales.Loc["bakugan_remove_event"].Replace("%", bakugan.Owner.ToString()).Replace("&", bakugan.GetFullName()));
        }

        static void RetractBakugan(int type, int power, int attribute, int treatment, int owner)
        {
            Bakugan bakugan = new Bakugan(type, power, attribute, treatment, owner, 0);
            Display.Events.Add(Locales.Loc["bakugan_retract_event"].Replace("%", owner.ToString()).Replace("&", bakugan.GetFullName()));
        }

        static void BoostBakugan(int id, int owner, int boost)
        {
            Bakugan bakugan = game.Field.Cast<GateCard>().SelectMany(x => x.Bakugans).First(x => x.BID == id);
            bakugan.Power += boost;
            if (boost > 0)
                Display.Events.Add(Locales.Loc["bakugan_boost_event"].Replace("%", owner.ToString()).Replace("&", bakugan.GetMidName()).Replace("$", boost.ToString()));
            else
                Display.Events.Add(Locales.Loc["bakugan_deboost_event"].Replace("%", owner.ToString()).Replace("&", bakugan.GetMidName()).Replace("$", boost.ToString()));
        }

        static void BoostBakuganHand(int type, int power, int attribute, int treatment, int id, int owner, int boost)
        {
            Bakugan bakugan = new Bakugan(type, power, attribute, treatment, owner, id);
            if (boost > 0)
                Display.Events.Add(Locales.Loc["bakugan_boost_event"].Replace("%", owner.ToString()).Replace("&", bakugan.GetMidName()).Replace("$", boost.ToString()));
            else
                Display.Events.Add(Locales.Loc["bakugan_deboost_event"].Replace("%", owner.ToString()).Replace("&", bakugan.GetMidName()).Replace("$", boost.ToString()));
        }

        static void BaseBoostBakugan(int id, int owner, int boost)
        {
            Bakugan bakugan = game.Field.Cast<GateCard>().SelectMany(x => x.Bakugans).First(x => x.BID == id);
            bakugan.Power += boost;
            if (boost > 0)
                Display.Events.Add(Locales.Loc["bakugan_base_boost_event"].Replace("%", owner.ToString()).Replace("&", bakugan.GetMidName()).Replace("$", boost.ToString()));
            else
                Display.Events.Add(Locales.Loc["bakugan_base_deboost_event"].Replace("%", owner.ToString()).Replace("&", bakugan.GetMidName()).Replace("$", boost.ToString()));
        }

        static void BaseBoostBakuganHand(int type, int power, int attribute, int treatment, int id, int owner, int boost)
        {
            Bakugan bakugan = new Bakugan(type, power, attribute, treatment, owner, id);
            if (boost > 0)
                Display.Events.Add(Locales.Loc["bakugan_base_boost_event"].Replace("%", owner.ToString()).Replace("&", bakugan.GetMidName()).Replace("$", boost.ToString()));
            else
                Display.Events.Add(Locales.Loc["bakugan_base_deboost_event"].Replace("%", owner.ToString()).Replace("&", bakugan.GetMidName()).Replace("$", boost.ToString()));
        }

        static void SpecialThrowUsed(int type, int user)
        {
            Display.Events.Add(Locales.Loc["bakugan_remove_event"].Replace("%", user.ToString()).Replace("$", Game.SpecialThrows[type]));
        }

        static void ContEffectStart(int type)
        {
            Display.activeAbilities.Add(Locales.Loc[Game.AbilityNames[type]]);
        }

        static void ContEffectStop(int type)
        {
            Display.activeAbilities.Remove(Locales.Loc[Game.AbilityNames[type]]);
        }

        static void GateBattleStart(int posX, int posY)
        {
            Display.Events.Add(Locales.Loc["gate_battle_start_event"].Replace("$", Display.alphabet[posY].ToString() + posX));
        }
    }
}
