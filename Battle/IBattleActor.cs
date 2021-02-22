using System;

namespace Prototype.Battle
{
    /// <summary>
    /// An interface for BattleActor's so ensure all required stats an an attack method exists.
    /// </summary>
    public interface IBattleActor
    {
        short Attack { get; set; }
        short Defense { get; set; }
        string Description { get; set; }
        ushort HP { get; set; }
        ushort MaxHP { get; set; }
        string Name { get; set; }
        Element Weakness { get; set; }

        Tuple<string, string> AttackActor(BattleActor target, Spell spell);
    }
}