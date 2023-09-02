using UnityEngine;
using System.Collections;
using Photon;

public class DisablePrefabsB : Photon.MonoBehaviour
{
    public GameObject[] prefabsToDisable;
    public GameObject losePrefab;

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

    public string targetTag = "ButtonPush"; // ターゲットとなるオブジェクトのタグを指定
    private int lastCount = 0; // 最後に確認したときのオブジェクトの数
    private int count = 0; // ターゲットとなるオブジェクトの総数
    private bool nextStagePrefabCreated = false;  // 追加


    void Start()
    {
        currentCamera = Instantiate(initialCameraPrefab, new Vector3(0, 0, -10), Quaternion.identity);
    }

    void Update()
    {
//----------------------勝者が決まった時そのIDを取得し、カメラを切り替えてクリックが利かないようにする----------------------------------

        bool currentFlagState = false;
        int winningPlayerID = -1;

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
                }

                StartCoroutine(SpawnAndDestroyZizou(winningPlayerID));
            }

        }

//----------------------ボタンを押した数をカウントする----------------------------------

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
        }

    }

//----------------------ボタンを押した数をカウントする2----------------------------------
     public int GetCount()
    {
        return count;
    }

    IEnumerator SpawnAndDestroyZizou(int winningPlayerID)
    {
//----------------------地蔵菩薩を出して消す----------------------------------
        yield return new WaitForSeconds(4f);
        zizouInstance = Instantiate(zizouPrefab, new Vector3(0, 0, -0.1f), Quaternion.identity);
        yield return new WaitForSeconds(6f);
        Destroy(zizouInstance);

//----------------------勝者のIDを取得し、勝者と敗者それぞれにルールプレハブを生成。勝者以外はクリック無効にする----------------------------------
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

    public void SceneChange()
{
    string testC_0822 = "testC_0822"; // 次のシーンの名前に書き換えてください
    photonView.RPC("RequestSceneChange", PhotonNetwork.masterClient, testC_0822);
}

IEnumerator NextStage()
{
    yield return new WaitForSeconds(4f);
    SceneChange();
}

    
}
