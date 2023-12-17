using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AB_Client
{
    internal static class ServerTalker
    {
        private static HttpClient client = new HttpClient() { BaseAddress = new Uri("http://185.155.18.243:8080/") };
        private static long sessionID = GenerateSessionID();

        private static dynamic Request(string request, string json)
        {
            var webRequest = new HttpRequestMessage(HttpMethod.Get, request)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            HttpResponseMessage response = client.Send(webRequest);

            using var reader = new StreamReader(response.Content.ReadAsStream());

            return JObject.Parse(reader.ReadToEnd());
        }

        private static long GenerateSessionID()
        {
            return Request("getsession", "{}")["UUID"];
        }

        public static string CreateRoom()
        {
            dynamic jobject = Request("createroom", "{\"playerCount\": 2}");
            Request("joinroom", "{\"UUID\":" + sessionID + ", \"roomName\": \"" + jobject["room"] + "\", \"userName\": \"" + Program.Settings.name + "\"}");

            return jobject["room"];
        }

        public static bool JoinRoom(string room)
        {
            return Request("joinroom", "{\"UUID\":" + sessionID + ", \"roomName\": \"" + room + "\", \"userName\": \"" + Program.Settings.name + "\"}")["success"];
        }

        public static bool UpdateReady(string room, bool isReady)
        {
            return Request("updateready", "{\"UUID\":" + sessionID + ", \"roomName\": \"" + room + "\", \"isReady\": \"" + isReady + "\"}")["canStart"];
        }

        public static bool CheckStarted(string room)
        {
            return Request("checkstarted", "{\"UUID\":" + sessionID + ", \"roomName\": \"" + room + "\"}")["started"];
        }

        public static void StartRoom(string room)
        {
            Request("startroom", "{\"UUID\":" + sessionID + ", \"roomName\": \"" + room + "\"}");
        }

        public static bool[] GetAllReady(string room)
        {
            JObject jobject = Request("getallready", "{\"UUID\":" + sessionID + ", \"roomName\": \"" + room + "\"}");

            return jobject["ready"].Select(x => (bool)x).ToArray();
        }

        public static string?[] GetAllPlayers(string room)
        {
            JObject jobject = Request("getplayerlist", "{\"UUID\":" + sessionID + ", \"roomName\": \"" + room + "\"}");

            return jobject["players"].Select(x => (string?)x).ToArray();
        }

        public static dynamic CheckTurnStart(long gid, int pid)
        {
            JObject json = new()
            {
                { "gid", gid },
                { "playerID", pid }
            };
            return Request("checkturnstart", json.ToString())["turnplayer"];
        }

        public static bool CheckReady(string room)
        {
            return Request("checkready", "{\"UUID\":" + sessionID + ", \"roomName\": \"" + room + "\"}")["canStart"];
        }

        public static long StartGame(string room, int playerCount)
        {
            return Request("newgame", "{\"UUID\":" + sessionID + ", \"roomName\": \"" + room + "\", \"playerCount\": \"" + playerCount + "\"}")["gid"];
        }

        public static long GetGID(string room)
        {
            return Request("getgid", "{\"room\": \"" + room + "\"}")["gid"];
        }

        public static (int PlayerCount, int PID) JoinGame(long GID, JObject deck)
        {
            dynamic jobject = Request("join", "{\"gid\":" + GID + ", \"UUID\":" + sessionID + ", \"deck\": " + deck.ToString() + "}");

            return (jobject["playerCount"], jobject["pid"]);
        }

        public static dynamic GetUpdates(long GID, int PID)
        {
            return Request("getupdates", "{\"gid\":" + GID + ", \"pid\":" + PID + "}")["updates"];
        }

        public static void Answer(JObject json, long gid, int pid)
        {
            json.Add("gid", gid);
            json.Add("playerID", pid);
            Request("answer", json.ToString());
        }

        public static void Move(JObject json, long gid, int pid)
        {
            json.Add("gid", gid);
            json.Add("playerID", pid);
            Request("move", json.ToString());
        }

        public static dynamic GetMoves(long gid, int pid)
        {
            JObject json = new()
            {
                { "gid", gid },
                { "playerID", pid }
            };
            return Request("getmoves", json.ToString())["moves"];
        }

        public static void Leave(long gid, int pid)
        {
            Request("leave", new JObject()
            {
                { "gid", gid },
                { "playerID", pid }
            }.ToString());
        }
    }
}
