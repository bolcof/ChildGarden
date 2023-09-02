using UnityEngine;
using System.Collections;
using Photon;

public class Stone_Stage : Photon.MonoBehaviour
{
    public GameObject effectPrefab; // このプレハブにはエフェクトを設定します
    public GameObject playerPrefab; // プレイヤーオブジェクトのプレハブ
    private Vector2 originalPosition; // オブジェクトが初めて生成された時の位置

    private void Start()
    {
        originalPosition = transform.position;
    }

    /// <summary>
    /// トリガーに触れた時
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerEnter2D(Collider2D collider)
    {
        // トリガーに触れた相手にPlayerタグが付いているとき
        if (collider.gameObject.tag == "Player")
        {
            // エフェクトを発生させる
            PhotonNetwork.Instantiate(effectPrefab.name, collider.transform.position, Quaternion.identity, 0);

            // 当たった相手をすぐに削除
            PhotonNetwork.Destroy(collider.gameObject);

            // 1秒後にオブジェクトを再生成
            Invoke("RespawnObject", 1.0f);
        }
    }

    private void RespawnObject()
    {
        var respawnedPlayer = PhotonNetwork.Instantiate(playerPrefab.name, originalPosition, Quaternion.identity, 0);

        // リスポーンしたプレハブに特殊な動作を付加する
        StartCoroutine(DisableColliderAndDestroyObjects());
    }

    private IEnumerator DisableColliderAndDestroyObjects()
    {
        var boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            boxCollider.enabled = false;

            // "Onbutu"タグのオブジェクトをすべて取得して削除
            GameObject[] objects = GameObject.FindGameObjectsWithTag("Onbutu");
            foreach (var obj in objects)
            {
                PhotonNetwork.Destroy(obj);
            }

            yield return new WaitForSeconds(2.0f);
            boxCollider.enabled = true;
        }
    }
}