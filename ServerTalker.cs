using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AB_Client
{
    internal static class ServerTalker
    {
#if DEBUG
        private static HttpClient client = new HttpClient() { BaseAddress = new Uri("http://localhost:8080/") };
#else
        private static HttpClient client = new HttpClient() { BaseAddress = new Uri("http://185.155.18.243:8080/") };
#endif
        private static long sessionID = GenerateSessionID();

        private static dynamic Request(string request, string json)
        {
            var webRequest = new HttpRequestMessage(HttpMethod.Get, request)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            HttpResponseMessage response = client.Send(webRequest);

            using var reader = new StreamReader(response.Content.ReadAsStream());

            string data = reader.ReadToEnd();
            try
            {
                return JObject.Parse(data);
            }
            catch
            {
                if (!Directory.Exists("error_logs")) Directory.CreateDirectory("error_logs");
                File.WriteAllText($"error_logs/data-{DateTime.Now.ToString().Replace(':', '-')}.txt", data);
                throw;
            }
        }

        private static long GenerateSessionID() =>
            Request("getsession", "{}")["UUID"];


        public static string CreateRoom(int playerCount)
        {
            dynamic jobject = Request("createroom", $"{{\"playerCount\": {playerCount}}}");
            Request("joinroom", "{\"UUID\":" + sessionID + ", \"roomName\": \"" + jobject["room"] + "\", \"userName\": \"" + Program.Settings.name + "\"}");

            return jobject["room"];
        }

        public static bool JoinRoom(string room) =>
            Request("joinroom", "{\"UUID\":" + sessionID + ", \"roomName\": \"" + room + "\", \"userName\": \"" + Program.Settings.name + "\"}")["success"];


        public static void LeaveRoom(string room) =>
            Request("leaveroom", "{\"UUID\":" + sessionID + ", \"roomName\": \"" + room + "\", \"userName\": \"" + Program.Settings.name + "\"}");


        public static bool UpdateReady(string room, bool isReady) =>
            Request("updateready", "{\"UUID\":" + sessionID + ", \"roomName\": \"" + room + "\", \"isReady\": \"" + isReady + "\"}")["canStart"];


        public static bool CheckStarted(string room) =>
            Request("checkstarted", "{\"UUID\":" + sessionID + ", \"roomName\": \"" + room + "\"}")["started"];


        public static void StartRoom(string room) =>
            Request("startroom", "{\"UUID\":" + sessionID + ", \"roomName\": \"" + room + "\"}");


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

        public static bool Ping()
        {
            try
            {
                return Request("ping", "{}")["response"];
            }
            catch
            {
                return false;
            }
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

        public static bool CheckReady(string room) =>
            Request("checkready", "{\"UUID\":" + sessionID + ", \"roomName\": \"" + room + "\"}")["canStart"];


        public static long StartGame(string room, int playerCount) =>
            Request("newgame", "{\"UUID\":" + sessionID + ", \"roomName\": \"" + room + "\", \"playerCount\": \"" + playerCount + "\"}")["gid"];


        public static long GetGID(string room) =>
            Request("getgid", "{\"room\": \"" + room + "\"}")["gid"];


        public static (int PlayerCount, int PID) JoinGame(long GID, JObject deck)
        {
            dynamic jobject = Request("join", $"{{\"gid\":{GID}, \"UUID\":{sessionID}, \"deck\": {deck}, \"name\": \"{Program.Settings.name}\"}}");

            return (jobject["playerCount"], jobject["pid"]);
        }

        public static dynamic GetUpdates(long GID, int PID) =>
            Request("getupdates", "{\"gid\":" + GID + ", \"pid\":" + PID + "}")["updates"];


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
