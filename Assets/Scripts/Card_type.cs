using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_type
{
    public int ID;
    public string Describe;
    public int Level;
    public int ATK;
    public int HP;
    // Start is called before the first frame update
    public void Setting(int id,string describe,int level,int atk,int hp)
    {
        ID = id;
        Describe = describe;
        Level = level; ;
        ATK = atk;
        HP = hp;
    }
    public Card_type Clone()
    {
        return this.MemberwiseClone() as Card_type;
    }

}
