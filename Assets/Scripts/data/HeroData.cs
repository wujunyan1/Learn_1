using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroData
{
    int level;
    int id;
    
    public HeroData(int _id, int _level = 0)
    {
        id = _id;
        level = _level;
    }


    public int GetId()
    {
        return id;
    }

    public int GetLevel()
    {
        return level;
    }

    // override object.Equals
    public override bool Equals(object obj)
    {
        //       
        // See the full list of guidelines at
        //   http://go.microsoft.com/fwlink/?LinkID=85237  
        // and also the guidance for operator== at
        //   http://go.microsoft.com/fwlink/?LinkId=85238
        //

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        // TODO: write your implementation of Equals() here
        HeroData hero = (HeroData)obj;
        if(hero.id != this.id || hero.level != this.level)
        {
            return false;
        }

        return true;
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
        // level 不可能会超过 2^10
        return (id >> 10) + level;
    }
}
