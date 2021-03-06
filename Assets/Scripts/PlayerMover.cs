﻿using UnityEngine;
using System.Collections;
using DG.Tweening;
using UniRx;

/// <summary>
/// PlayerMover
/// プレイヤーの動作を管理するクラス
/// </summary>
public class PlayerMover : MonoBehaviour{

    #region[WalkParameter]
    private const float WALK_DISTANCE = 1.0f;//前進する距離
    private const float JUMP_POWER = 0.03f;//上下運動の力
    private const int JUMP_NUMBER = 10;//移動回数
    private const float WALK_DURATION = 8f;//移動にかかる時間
    #endregion

    #region[DamageParameter]
    private const float DAMAGE_DURATION = 1f;//ダメージを受けているときの時間
    private const float DAMAGE_POWER = 0.015f;//ダメージの振動の強さ
    private const int DAMAGE_SHAKE = 22;//ダメージの振動数
    private const int DAMAGE_SHAKE_ANGLERANGE = 20;//ダメージの振動の角度の散らばり
    #endregion

    //プレイヤーを歩行させる
    public void OnPlayerWalk()
    {
        //普段は普通に歩行、ボス戦前のみ左折処理を挟む

        if (StepManager.Instance.battleStep == 4)
        {
            Sequence seq = DOTween.Sequence();

            seq.Append(transform.DORotate(new Vector3(0f, -90f, 0f), 2f));

            seq.Append(transform.DOJump(new Vector3(-WALK_DISTANCE, 0f, 0f)*2F, JUMP_POWER, JUMP_NUMBER*2, WALK_DURATION*2f)
            .SetRelative()
            .SetEase(Ease.Linear))
            .OnComplete(() => StepManager.Instance.OnBattle());
        }
        else
        {
            transform.DOJump(new Vector3(0f, 0f, WALK_DISTANCE), JUMP_POWER, JUMP_NUMBER, WALK_DURATION)
                .SetRelative()
                .SetEase(Ease.Linear)
                .OnComplete(() => StepManager.Instance.OnBattle());
        }
    }

    //プレイヤーがダメージを受けた時の挙動
    public void OnPlayerDamage()
    {
        transform.DOKill();
        transform.DOShakePosition(DAMAGE_DURATION,DAMAGE_POWER,DAMAGE_SHAKE,DAMAGE_SHAKE_ANGLERANGE);
    }

}
