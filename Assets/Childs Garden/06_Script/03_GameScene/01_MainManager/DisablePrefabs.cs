using UnityEngine;
using System.Collections;
using Photon;
using TMPro;
using System.Linq;

public class DisablePrefabs : Photon.MonoBehaviour
{
    public GameObject[] prefabsToDisable;
    public GameObject losePrefab;
    //public GameObject loseCoinPrefab;

    [System.Serializable]
    public class SharedFlagSource
    {
        public GameObject prefab;
        public UnityEngine.MonoBehaviour sourceComponent;
        public string sharedFlagMethodName = "GetSharedFlag";
        public string winningPlayerIDMethodName = "GetWinningPlayerID";
    }
    public SharedFlagSource[] sharedFlagSources;

    private bool previousFlagState = false;

    public GameObject initialCameraPrefab;
    public GameObject alternateCameraPrefab;
    public GameObject zizouPrefab;
    public GameObject ruleSelectPrefab;
    public GameObject ruleWaitingPrefab;
    public GameObject ruleButton;
    public GameObject prefabNextStage;

    private GameObject currentCamera;
    private GameObject zizouInstance;

    public string targetTag = "ButtonPush";
    private int lastCount = 0;
    private int count = 0;
    private bool nextStagePrefabCreated = false;

    public TextMeshProUGUI countdownText;
    private float countdownTime = 90.0f;

    private int winningPlayerID = -1;  // この行を追加
    void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        currentCamera = Instantiate(initialCameraPrefab, new Vector3(0, 0, -10), Quaternion.identity);
    }

    void Update()
    {
//----------------------勝者が決まった時そのIDを取得し、カメラを切り替えてクリックが利かないようにする----------------------------------

         bool currentFlagState = false;

        foreach (var flagSource in sharedFlagSources)
        {
            GameObject clone = GameObject.Find(flagSource.prefab.name + "(Clone)");
            if (clone)
            {
                UnityEngine.MonoBehaviour sourceComponent = clone.GetComponent(flagSource.sourceComponent.GetType()) as UnityEngine.MonoBehaviour;
                if (sourceComponent)
                {
                    bool sharedFlag = (bool)sourceComponent.GetType().GetMethod(flagSource.sharedFlagMethodName).Invoke(sourceComponent, null);
                    currentFlagState = currentFlagState || sharedFlag;

                    if (sharedFlag)
                    {
                        winningPlayerID = (int)sourceComponent.GetType().GetMethod(flagSource.winningPlayerIDMethodName).Invoke(sourceComponent, null);
                    }
                }
            }
        }

        if (currentFlagState)
        {
            foreach (GameObject prefab in prefabsToDisable)
            {
                GameObject clone = GameObject.Find(prefab.name + "(Clone)");
                if (clone)
                {
                    Destroy(clone);
                }
            }

            if (!previousFlagState)
            {
                Debug.Log("地蔵召喚条件をすべてOFFにします");
                previousFlagState = true;

                Destroy(currentCamera);
                currentCamera = Instantiate(alternateCameraPrefab, new Vector3(0, 0, -10), Quaternion.identity);

                if (PhotonNetwork.player.ID != winningPlayerID)
                {
                    Instantiate(losePrefab, new Vector3(0, 0, -0.1f), Quaternion.identity);
                    //Instantiate(loseCoinPrefab, new Vector3(0, 0, -0.1f), Quaternion.identity);
                }

                StartCoroutine(SpawnAndDestroyZizou(winningPlayerID));
               // IncreaseWinningPlayerCount(winningPlayerID);  // <-- この行を追加
            }

        }

//----------------------ルール選択ボタンを押した数をカウントする----------------------------------

        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(targetTag);
        // ターゲットのタグを持つオブジェクトの数が増加している場合、カウントを増やす
        if (objectsWithTag.Length > lastCount)
        {
            count += (objectsWithTag.Length - lastCount);
            Debug.Log("現状のint値: " + count);
        }

        lastCount = objectsWithTag.Length;

       // int値が2になった瞬間にprefabNextStageを一つだけ生成する
        if (count == 2 && !nextStagePrefabCreated)
        {
            Instantiate(prefabNextStage, new Vector3(0, 0, 0), Quaternion.identity);
            nextStagePrefabCreated = true;
            StartCoroutine(NextStage());
            count = 0;
        }

    }
//----------------------カウントダウンタイマー---------------------------------
    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(5.0f);  // 5秒待つ
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        while (countdownTime > 0)
        {
            //分と秒に変換
            int minutes = Mathf.FloorToInt(countdownTime / 60);
            int seconds = Mathf.FloorToInt(countdownTime % 60);

            countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            yield return new WaitForSeconds(1.0f);

            countdownTime -= 1.0f;
        }

        countdownText.text = "00:00";

       RequestSceneChange();
    }

   

//----------------------ルール選択ボタンを押した数を返す----------------------------------
     public int GetCount()
    {
        return count;
    }

//----------------------地蔵菩薩を出して消す----------------------------------------------------
    IEnumerator SpawnAndDestroyZizou(int winningPlayerID)
    {
    yield return new WaitForSeconds(4f);
    zizouInstance = Instantiate(zizouPrefab, new Vector3(0, 0, -0.1f), Quaternion.identity);
    yield return new WaitForSeconds(10f);
    Destroy(zizouInstance);


    // 現在のシーンが "testC_0822" であるかのチェックを追加
    if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "testC_0822")
    {
        Debug.Log("Current scene is testC_0822");
    }


//----------------------勝者のIDを取得し、勝者と敗者それぞれにルールプレハブを生成。勝者以外はボタンのついてないパネルを出す----------------------------------
        if (PhotonNetwork.player.ID == winningPlayerID)
        {
            Instantiate(ruleSelectPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            PhotonNetwork.Instantiate(this.ruleButton.name, Vector3.zero, Quaternion.identity, 0);
        }
        else
        {
            Instantiate(ruleWaitingPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
//-----------------------Photonを用いたシーン移動------------------------------

 // この関数を呼び出すと、マスタークライアントにシーンの変更をリクエストします。
    public void RequestSceneChange()
    {
        // RPCを使用して、MasterSceneChangerAのRequestSceneChangeを呼び出します。
        photonView.RPC("RequestSceneChangeRPC", PhotonTargets.MasterClient);
    }

    [PunRPC]
    private void RequestSceneChangeRPC()
    {
        // この部分はMasterClientのみ実行します。
        StaticSceneManager sceneChanger = FindObjectOfType<StaticSceneManager>();
        if (sceneChanger != null)
        {
            sceneChanger.ChangeToNextScene();
        }
    }

    IEnumerator NextStage()
{
    yield return new WaitForSeconds(4f);
    RequestSceneChange();
}

//-----------------------すべてのプレイヤーのIDを取得しタグの数を検索する ----------------------------------
   
    
}
