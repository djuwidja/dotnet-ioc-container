using DjaOC;
using System;
using System.Collections.Generic;
using System.Text;

namespace DjaOC.Example
{
    public class Example
    {
        public static void StaticInstantiation()
        {
            StaticContainer.Instantiate();
            StaticContainer.BindNewInstance<Weapon, HeroDefaultWeapon>();
            StaticContainer.BindNewInstance<Armor, HeroDefaultArmor>();
            StaticContainer.BindNewInstance<Character, Hero>();

            Hero hero = (Hero)StaticContainer.Get<Character>("hero");
            Console.WriteLine(string.Format("This hero has weapon with atk = {0} and armor with def = {1}.", hero.Weapon.Atk, hero.Armor.Def));

            StaticContainer.Dispose();
        }

        public static void InstancedInstantiation()
        {
            const string legendId = "legend";

            Injector injector = new Injector();
            injector.BindNewInstance<Weapon, HeroDefaultWeapon>();
            injector.BindNewInstance<Armor, HeroDefaultArmor>();
            injector.BindNewInstance<Character, Hero>();

            Weapon excalibur = new Weapon(8);
            injector.Bind(excalibur, "excalibur");

            Armor genjiArmor = new Armor(8);
            injector.Bind(genjiArmor, "genjiArmor");

            injector.Bind(new Hero(excalibur, genjiArmor), legendId);

            Hero legend = injector.Get<Hero>(legendId);
            Console.WriteLine(string.Format("This legend has weapon with atk = {0} and armor with def = {1}.", legend.Weapon.Atk, legend.Armor.Def));
        }
    }
}
