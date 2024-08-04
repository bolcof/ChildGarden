using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class ForceRestarter : MonoBehaviour {
    public static ForceRestarter instance;

    [SerializeField] private float inactivityTime;
    [SerializeField] private float timer = 0f;
    [SerializeField] private List<GameObject> MustDestroyObject;
    public bool ableForceRestart;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Update() {
        if (SceneManager.GetActiveScene().name != "Launcher") {
            if (Input.anyKeyDown || Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2) || Input.mouseScrollDelta.y != 0) {
                timer = 0f;
            } else {
                timer += Time.deltaTime;
                if (timer >= inactivityTime) {
                    Debug.Log(inactivityTime.ToString() + "秒間の無操作を検知しました。");
                    OnInactivityDetected();
                    timer = 0f;
                }
            }

            if (Input.GetKeyDown(KeyCode.R)) {
                Debug.Log("Rでの再起動");
                OnInactivityDetected();
                timer = 0f;
            }
        }

        if (Input.GetKey(KeyCode.Escape)) {

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
        }
    }

    
    public void OnInactivityDetected() {
        Debug.Log("OnInactivityDetected");
        if (ableForceRestart) {
            RoomBreakAndRestart();
        }
    }

    public void OnlyRestart() {
        SoundManager.Instance.AllSoundStop();
        foreach (var obj in MustDestroyObject) {
            Destroy(obj);
        }
        SoundManager.Instance.PlayBgm(SoundManager.Instance.BGM_Title);
        Debug.Log("MyDebug Resterter");
        SceneManager.LoadScene("Restarter");
    }

    public void RoomBreakAndRestart() {
        RoomConector.Instance.networkRunner.Shutdown();
        OnlyRestart();
    }
}