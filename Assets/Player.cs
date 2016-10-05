using UnityEngine;
using UniRx;
using System.Collections;

public class Player{

    static Player _instance;
    private const int START_LIFE = 5;
    public IntReactiveProperty _life { get; private set;}

    public void LifeAffect(int point)
    {
        _life.Value -= point;
    }

    public static Player Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new Player();
                _instance._life = new IntReactiveProperty();
                _instance._life.Value = START_LIFE;

                _instance._life
                    .Skip(1)
                    .TakeWhile(x => x > 0)
                    .Subscribe(x => Debug.Log(x));
                
            }

            return _instance;
        }
    }

}
