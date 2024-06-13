using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Utsuwa : NetworkBehaviour {
    public GameObject myPlayerSign;
    public bool isMine; // TODO:tukattenai?
    public int holderId; //TODO:nanikore
    public int CpuId; // TODO:nanikore

    public override void Spawned() {
        holderId = Object.StateAuthority.PlayerId;
        if (HasStateAuthority) {
            isMine = true;
            RuleManager.instance.myUtsuwa = this;
        } else {
            isMine = false;
            myPlayerSign.SetActive(false);
            RuleManager.instance.otherUtsuwaList.Add(this);
            CpuId = RuleManager.instance.otherUtsuwaList.Count - 1;
        }
    }

    public void SignEnabled(bool enabled) {
        if (!HasStateAuthority) {
            myPlayerSign.SetActive(false);
        } else {
            myPlayerSign.SetActive(enabled);
            myPlayerSign.GetComponent<UtsuwaPinIcon>().StartAnimation();
        }
    }
}