using UnityEngine;
using DG.Tweening;
using System.Collections;

public class RockController : MonoBehaviour {

    private Vector3 enemyPos;
    private Vector3 cameraPos;

    public bool isReflect;

    enum pathDirection {player,enemy}

    void Start()
    {
        enemyPos = gameObject.transform.position;
        isReflect = false;
        gameObject.transform.DOPath(RockPath(pathDirection.player), 3f, PathType.CatmullRom);
           
    }

    Vector3[] RockPath(pathDirection dir)
    {
        Vector3[] _path;
        cameraPos = GameObject.FindGameObjectWithTag("Player").transform.position;

        Vector3 rockPos = enemyPos;
        Vector3 goal = new Vector3(Random.Range(cameraPos.x - 0.1f, cameraPos.x + 0.1f), Random.Range(cameraPos.y, cameraPos.y + 0.05f), cameraPos.z);
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
        if(collider.tag == "Player")
        {
            gameObject.transform.DOKill();
            Destroy(gameObject);
            CharacterManager.Instance.PlayerDamage(1);
        }

        else if(collider.tag == "Hand")
        {
            isReflect = true;
            gameObject.transform.DOKill();
            gameObject.transform.DOPath(RockPath(pathDirection.enemy), 3f, PathType.CatmullRom)
           .OnComplete(() => Destroy(gameObject.transform.gameObject));
        }

        else if(collider.tag == "Boss" && isReflect)
        {
            gameObject.transform.DOKill();
            Destroy(gameObject);

        }
    }

}
