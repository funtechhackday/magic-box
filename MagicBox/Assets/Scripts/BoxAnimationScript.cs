using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxAnimationScript : MonoBehaviour {

    private Animator anim;

    void Awake() {
        anim = GetComponent<Animator>();
    }

    void Clicked() {
        Debug.Log("clicked on object");
        anim.SetBool("WasClicked", true);
    }
}
