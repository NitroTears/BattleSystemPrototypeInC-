using System;
using static System.Console;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Prototype.Battle
{
    /// <summary>
    /// Represents a spell or attack that a <see cref="BattleActor"/> may use in battle against another <see cref="BattleActor"/>.
    /// Includes the name of the spell, the damage it will do, it's the element, and a description.
    /// and weaknesses. Can be constructed with parameters from stats.json or with values. 
    /// </summary>
    public class Spell
    {
        /// <summary>
        /// Gets or sets the ID of the spell.
        /// </summary>
        public string ID {get;}
        /// <summary>
        /// Gets or sets the name of the spell.
        /// </summary>
        public string Name {get; set;}
        /// <summary>
        /// Gets or sets the description of the spell.
        /// </summary>
        public string Description {get; set;}
        /// <summary>
        /// Gets or sets the damage of the spell, negative values are used for healing instead of damage.
        /// </summary>
        public short Damage { get; set; }
        /// <summary>
        /// Gets or sets the element of the spell. This value is nullable, and if so, indicates this spell does not have an element.
        /// </summary>
#nullable enable
        public Element Element { get; set; } // This could easily be an enum for future projects.
#nullable disable
        /// <summary>
        /// Initialises a new instance of the <see cref="Spell"/> class.
        /// </summary>
        /// <param name="name">Name of the Spell.</param>
        /// <param name="damage">Damage of the spell. Negative values are used for healing instead of damage.</param>
        /// <param name="desc">Description of the Spell</param>
        /// <param name="id">ID of the Spell</param>
        /// <param name="element">Element of the spell</param>
        public Spell(string name, short damage, string desc, string id, Element element = Element.none) 
        {
            this.Name = name;
            this.Element = element;
            this.Description = desc;
            this.Damage = damage;
            this.ID = id;
        }
        /// <summary>
        /// Creates a spell with parameters (name, damage, element ect.) sourced from 
        /// spells.json according to the id argument provided.
        /// </summary>
        /// <exception cref="System.NullReferenceException">
        /// Thrown if the provided id does not exist in spells.json. </exception>
        /// <exception cref="System.IO.FileNotFoundException">
        /// Thrown if spells.json cannot be found. </exception>
        /// <param name="id">id from spells.json used to fill spell stats.</param>
        public Spell(string id)
        {
            string elementStr = ""; //used when casting string to Element enum
            JObject newSpell = new JObject();

            try
            {
                if (OperatingSystem.IsWindows())
                {
                    newSpell = JObject.Parse(File.ReadAllText(@"data\spells.json"));
                }
                else
                {
                    newSpell = JObject.Parse(File.ReadAllText(AppContext.BaseDirectory + @"data/spells.json"));
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
                Name = (string)newSpell[id]["name"];
                Damage = (short)newSpell[id]["damage"];
                Description = (string)newSpell[id]["desc"];
                elementStr = (string)newSpell[id]["element"] ?? "none";
            }
            catch (System.NullReferenceException e)
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{e.GetType()}: {e.Message}\nERROR: The Spell ID '" + id + "' does not exist in spells.json");

                Console.WriteLine("The program will now close.");
                ReadKey();
                System.Environment.Exit(1);
            }
            ID = id;

            WriteLine("{0}'s element is: {1}", this.Name, this.Element);
            if (Enum.TryParse<Element>(elementStr, out Element elementEnum))
            {
                WriteLine("set {0}'s element to: {1}", this.Name, elementEnum);
                this.Element = elementEnum;
                WriteLine("{0}'s element is: {1}", this.Name, this.Element);
            }
            else
            {
                WriteLine("{0}'s element could not be set", this.Name);
                this.Element = elementEnum;
                WriteLine("{0}'s element is: {1}", this.Name, this.Element);
            }
        }

        /// <summary>
        /// Summarises the spell information to console. A null element will instead display "None."
        /// </summary>
        /// <returns>A formatted string with all information about the actor, designed to be printed to console. </returns>
        override public string ToString() {
            string elementStr = Element.ToString() == "none" ? "None" : Element.ToString(); //if element is 'none, print 'None.', else print the element name.
            string descText = Description ?? "No Data Available.";
            string elemText = char.ToUpper(elementStr[0]) + elementStr.Substring(1);
            return $"~~~'{Name}' Spell~~~\nDamage: {Damage} Element: {elemText}\nDescription: {descText}";
        }
    }
}
