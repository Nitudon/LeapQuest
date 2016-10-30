using UnityEngine;
using UniRx;
using System.Collections;

public class Player{
    /*
     * Player
     * ライフなどのPlayer情報管理
     * 
      */
    public readonly int START_LIFE = 25;
    public IntReactiveProperty _life { get; private set; }

    public Player()
    {
        _life = new IntReactiveProperty();
        _life.Value = START_LIFE;
    }

    //ダメージ処理
    public void LifeAffect(int point)
    {
        _life.Value -= point;
    }

}
