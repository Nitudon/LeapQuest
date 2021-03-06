﻿using UnityEngine;
using DG.Tweening;
using System.Collections;

public class RockController : MonoBehaviour {

    /// <summary>
    /// 岩のコントローラースクリプト
    /// </summary>

    
    private Vector3 enemyPos;//敵の位置
    private Vector3 cameraPos;//プレイヤーの位置

    [SerializeField]
    private GameObject ExplosionParticle;//爆散エフェクト

    [HideInInspector]
    public bool isReflect { get; private set; }//反射フラグ

    enum pathDirection {player,enemy}

    void Start()
    {
        enemyPos = gameObject.transform.position;
        isReflect = false;

        gameObject.transform.DOPath(RockPath(pathDirection.player), 3f, PathType.CatmullRom);
           
    }

    //岩の軌道
    Vector3[] RockPath(pathDirection dir)
    {
        //ボスからプレイヤー、プレイヤーからボスへの岩の軌道を演算し返す

        Vector3[] _path;
        cameraPos = GameObject.FindGameObjectWithTag("Player").transform.position;

        Vector3 rockPos = enemyPos;
        Vector3 goal = new Vector3(cameraPos.x, Random.Range(cameraPos.y-0.1f, cameraPos.y-0.05f), Random.Range(cameraPos.z - 0.1f, cameraPos.z + 0.1f));
        Vector3 intercept = rockPos + (goal - rockPos) * 0.5f + new Vector3(0f, 0.18f, 0f);

        if(dir == pathDirection.player)
        {
            _path = new Vector3[]{ intercept,goal};
        }

        else
        {
            _path = new Vector3[] { intercept, enemyPos };
        }

        return _path;
    }


    void OnTriggerEnter(Collider collider)
    {
        //プレイヤーにあたったとき、反射させたときの処理
        if(collider.tag == "Player")
        {
            gameObject.transform.DOKill();
            Destroy(gameObject);
            PlayerUIManager.Instance.LifeAffect(1);
        }

        else if(collider.tag == "Hand" && !isReflect)
        {
            gameObject.transform.DOKill();
            if (this.tag == "Rock")
            {
                isReflect = true;
                EnemyManager.Instance.EnemySoundEffect(AudioManager.EnemySE.Guard);
                gameObject.transform.DOPath(RockPath(pathDirection.enemy), 3f, PathType.CatmullRom);
            }

            else if(this.tag == "FireRock")
            {
                Instantiate(ExplosionParticle,transform.position,ExplosionParticle.transform.rotation);
                Destroy(gameObject);
            }
        }

    }

}
