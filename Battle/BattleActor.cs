using System;
using static System.Console;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Prototype.Battle
{
    /// <summary>
    /// Represents an Actor taking place in a battle scene. Enemies use this class, though a specific <see cref="PlayerBattleActor"/> for the player inherits this class.
    /// Includes required stats and information for battle such as the name, ATK/DEF, HP,
    /// and weaknesses. Can be constructed with parameters from stats.json or with values. 
    /// </summary>
    public class BattleActor : IBattleActor
    {
        /// <summary>
        /// Gets or sets the name of the actor.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the current HP of the actor.
        /// </summary>
        public ushort HP { get; set; }
        /// <summary>
        /// Gets or sets the maximum HP of the actor.
        /// </summary>
        public ushort MaxHP { get; set; }
        /// <summary>
        /// Gets or sets the attack stat of the actor.
        /// </summary>
        public short Attack { get; set; }
        /// <summary>
        /// Gets or sets the defence stat of the actor.
        /// </summary>
        public short Defense { get; set; }
        /// <summary>
        /// Gets or sets the description of the actor.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the weakness of the actor. 'none' or 0 values indicate the actor does not have a weakness.
        /// </summary>
#nullable enable
        public Element Weakness { get; set; } = Element.none;
#nullable disable

        /// <summary>
        /// Initialises a new instance of the <see cref="BattleActor"/> class.
        /// </summary>
        /// <param name="name">Name of the Actor.</param>
        /// <param name="hp">Current and maximum HP of the actor.</param>
        /// <param name="atk">Attack stat of the actor.</param> <param name="def">Defense stat of the actor.</param>
        /// <param name="weakness">Weakness of the actor.</param>
        /// <param name="description">Description of the actor.</param>
        public BattleActor(string name, ushort hp, short atk, short def, string description, Element weakness = Element.none)
        {
            this.Name = name;
            this.MaxHP = hp;
            this.HP = hp;
            this.Attack = atk;
            this.Defense = def;
            this.Description = description;
            this.Weakness = weakness;
        }
        /// <summary>
        /// Creates an <see cref="BattleActor"/> instance with parameters (HP, ATK, Spells ect.) sourced from 
        /// stats.json according to the id argument provided. </summary>
        /// <exception cref="System.NullReferenceException">
        /// Thrown if the provided id does not exist in stats.json. </exception>
        /// /// <exception cref="System.IO.FileNotFoundException">
        /// Thrown if stats.json cannot be found. </exception>
        /// <param name="id">id from stats.json used to fill actor stats.</param>
        public BattleActor(string id)
        {
            string elementStr = ""; //used when casting string to Element enum
            JObject newActor = new JObject();
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    newActor = JObject.Parse(File.ReadAllText(@"data\stats.json"));
                }
                else
                {
                    newActor = JObject.Parse(File.ReadAllText(AppContext.BaseDirectory + @"data/stats.json"));
                }
            }
            catch (System.IO.FileNotFoundException e)
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{e.GetType()}\nERROR: {e.Message}");
                Console.WriteLine("The program will now close.");
                ReadKey();
                System.Environment.Exit(1);
            }

            try
            {
                Name = (string)newActor[id]["name"];
                HP = (ushort)newActor[id]["hp"];
                MaxHP = (ushort)newActor[id]["hp"];
                Attack = (short)newActor[id]["atk"];
                Defense = (short)newActor[id]["def"];
                Description = (string)newActor[id]["desc"];
                elementStr = (string)newActor[id]["weakness"] ?? "none";

            }
            catch (System.NullReferenceException e)
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{e.GetType()}: {e.Message}\nERROR: The Actor ID '" + id + "' does not exist in stats.json");
                Console.WriteLine("The program will now close.");
                ReadKey();
                System.Environment.Exit(1);
            }

            WriteLine("{0}'s weakness is: {1}", this.Name, this.Weakness);
            if (Enum.TryParse<Element>(elementStr, out Element elementEnum))
            {
                WriteLine("set {0}'s weakness to: {1}", this.Name, elementEnum);
                this.Weakness = elementEnum;
                WriteLine("{0}'s weakness is: {1}", this.Name, this.Weakness);
            }
            else
            {
                WriteLine("{0}'s weakness could not be set", this.Name);
                this.Weakness = elementEnum;
                WriteLine("{0}'s weakness is: {1}", this.Name, this.Weakness);
            }
        }

        /// <summary>
        /// Primary attack method to be used in battles. Automatically prevents overhealing, and sets HP to 0 if it falls below.
        /// Also prepares tuple messages to be used with <see cref="BattleWindowControl"/>.
        /// The <see cref="BattleActor"/> invoking this method is the 'attacker',
        /// the <see cref="Spell"/> arguement is the attack to be used, and the 
        /// <see cref="BattleActor"/> argument is the defending target of the attack.
        /// </summary>
        /// <param name="target">The target of the attack.</param>
        /// <param name="spell">The spell (attack) to be used in damage calculation.</param>
        /// <returns>A tuple containing one or two lines to be printed to the console.</returns>
        public Tuple<string, string> AttackActor(BattleActor target, Spell spell)
        {
            string message = "";
            string extra = null;
            var damage = spell.Damage;
            int newHP = HP;
            //Weakness calculation. If the spell element matches the actor weakness (except null element), deal more damage.
            if (spell.Element == target.Weakness && spell.Element != Element.none)
            {
                damage = (short)Math.Round(damage * 1.25);
                extra = $"Hit {target.Name}'s weakness!";
                newHP = target.HP - damage;
            }
            //heals automatically if used on self.
            switch (damage)
            {
                case < 0 when spell.Name == "Heal":
                    message = $"{Name} healed for {-damage} HP!";
                    newHP = HP - damage;
                    HP = (ushort)newHP;
                    break;
                case < 0 when spell.Name != "Heal":
                    damage = 0;
                    break;
                default:
                    message = $"{Name} used {spell.Name}! Dealt {damage} damage to {target.Name}!";
                    newHP = target.HP - damage;
                    break;
            }
            //prevents overhealing by setting HP equal to maxHP, if HP > maxHP.
            if (newHP > target.MaxHP)
            {
                HP = MaxHP;
            }
            //sets HP to 0 if it falls below that amount;
            else if (newHP < 0)
            {
                target.HP = 0;
            }
            else if (spell.Name != "Heal")
            {
                target.HP = (ushort)newHP;
            }
            return Tuple.Create(message, extra);
        }

        /// <summary>
        /// Summarises the actor information for the player to view. A null weakness will instead display "None."
        /// </summary>
        /// <returns>A formatted string with all information about the actor, designed to be printed to console. </returns>
        override public string ToString()
        {
            string weakLower = Weakness.ToString() ?? "None";
            string weakText = char.ToUpper(weakLower[0]) + weakLower.Substring(1);
            return $"~~STATUS INFORMATION~~\nName: {Name}\nHP: {HP} / {MaxHP}\nATK: {Attack} DEF: {Defense}\nWeakness: {weakText}";
        }
    }
}