using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public interface IBattleMessage : IEventSystemHandler{

    void OnBattle(int step);

}
