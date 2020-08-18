using DjaOC.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DjaOC.Example
{
    public class Character
    {

    }

    [Singleton]
    [ID("hero")]
    public class Hero : Character
    {
        private Weapon _weapon;
        private Armor _armor;

        public Weapon Weapon { get { return _weapon; } }
        public Armor Armor { get { return _armor; } }

        [Inject]
        public Hero([ID("heroDefault")] Weapon weapon, [ID("heroDefault")] Armor armor)
        {
            _weapon = weapon;
            _armor = armor;
        }
    }

    public class Weapon
    {
        private int _atk;
        public int Atk { get { return _atk; } }
        public Weapon(int atk)
        {
            _atk = atk;
        }
    }

    [Prototype]
    [ID("heroDefault")]
    public class HeroDefaultWeapon : Weapon
    {
        [Inject]
        public HeroDefaultWeapon() : base(5)
        {

        }
    }

    public class Armor
    {
        private int _def;
        
        public int Def { get { return _def; } }
        public Armor(int def)
        {
            _def = def;
        }
    }

    [Prototype]
    [ID("heroDefault")]
    public class HeroDefaultArmor : Armor
    {
        [Inject]
        public HeroDefaultArmor() : base(5)
        {

        }
    }
}
