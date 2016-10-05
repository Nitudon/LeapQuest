using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public interface IPlayerMessage : IEventSystemHandler {

    void OnPlayerMove(string trigger);

}
