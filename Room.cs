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
        Timer updateChecker = new(200);
        public bool IsMember;

        public Room(string roomKey, int playerCount)
        {
            this.roomKey = roomKey;
            updateChecker.Elapsed += CheckUpdates;
            updateChecker.AutoReset = true;
            updateChecker.Enabled = true;
            updateChecker.Start();
            IsMember = Display.RoomPrompt(roomKey, playerCount);
        }

        private void CheckUpdates(object source, ElapsedEventArgs e)
        {
            if (ServerTalker.CheckStarted(roomKey))
            {
                updateChecker.AutoReset = false;
                updateChecker.Enabled = false;
                Display.RoomStarted = true;
            }

            bool[] readyinfo = ServerTalker.GetAllReady(roomKey);
            string[] playersinfo = ServerTalker.GetAllPlayers(roomKey) as string[];

            Display.Updating = true;
            if (readyinfo != null)
                for (int i = 0; i < readyinfo.Length; i++)
                {
                    Display.UsersReady[i] = readyinfo[i];
                }
            if (playersinfo != null)
                for (int i = 0; i < playersinfo.Length; i++)
                {
                    Display.UserNames[i] = playersinfo[i];
                }
            Display.Updating = false;
            if (Display.IOType == "room") Display.DrawRoomIO();
        }
    }
}
