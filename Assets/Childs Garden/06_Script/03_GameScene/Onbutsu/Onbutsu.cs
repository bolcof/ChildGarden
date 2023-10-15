using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Onbutsu : MonoBehaviour {
    public int holderId, StagingId;

    public bool hasLand_Utsuwa;
    public bool landing_Utsuwa;
    public bool dropped;

    private float stoppingTime = 0f;
    [SerializeField] float threshold = 0.01f;
    private Rigidbody rb;

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

        rb = GetComponent<Rigidbody>();

        RuleManager.instance.OnbutsuList.Add(this);
    }
    void Update() {
        if (!dropped) {
            if (rb.velocity.magnitude <= threshold) {
                stoppingTime += Time.deltaTime;
                if (stoppingTime >= 0.5f) {
                    landing_Utsuwa = true;

                    float myUtsuwaDistance = Vector2.Distance(RuleManager.instance.myUtsuwa.transform.position, transform.position);
                    float otherUtsuwaDistance = Vector2.Distance(RuleManager.instance.otherUtsuwa.transform.position, transform.position);

                    if (myUtsuwaDistance <= otherUtsuwaDistance) {
                        StagingId = MatchingStateManager.instance.MyPlayerId();
                    } else {
                        if (MatchingStateManager.instance.MyPlayerId() == 1) {
                            StagingId = 2;
                        } else {
                            StagingId = 1;
                        }
                    }

                    stoppingTime = 0f; // タイマーをリセットする（何度も静止の判断を行いたい場合）
                }
            } else {
                stoppingTime = 0f; // 動いている場合、タイマーをリセット
                landing_Utsuwa = false;
            }
        }

        /*if (hasLand_Utsuwa) {
            touchingObjectsCount = CountTouchingObjects();
            if(touchingObjectsCount == 0) {
                Landing_Utsuwa = false;
            }
        }*/
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Floor")) {
            Dropped();
        }
        switch(collision.gameObject.tag) {
            case "Utsuwa":
                hasLand_Utsuwa = true;
                break;
            case "Onbutu":
                if (collision.gameObject.GetComponent<Onbutsu>().landing_Utsuwa) {
                    hasLand_Utsuwa = true;
                }
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
        if (_polygonCollider != null) {
            _polygonCollider.enabled = false;
        }
        if(_circleCollider != null) {
            _circleCollider.enabled = false;
        }
        _spriteRenderer.enabled = false;
    }
}
