using Prototype.Battle;
using System;

namespace Prototype
{
    class Program
    {
        /// <summary>
        /// Example / test method. Change 'initplayer' and 'bot' to an entity listed in stats.json to change battlers.
        /// </summary>
        static void Main(string[] args)
        {
            PlayerBattleActor player = new PlayerBattleActor("initplayer"); //Who the player controls
            BattleActor enemy = new BattleActor("bot"); //Who the player will fight
            BattleScene battleScene = new BattleScene(player, enemy);
            battleScene.BattleLoop();
            
        }
    }
}
