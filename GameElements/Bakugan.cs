using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AB_Client
{
    internal class Bakugan
    {
        public static readonly string[] Attributes = new string[]
        {
            "Pyrus",
            "Aquos",
            "Darkus",
            "Ventus",
            "Haos",
            "Subterra",
            "Clear"
        };
        static readonly string[] Treatments = new string[]
        {
            "none",
            "clear",
            "diamond",
            "camo",
            "lightup",
            "golden"
        };
        static readonly string[] BakuganTypes = new string[]
        {
            "beestriker",
            "cancer",
            "centipede",
            "crow",
            "elcondor",
            "elephant",
            "fairy",
            "gargoyle",
            "garrison",
            "glorius",
            "griffon",
            "jackal",
            "juggernaut",
            "knight",
            "laserman",
            "limulus",
            "mantis",
            "raptor",
            "rattloid",
            "saurus",
            "scorpion",
            "serpent",
            "shredder",
            "sphinx",
            "worm"
        };

        public int BID { get; protected set; }
        public string Attribute { get; protected set; }
        public string Treatment { get; protected set; }
        public string Type { get; protected set; }
        public int Power { get; set; }
        public int Owner { get; protected set; }

        public Bakugan(int type, int attribute, int treatment, int power, int owner, int BID)
        {
            this.BID = BID;
            Attribute = Attributes[attribute];
            Treatment = Treatments[treatment];
            Power = power;
            Owner = owner;
            Type = BakuganTypes[type];
        }

        public static Bakugan FromJson(dynamic json, int owner)
        {
            return new Bakugan((int)json.Type, (int)json.Attribute, (int)json.Treatment, (int)json.Power, owner, (int)json.BID);
        }

        public string GetFullName()
        {
            if (Treatment != "none")
                return Locales.Loc[Treatment] + " " + Locales.Loc[Attribute] + " " + Locales.Loc[Type] + ", " + Power;
            return Locales.Loc[Attribute] + " " + Locales.Loc[Type] + ", " + Power;
        }

        public string GetMidName()
        {
            return Locales.Loc[Attribute] + " " + Locales.Loc[Type];
        }

        public string GetShortName()
        {
            return Locales.Loc[Type];
        }
    }
}
