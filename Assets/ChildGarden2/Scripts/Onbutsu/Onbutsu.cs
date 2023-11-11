﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Onbutsu : MonoBehaviour {
    public int holderId, StagingId;
    [SerializeField] private int spawnedId;

    public int onbutsuSize;
    public bool hasLand_Utsuwa;
    public bool landing_Utsuwa;
    public bool dropped;

    private float stoppingTime = 0f;
    private float threshold = 1.0f;
    private Rigidbody2D rb;

    public float checkRadius = 1.0f;
    public int touchingObjectsCount = 0;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private PolygonCollider2D _polygonCollider;
    [SerializeField] private CircleCollider2D _circleCollider;

    private void Awake() {
        hasLand_Utsuwa = false;
        landing_Utsuwa = false;
        dropped = false;

        PhotonView photonView = GetComponent<PhotonView>();
        holderId = photonView.owner.ID;
        StagingId = -1;

        rb = GetComponent<Rigidbody2D>();

        RuleManager.instance.OnbutsuList.Add(this);
        spawnedId = RuleManager.instance.OnbutsuList.Count - 1;
    }
    void Update() {
        if (!dropped) {
            if (rb.velocity.magnitude <= threshold) {
                if (!landing_Utsuwa) {
                    stoppingTime += Time.deltaTime;
                    if (stoppingTime >= 0.5f) {
                        landing_Utsuwa = true;

                        List<Utsuwa> compareUtsuwaList = new List<Utsuwa>();
                        compareUtsuwaList.Add(RuleManager.instance.myUtsuwa);
                        for (int i = 0; i < RuleManager.instance.otherUtsuwaList.Count; i++) {
                            compareUtsuwaList.Add(RuleManager.instance.otherUtsuwaList[i]);
                        }

                        int tmpStagingId = RoomConector.Instance.MyPlayerId();
                        float minDistance = Vector2.Distance(RuleManager.instance.myUtsuwa.transform.position, transform.position);

                        for (int i = 1; i < compareUtsuwaList.Count; i++) {
                            if (minDistance > Vector2.Distance(compareUtsuwaList[i].transform.position, transform.position)) {
                                tmpStagingId = compareUtsuwaList[i].holderId;
                            }
                        }

                        StagingId = tmpStagingId;
                    }
                }
            } else {
                stoppingTime = 0f; // 動いている場合、タイマーをリセット
                landing_Utsuwa = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        switch (collision.gameObject.tag) {
            case "Utsuwa":
                hasLand_Utsuwa = true;
                break;
            case "Onbutu":
                if (collision.gameObject.GetComponent<Onbutsu>().landing_Utsuwa) {
                    hasLand_Utsuwa = true;
                }
                break;
            case "Floor":
                Debug.Log("Touch Floor int switch");
                Dropped();
                break;
            default:
                Debug.Log("Touch " + collision.gameObject.tag);
                break;
        }
    }
    private int CountTouchingObjects() {
        // 現在のGameObjectの位置を中心にした球形領域内の全てのColliderを取得
        Collider[] colliders = Physics.OverlapSphere(transform.position, checkRadius);

        // 自分自身のColliderを除外する
        int count = 0;
        foreach (Collider col in colliders) {
            if (col.gameObject != gameObject) {
                count++;
            }
        }
        return count;
    }

    private void Dropped() {
        dropped = true;
        landing_Utsuwa = false;
        if (_polygonCollider != null) {
            _polygonCollider.enabled = false;
        }
        if (_circleCollider != null) {
            _circleCollider.enabled = false;
        }
        _spriteRenderer.enabled = false;
    }
}
