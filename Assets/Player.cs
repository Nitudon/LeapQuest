using UnityEngine;
using UniRx;
using System.Collections;

public class Player{
    /*
     * Player
     * ライフなどのPlayer情報管理
     * 
      */
    public readonly int START_LIFE = 10;
    public IntReactiveProperty _life { get; private set; }

    public Player()
    {
        _life = new IntReactiveProperty();
        _life.Value = START_LIFE;
    }

    public void LifeAffect(int point)
    {
        _life.Value -= point;
    }

}
