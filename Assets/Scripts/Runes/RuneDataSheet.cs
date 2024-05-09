public class RuneData
{
    // Properties for rune attributes
    public float damage = 0; // Damage attribute of the rune
    public float range = 0; // Range attribute of the rune
    public float speed = 0; // Speed attribute of the rune
    public int mana = 0; // Mana cost of the rune

    // Constructor to initialize rune attributes
    public RuneData(float d, float r, float s, int m)
    {
        damage = d;
        range = r;
        speed = s;
        mana = m;
    }
}

public static class RuneDataSheet
{
    // Array to store rune data
    public static RuneData[] runeStats = new RuneData[8]
    {
        //damage, range, speed, mana
        new RuneData(4, 5, 3, 0), // Weak attack, no mana cost
        new RuneData(2, 7, 3, 1), // Further and faster, cheap mana cost
        new RuneData(3, 7, 5, 2), // More range and speed, moderate mana cost
        new RuneData(4, 2, 1, 5), // Damage increase, moderate mana cost
        new RuneData(6, 2, 1, 4), // Higher damage, high mana cost
        new RuneData(2, 2, 5, 3), // Faster speed, moderate mana cost
        new RuneData(5, 2, 3, 3), // Buffs all attributes, damage-oriented
        new RuneData(3, 5, 5, 5)  // Buffs all attributes, range-oriented
    };

    // Returns the highest damage value among all runes
    public static float GetHighestDamage()
    {
        float highest = 0;

        // Iterate through all runes to find the highest damage value
        foreach (RuneData rune in runeStats)
        {
            if (rune.damage > highest)
            { highest = rune.damage; }
        }

        return highest;
    }

    // Returns the highest range value among all runes
    public static float GetHighestRange()
    {
        float highest = 0;

        // Iterate through all runes to find the highest range value
        foreach (RuneData rune in runeStats)
        {
            if (rune.range > highest)
            { highest = rune.range; }
        }

        return highest;
    }

    // Returns the highest speed value among all runes
    public static float GetHighestSpeed()
    {
        float highest = 0;

        // Iterate through all runes to find the highest speed value
        foreach (RuneData rune in runeStats)
        {
            if (rune.speed > highest)
            { highest = rune.speed; }
        }

        return highest;
    }

    // Returns the highest mana cost among all runes
    public static int GetHighestMana()
    {
        int highest = 0;

        // Iterate through all runes to find the highest mana cost
        foreach (RuneData rune in runeStats)
        {
            if (rune.mana > highest)
            { highest = rune.mana; }
        }

        return highest;
    }
}
