using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Utsuwa : NetworkBehaviour {
    public GameObject myPlayerSign;
    public bool isMine;
    public int holderId;
    public int CpuId;

    public override void Spawned() {
        if (HasStateAuthority) {
            myPlayerSign.SetActive(false);
            RuleManager.instance.myUtsuwa = this;
        } else {
            RuleManager.instance.otherUtsuwaList.Add(this);
            CpuId = RuleManager.instance.otherUtsuwaList.Count - 1;
        }
    }

    public void SignEnabled(bool enabled) {
        if (HasStateAuthority) {
            myPlayerSign.SetActive(false);
        } else {
            myPlayerSign.SetActive(enabled);
            myPlayerSign.GetComponent<UtsuwaPinIcon>().StartAnimation();
        }
    }
}