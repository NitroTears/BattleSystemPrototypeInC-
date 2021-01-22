using System;
using static System.Console;

namespace Prototype.Battle
{
    /// <summary>
    /// This class represents the battle between two sides, usually a <see cref="PlayerBattleActor"/>
    /// against a <see cref="BattleActor"/>. Each side has a turn using a <see cref="Spell"/>, until one side has no HP remaining, 
    /// which then a winning side will be decided.
    /// </summary>
    class BattleScene
    {
        /// <summary>
        /// gets or sets the outcome byte. 1 for player victory, 2 for enemy victory.
        /// </summary>
        byte _outcome {get; set;} = 0;
        /// <summary>
        /// Gets or sets the number of turns that have passed during the battle.
        /// </summary>
        public byte TurnCount {get; set;}
        /// <summary>
        /// Gets or sets the current player in the battle scene.
        /// </summary>
        public PlayerBattleActor player {get; set;}
        /// <summary>
        /// Gets or sets the current enemy in the battle scene.
        /// </summary>
        public BattleActor enemy {get; set;}
        /// <summary>
        /// Gets the BattleControlWindow to display the battle scene.
        /// </summary>
        public BattleWindowControl window {get;} = new BattleWindowControl();

        /// <summary>
        /// Initialises a new instance of the <see cref="BattleScene"/> class. Sets turn count to 1 and sets the player's turn first.
        /// Also initialises <see cref="PlayerBattleActor"/>'s spells.
        /// </summary>
        /// <param name="player">The player actor that will participate in battle.</param>
        /// <param name="enemy">The enemy actor that will participate in battle.</param>
        public BattleScene(PlayerBattleActor player, BattleActor enemy)
        {
            this.player = player;
            this.enemy = enemy;
            TurnCount = 0;
        }

        /// <summary>
        /// The main battle loop. After an actor chooses a spell, the turn will switch and repeats until only one actor has HP left.
        /// </summary>
        /// <returns>The value '_outcome' is set to at the end of the battle.</returns>
        public byte BattleLoop() {
            var playerMsg = Tuple.Create("", "");
            var enemyMsg = Tuple.Create("", "");
            var firstTurn = true;
            
            while (player.HP > 0 && enemy.HP > 0)
            {
                Console.Clear();
                window.PrintStatusLines(player, enemy, TurnCount);
                if (firstTurn) 
                {
                    window.PrintEmptyLines(true, 5);
                    firstTurn = false;
                }
                else
                {
                    window.PrintLine1(playerMsg.Item1, playerMsg.Item2);
                    window.PrintEmptyLines(false, 1);
                    window.PrintLine1(enemyMsg.Item1, enemyMsg.Item2);
                }
                //PLAYER TURN
                window.PrintSkills(player.SpellList);
                Write("/// ");
                playerMsg = PlayerTurn();
                if (_outcome != 0) //break out if battle finished
                {
                    return _outcome;
                }
                //ENEMY TURN
                enemyMsg = EnemyTurn();
                if (_outcome != 0) //break out if battle finished
                {
                    return _outcome;
                }
                
            }
            return _outcome;
        }
        /// <summary>
        /// Handles player turn actions, such as incrementing turn count, selecting a spell,
        /// and then attacking an enemy.
        /// </summary>
        /// <returns>A tuple containing one or two lines to be printed to the console. Passed along through AttackActor method.</returns>
        Tuple<string, string> PlayerTurn() {
            TurnCount++;
            var tup = player.AttackActor(enemy, PlayerSpellChoice());
            if (enemy.HP <= 0)
            {
                _outcome = 1;
                Console.Clear();
                window.PrintStatusLines(player, enemy, TurnCount);
                Console.WriteLine();
                window.printVictoryText(player, _outcome);
            }
            return tup;
        }

        /// <summary>
        /// The player inputs the spell they would like to use.
        /// If they do not have the spell, try the input again.
        /// </summary>
        /// <returns>The spell the player sucessfully selected.</returns>
        Spell PlayerSpellChoice() {
            var exists = false;
            string selection = null;
            while (!exists)
            {
                selection = ReadLine().ToLower();
                foreach (Spell s in player.SpellList)
                {
                    if (selection.Equals(s.ID)) //checks if spell is in player's SpellList
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists) //incorrect input or player doesn't have spell in list, try input again.
                {
                    Write("ERROR: You do not have that spell. Press Enter to try again.");
                    ReadKey();
                    Console.Clear();
                    window.PrintStatusLines(player, enemy, TurnCount);
                    window.PrintEmptyLines(false, 5);
                    window.PrintSkills(player.SpellList);
                    Write("/// ");
                }
            }
            Spell spell = new Spell(selection);
            return spell;
        }


        /// <summary>
        /// Handles enemy turn actions, such as calculating which move to use with AI, and then attacking an enemy.
        /// </summary>
        /// <returns>A tuple containing one or two lines to be printed to the console. Passed along through AttackActor method.</returns>
        Tuple<string, string> EnemyTurn() 
        {
            var tup = Tuple.Create("","");
            Random rand = new Random();
            if (enemy.HP > enemy.MaxHP * 0.75) //75% of MaxHP or more
                {
                    Spell attack = new Spell("attack");
                    tup = enemy.AttackActor(player, attack);
                    window.PrintLine1(tup.Item1, tup.Item2);
                }
                else if (enemy.HP <= enemy.MaxHP * 0.75 && enemy.HP > enemy.MaxHP * 0.35) //75% to 35% range of MaxHP
                {
                    var choice = rand.Next(0, 2);
                    if (choice == 1)
                    {
                        Spell fire = new Spell("fire");
                        tup = enemy.AttackActor(player, fire);
                        window.PrintLine1(tup.Item1, tup.Item2);
                    }
                    else
                    {
                        Spell ice = new Spell("ice");
                        tup = enemy.AttackActor(player, ice);
                        window.PrintLine1(tup.Item1, tup.Item2);
                    }
                }
                else if (enemy.HP < enemy.MaxHP * 0.35) //35% of MaxHP or less)
                {
                    var choice2 = rand.Next(0, 2);
                    if (choice2 == 1)
                    {
                        Spell heal = new Spell("heal");
                        tup = enemy.AttackActor(enemy, heal);
                        window.PrintLine1(tup.Item1, tup.Item2);
                    }
                    else
                    {
                        Spell attack = new Spell("attack");
                        tup = enemy.AttackActor(player, attack);
                        window.PrintLine1(tup.Item1, tup.Item2);
                        
                    }
                }
            if (player.HP <= 0)
            {
                _outcome = 2;
                Console.Clear();
                window.PrintStatusLines(player, enemy, TurnCount);
                Console.WriteLine();
                window.printVictoryText(enemy, _outcome);
            }
            return tup;
        }
    }
}