using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using System.Timers;
using System.Threading.Tasks;
using Terraria.Localization;

namespace RealTime
{
    [ApiVersion(2, 1)]
    public class RealTime : TerrariaPlugin
    {
        public override string Author => "十七";
        public override string Description => "同步现实时间";
        public override string Name => "RealTime";
        public override Version Version => new Version(2, 0, 0, 0);
        public RealTime(Main game) : base(game)
        {
        }
        public override void Initialize()
        {
            ServerApi.Hooks.GameUpdate.Register(this, OnGameUpdate);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameUpdate.Deregister(this, OnGameUpdate);
            }
            base.Dispose(disposing);
        }
        int i = 0;
        int y = 0;
        int q = 0;
        bool lastBloodMoon = false;
        bool lastEclipse = false;
        bool lastPumpkinMoon = false;
        bool lastSnowMoon = false;
        DateTime time = DateTime.Now;
        private void OnGameUpdate(EventArgs args)
        {
            #region 血月
            if (lastBloodMoon ^ Main.bloodMoon)
            {
                if (Main.bloodMoon)
                {
                    //血月开启瞬间
                    time = DateTime.Now + TimeSpan.FromMinutes(30);
                }
            }
            lastBloodMoon = Main.bloodMoon;
            if (Main.bloodMoon == true)
            {
                if (DateTime.Now >= time)
                {
                    Main.bloodMoon = false;
                }
            }
            #endregion
            #region 日食
            if (lastEclipse ^ Main.eclipse)
            {
                if (Main.eclipse)
                {
                    //日食开启瞬间
                    time = DateTime.Now + TimeSpan.FromMinutes(30);
                }
            }
            lastEclipse = Main.eclipse;
            if (Main.eclipse == true)
            {
                if (DateTime.Now >= time)
                {
                    Main.eclipse = false;
                }
            }
            #endregion
            #region 万圣节
            if (lastPumpkinMoon ^ Main.pumpkinMoon)
            {
                if (Main.pumpkinMoon)
                {
                    //万圣节开启瞬间
                    time = DateTime.Now + TimeSpan.FromMinutes(59);
                }
            }
            lastPumpkinMoon = Main.pumpkinMoon;
            if (Main.pumpkinMoon == true)
            {
                if (DateTime.Now >= time)
                {
                    Main.pumpkinMoon = false;
                }
            }
            #endregion
            #region 霜月
            if (lastSnowMoon ^ Main.snowMoon)
            {
                if (Main.snowMoon)
                {
                    //霜月开启瞬间
                    time = DateTime.Now + TimeSpan.FromMinutes(59);
                }
            }
            lastSnowMoon = Main.snowMoon;
            if (Main.snowMoon == true)
            {
                if (DateTime.Now >= time)
                {
                    Main.snowMoon = false;
                }
            }
            #endregion
            q++;
            if (q == 86400)//渔夫任务和月相刷新
            {
                string msg = GetMoon(Main.moonPhase);
                Main.moonPhase += 1;
                Main.AnglerQuestSwap();
                TSPlayer.All.SendInfoMessage("月相已更换为：{0}，渔夫任务已更换", msg);
                q = 0;
            }
            i++; y++;
            if (i == 480)//真实时间
            {
                DateTime dt = DateTime.Now;
                dt.ToShortTimeString().ToString();
                decimal d = int.Parse(dt.Hour.ToString()) + int.Parse(dt.Minute.ToString()) / 60.0m;
                d -= 4.50m;
                if (d < 0.00m)
                {
                    d += 24.00m;
                }
                if (d >= 15.00m)
                {
                    TSPlayer.Server.SetTime(false, (double)((d - 15.00m) * 3600.0m));
                }
                else
                {
                    TSPlayer.Server.SetTime(true, (double)(d * 3600.0m));
                }
                i = 0;
            }
            if (y == 86400)//npc生成
            {
                var AllNPCS = Main.npc.Where(n => n != null);
                foreach (var TNPC in AllNPCS)
                {
                    if (TNPC.netID == 37)
                    {
                        if (!TNPC.active)//遍历所有NPC，判断这个NPC是否存在
                        {
                            if (!NPC.downedBoss3)//判断有没有击败骷髅王
                            {
                                TSPlayer.Server.SpawnNPC(37, "老人", 200, Main.dungeonX, Main.dungeonY, 50, 20);
                            }

                        }
                    }
                    if (TNPC.netID == 439)
                    {
                        if (!TNPC.active)
                        {
                            if (!NPC.downedMoonlord && NPC.downedGolemBoss)//没有击败月总但击败了石头人
                            {
                                TSPlayer.Server.SpawnNPC(439, "教徒", 200, Main.dungeonX, Main.dungeonY, 50, 20);
                            }
                        }
                    }
                }
                y = 0;
            }
        }
        private String GetMoon(int index)
        {
            String[] arr = new string[] { "满月", "亏凸月", "下弦月", "残月", "新月", "娥眉月", "上弦月", "盈凸月" };
            if (index == -1 || index + 1 > arr.Length)
                return "未知";

            return arr[index];
        }
    }
}
