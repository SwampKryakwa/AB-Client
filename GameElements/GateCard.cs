using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AB_Client
{
    internal class GateCard
    {
        public List<Bakugan> Bakugans = new();

        public static string[] Types =
        [
            "normalgate",
            "triplebattle",
            "quartetbattle",
            "mindghost",
            "attributehazard"
        ];

        public string Type;
        public dynamic ExtraData;
        public int CID;
        public int Owner;
        public bool ActiveBattle = false;
        public bool BattleFrozen = false;
        public int Pos = 0;

        public bool isOpen = false;

        public GateCard(int type, dynamic extraData, int owner, int CID)
        {
            this.CID = CID;
            Owner = owner;
            if (type == -1)
                Type = "none";
            else
                Type = Types[type];
            ExtraData = extraData;
        }

        public static GateCard FromJson(dynamic json, int owner)
        {
            try
            {
                return new GateCard((int)json.Type, json, owner, 0);
            }
            catch
            {
                if (!Directory.Exists("error_logs")) Directory.CreateDirectory("error_logs");
                File.WriteAllText($"error_logs/data-{DateTime.Now.ToString().Replace(':', '-')}.txt", json.ToString());
                throw;
            }
        }

        public static GateCard FromJson(dynamic json, int CID, int owner)
        {
            return new GateCard((int)json.Type, json, owner, CID);
        }

        public string GetFullName()
        {
            if (Type == "normalgate")
                return Locales.Loc["normalgate"] + " (" + Locales.Loc[Bakugan.Attributes[ExtraData.Attribute]] + ", " + ExtraData.Power + ")";

            if (Type == "attributehazard")
                return Locales.Loc["attributehazard"] + " (" + Locales.Loc[Bakugan.Attributes[ExtraData.Attribute]] + ")";

            return Locales.Loc[Type];
        }

        public string GetShortName()
        {
            return Locales.Loc[Type];
        }
    }
}
