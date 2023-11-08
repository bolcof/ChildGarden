using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Photon;

public class CheckStaticObjects : PunBehaviour
{
    public float checkInterval = 1.0f;
    public float detectionRadius = 5.0f;
    private const int minStaticObjects = 5;
    private List<GameObject> objectsInRadius = new List<GameObject>();

    private Dictionary<GameObject, float> staticTimeCounter = new Dictionary<GameObject, float>();

    private bool sharedFlag = false;
    public bool SharedFlag
    {
        get { return sharedFlag; }
    }

    private int previousStaticCount = 0;
    private bool hasSpawned = false;

    private bool winPlayer = false;
    private bool losePlayer = false;

    public bool WinPlayer
    {
        get { return winPlayer; }
    }

    public bool LosePlayer
    {
        get { return losePlayer; }
    }

    private int playerID;

    void Start()
    {
        playerID = PhotonNetwork.player.ID;

        if (photonView.isMine)
        {
            InvokeRepeating("CheckForStaticObjects", checkInterval, checkInterval);
        }
    }

    void CheckForStaticObjects()
    {
        objectsInRadius.Clear();

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("Onbutu"))
            {
                var ownerID = hitCollider.gameObject.GetComponent<PhotonView>().owner.ID;
                if (ownerID == playerID)
                {
                    objectsInRadius.Add(hitCollider.gameObject);
                }
            }
        }

        foreach (var obj in objectsInRadius)
        {
            var rb2d = obj.GetComponent<Rigidbody2D>();
            if (rb2d.velocity.magnitude < 0.01f)
            {
                if (staticTimeCounter.ContainsKey(obj))
                {
                    staticTimeCounter[obj] += checkInterval;
                }
                else
                {
                    staticTimeCounter.Add(obj, checkInterval);
                }
            }
            else
            {
                if (staticTimeCounter.ContainsKey(obj))
                {
                    staticTimeCounter.Remove(obj);
                }
            }
        }

        int staticCount = staticTimeCounter.Count(kv => kv.Value >= 3.0f);

        if (staticCount != previousStaticCount)
        {
            Debug.Log($"指定範囲内の「Onbutu」というタグのプレハブオブジェクトの現在の数: {staticCount}。");
            previousStaticCount = staticCount;
        }

        if (staticCount >= minStaticObjects && !hasSpawned)
        {
            photonView.RPC("SetSharedFlagTrue", PhotonTargets.AllBuffered);
            photonView.RPC("SpawnDifferentPrefabs", PhotonTargets.All);
            hasSpawned = true;
            Debug.Log("目標達成");
        }
    }

    [PunRPC]
    void SpawnDifferentPrefabs()
    {
        winPlayer = true;
        photonView.RPC("SetPlayerState", PhotonTargets.Others, playerID, winPlayer);
        Debug.Log("勝利");
    }

    [PunRPC]
    void SetPlayerState(int otherPlayerID, bool otherWinPlayer)
    {
        if (playerID != otherPlayerID && otherWinPlayer)
        {
            losePlayer = true;
            Debug.Log("敗北");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    [PunRPC]
    void SetSharedFlagTrue()
    {
        sharedFlag = true;
    }

    public bool GetSharedFlag()
    {
        return sharedFlag;
    }
    public bool GetWinPlayer()
    {
        return winPlayer;
    }

    public bool GetLosePlayer()
    {
        return losePlayer;
    }
}