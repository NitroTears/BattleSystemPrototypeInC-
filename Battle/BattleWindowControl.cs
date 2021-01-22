using System;
using System.Diagnostics;
using static System.Console;
using System.Drawing;
using Console = Colorful.Console;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Prototype.Battle
{
    /// <summary>
    /// This class is primarily used for controlling and altering the terminal GUI.
    /// I.e. the main game view for the player
    /// </summary>
    /// 
    class BattleWindowControl
    {
        /// <summary>
        /// String representing a window border in the GUI.
        /// </summary>
        private const string _longBar = "+----------------------------------------------------------------+"; //64 dashes
        /// <summary>
        /// String representing an empty space in the GUI (still has outer borders).
        /// </summary>
        private const string _emptyBar = "|                                                                |"; //64 spaces
        /// <summary>
        /// String representing the first few lines of the SpellList display area.
        /// </summary>
        private const string _skillListLabel = "+------------+                                                   || Spell List |                                                   |+------------+----------------------------+                      |";
        /// <summary>
        /// String representing the bottom line of the GUI, right before the user input.
        /// </summary>
        private readonly string _bottomBar = "/// Enter Spell Name ---------------------+----------------------+";
        /// <summary>
        /// Gets the main colour of the GUI borders.
        /// </summary>
        private readonly Color _borderColour  = Color.FromArgb(56, 39, 170);
        /// <summary>
        /// Gets the main colour used for numbers in the GUI;
        /// </summary>
        private readonly Color _digitColour = Color.FromArgb(113, 186, 135);
        /// <summary>
        /// Gets the main text colour of the GUI;
        /// </summary>
        private readonly Color _textColour = Color.FromArgb(113, 128, 185);
        /// <summary>
        /// Gets the colour used for labels and names in the GUI;
        /// </summary>
        private readonly Color _labelColour = Color.FromArgb(163, 231, 252);

        /// <summary>
        /// Used to assist terminal operations on MacOSX.
        /// Thanks to: https://stackoverflow.com/questions/31522500/mono-resize-terminal-on-mac-os-x
        /// </summary>
        [DllImport("libc")]
        private static extern int system(string exec);

        /// <summary>
        /// Creates an instance of the <see cref='BattleWindowControl'> class.
        /// </summary>
        public BattleWindowControl() {
            Console.Title = "RPG Prototype";
            if (OperatingSystem.IsWindows())
            {
                Console.SetWindowSize(65, 17);
                Console.SetBufferSize(66, 17); //actual window size
                Console.BackgroundColor = Color.FromArgb(20, 20, 50);
                Console.ForegroundColor = _borderColour;
            }
            else
            {
                system(@"printf '\e[8;17;66t'");
                Console.ForegroundColor = _borderColour;
                Console.BackgroundColor = Color.FromArgb(0, 205, 205);
            }
            Console.Clear();
        }


        /// <summary>
        /// Formats player and enemy data and to be displayed. Prints the 'status bar' portion of the GUI to the console.
        /// </summary>
        /// <param name="player">The <see cref='PlayerBattleActor'> whose information will be displayed.</param>
        /// <param name="enemy">The <see cref='BattleActor'> whose information will be displayed.</param>
        /// <param name="turnCount">The turn number to be displayed.</param>
        public void PrintStatusLines(IBattleActor player, IBattleActor enemy, byte turnCount)
        {  
            //Top Border
            Console.Write(_longBar);
            //HP Line Portion
            Console.Write("| "); 
            Console.Write(string.Format("{0,-9}", player.Name), _textColour);
            Console.Write(" HP: ", _labelColour);
            Console.Write(string.Format("{0, -6}", player.HP), _digitColour);
            Console.Write("          -         "); // 8 - 9
            Console.Write(string.Format("{0, 12}", enemy.Name), _textColour);
            Console.Write(" HP: ", _labelColour);
            Console.Write(string.Format("{0, -5}", enemy.HP), _digitColour);
            Console.Write(" |");
            //Turn Line Portion
            Console.Write("| ");
            Console.Write("Turn: ", _labelColour);
            Console.Write(string.Format("{0, -3}", turnCount), _digitColour);
            string spaceleft = new string(' ', 54);
            Console.Write(spaceleft + "|");
            //Bottom Border
            Console.Write(_longBar);
        }

        /// <summary>
        /// Prints one or two formatted lines to the console, depending on the string arguments provided.
        /// if extra is null, only message will be printed.
        /// </summary>
        /// <param name="message">The main message string to be displayed.</param>
        /// <param name="extra">The extra message string to be displayed.</param>
        public void PrintLine1(string message, string extra) 
        {
            if (extra == null)
            {
                
                Console.Write("| ");
                Console.Write(string.Format("{0,-62}", message), _textColour);
                Console.Write($" |");
                PrintLine2("");
            }
            else
            {
                Console.Write("| ");
                Console.Write(string.Format("{0,-62}", message), _textColour);
                Console.Write(" |");
                PrintLine2(extra);
            }
            

        }

        /// <summary>
        /// Prints one formatted line to the console. Only used in printLine1()
        /// </summary>
        /// <param name="message">The message string to be displayed.</param>
        private void PrintLine2(string message)
        {
            Console.Write("| ");
            Console.Write(string.Format("{0,-62}", message), _textColour);
            Console.Write(" |");
        }

        /// <summary>
        /// Prints formated blank lines to the console, used for line spacing in the GUI.
        /// </summary>
        /// <param name="introMessage">Whether to display the opening battle message.</param>
        /// <param name="noOfLines">Number of blank lines to be printed.</param>
        public void PrintEmptyLines(Boolean introMessage, byte noOfLines) // Lines 1-5
        {
            if (introMessage)
            {
                Console.Write("| ");
                Console.Write("Battle Start! Enter your chosen spell to attack!              ", _textColour);
                Console.Write(" |");
                for (int i = 0; i < noOfLines - 1; i++)
                {
                    Write(_emptyBar);
                }
            }
            else 
            {
                for (int i = 0; i < noOfLines; i++)
                {
                    Write(_emptyBar);
                }
            }
        }

        /// <summary>
        /// Prints a formatted list of spells to the console, to be used with the GUI.
        /// </summary>
        /// <param name="spellList">spell list to be displayed.</param>
        public void PrintSkills(List<Spell> spellList)
        {
            Console.Write(_skillListLabel);
            byte bars = 0; //goes up to 3
            var spellRightArea = "|                      |";
            var spellsString = "";
            int count = 0;
            foreach (Spell spell in spellList)
            {
                if (count.Equals(spellList.Count - 1) && bars != 3)
                {
                    spellsString = spellsString + spell.Name;
                    Console.Write("| ");
                    Console.Write(string.Format("{0,-40}", spellsString), _textColour);
                    Console.Write(spellRightArea);
                    spellsString = "";
                    bars++;

                }
                else if (count < 4 && bars != 3)
                {
                    spellsString = spellsString + spell.Name + ", ";
                }
                else if (bars != 3)
                {
                    spellsString = spellsString + spell.Name;
                    Console.Write("| ");
                    Console.Write(string.Format("{0,-40}", spellsString), _textColour);
                    Console.Write(spellRightArea);
                    spellsString = "";
                    bars++;
                }
                count++;    
            }
            for (int i = bars; i < 3; i++)
            {
                Console.Write(string.Format("| {0,-40}|                      |", ""));
            }
            Write(_bottomBar);

        }

        /// <summary>
        /// displays the victory text in the console, changing based on who won the battle.
        /// </summary>
        /// <param name="victor">Actor that won the battle.</param>
        /// <param name="outcome">outcome of the battle.</param>
        public void printVictoryText(IBattleActor victor, byte outcome) {
            switch (outcome)
            {
                case 1:
                    WriteLine(@"   ___ _                         __    __ _          ");
                    WriteLine(@"  / _ \ | __ _ _   _ ___ _ __   / / /\ \ (_)_ __  ___");
                    WriteLine(@" / /_)/ |/ _` | | | |/ _ \ '__| \ \/  \/ / | '_ \/ __|");
                    WriteLine(@"/ ___/| | (_| | |_| |  __/ |     \  /\  /| | | | \__ \");
                    WriteLine(@"\/    |_|\__,_|\__, |\___|_|      \/  \/ |_|_| |_|___/");
                    WriteLine(@"               |___/                                  ");
                    WriteLine();
                    WriteLine($"{victor.Name} Wins! {victor.HP}/{victor.MaxHP}HP remained!");
                    Read();
                    break;

                case 2:
                    WriteLine(@"  _____                             __        ___           ");
                    WriteLine(@" | ____|_ __   ___ _ __ ___  _   _  \ \      / (_)_ __  ___ ");
                    WriteLine(@" |  _| | '_ \ / _ \ '_ ` _ \| | | |  \ \ /\ / /| | '_ \/ __|");
                    WriteLine(@" | |___| | | |  __/ | | | | | |_| |   \ V  V / | | | | \__ \");
                    WriteLine(@" |_____|_| |_|\___|_| |_| |_|\__, |    \_/\_/  |_|_| |_|___/");
                    WriteLine(@"                             |___/                          ");
                    WriteLine();
                    WriteLine($"{victor.Name} Wins! {victor.HP}/{victor.MaxHP}HP remained!");
                    Read();
                    break;
                default:
                    Trace.WriteLine($"Something Wrong Happened to reach this. Outcome: {outcome} - {victor.ToString()}");
                    Read();
                    break;
            }
        }
    }
}





