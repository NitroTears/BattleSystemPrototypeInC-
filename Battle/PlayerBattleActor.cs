using System;
using static System.Console;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Prototype.Battle
{
    /// <summary>
    /// A class used specifically for player controlled <see cref="BattleActor"/>'s. 
    /// Methods that the <see cref="BattleActor"/> would not be able to use are included in this class.
    /// This includes spells in a SpellList
    /// </summary>
    public class PlayerBattleActor : BattleActor, IBattleActor
    {
        /// <summary>
        /// Gets or sets the list of Spells the BattleActor has.
        /// </summary>
        public List<Spell> SpellList {get;} = new List<Spell>();

        /// <summary>
        /// Uses the id from stats.json to get a list of starting spells assigned to that id. All spells in the list are created and added to this <see cref="PlayerBattleActor"/>'s SpellList.
        /// </summary>
        /// <param name="id">id to be used with stats.json to get the list of spells assigned to the id.</param>
        public PlayerBattleActor(string id) : base(id) 
        {
            JObject jsonSpells = new JObject();
            if (OperatingSystem.IsWindows())
            {
                jsonSpells = JObject.Parse(File.ReadAllText(@"data\stats.json"));
            }
            else
            {
                jsonSpells = JObject.Parse(File.ReadAllText(AppContext.BaseDirectory + @"data/stats.json"));
            }

            var jsonList = jsonSpells[id]["spells"].ToObject<List<string>>();
            for (int i = 0; i < jsonList.Count; i++)
            {
                Spell spell = new Spell(jsonList[i]);
                AddSpell(spell);
            }
            Write("added to new actor: " + Name + "\n");
        }
       
        /// <summary>
        /// Adds a <see cref="Spell"/> to the <see cref="PlayerBattleActor"/>'s SpellList.
        /// </summary>
        /// <param name="spell">The Spell to be added to the SpellList.</param>
        public void AddSpell(Spell spell)
        {
            var exists = false;
            foreach (Spell s in SpellList) 
            {
                if (spell.Name.Equals(s.Name)) //checks if spell is already in spell list
                {
                    exists = true;
                    break;
                }
            }
            if (!exists) // if not, add it to the SpellList
            {
                SpellList.Add(spell);
                Write(spell.Name + ", ");
            }
        }

        /// <summary>
        /// prints a formatted list of all current spells in the <see cref="PlayerBattleActor"/>'s SpellList.
        /// Spells are displayed in increments of five, before printing on a new line.
        /// 
        /// </summary>
        public string printSpellList()
        {
            var spellsString = "";
            int count = 0;
            foreach (Spell spell in SpellList)
            {
                if (count.Equals(SpellList.Count - 1))
                {
                    spellsString = spellsString + spell.Name;
                }
                else if (count < 4)
                {
                    spellsString = spellsString + spell.Name + " - ";
                }
                else
                {
                    spellsString = spellsString + spell.Name + " - \n";
                }
                count++;
            }
            return spellsString;
        }
    }
}