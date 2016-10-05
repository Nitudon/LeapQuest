using UnityEngine;
using System.Collections;
using DG.Tweening;
using UniRx;

[RequireComponent(typeof(Animator))]
public class PlayerMover : MonoBehaviour,IPlayerMessage {

    private Animator PlayAnimation;

    public void OnPlayerMove(string trigger)
    {
        PlayAnimation.SetTrigger(trigger);
    }

    void Start()
    {
        PlayAnimation = GetComponent<Animator>();
    }

}
