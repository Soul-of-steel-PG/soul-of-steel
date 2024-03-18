using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Movement : MonoBehaviour {
    [SerializeField] private float velocity = 0.1f;

    public PhotonView pv;

    private void Start() {
        pv = GetComponent<PhotonView>();
    }

    private void Update() {
        OnMovement();
    }

    private void OnMovement() {
        if (pv.IsMine) {
            if (Input.GetKey(KeyCode.W)) {
                transform.position = new Vector3(transform.position.x, transform.position.y + velocity);
            }
            
            if (Input.GetKey(KeyCode.S)) {
                transform.position = new Vector3(transform.position.x, transform.position.y - velocity);
            }
            
            if (Input.GetKey(KeyCode.A)) {
                transform.position = new Vector3(transform.position.x - velocity, transform.position.y);
            }
            
            if (Input.GetKey(KeyCode.D)) {
                transform.position = new Vector3(transform.position.x + velocity, transform.position.y);
            }
        }
        
    }
}
