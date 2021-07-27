using System;
using System.Collections;
using InputSystem;
using UnityEngine;

public class ChangeView : MonoBehaviour
{
    public GameObject cam;
    private InputsHandler _inputHandler;
    private GameObject _chiusky;
    private ChiuskyController _chiuskyController;

    private void Awake()
    {
        if (!_chiusky)
        {
            _chiusky = GameObject.FindGameObjectWithTag("Chiusky");
        }
    }

    private void Start()
    {
        if (!"CC00".Equals(cam.tag))
        {
            cam.SetActive(false);
        }
        _chiuskyController = _chiusky.GetComponent<ChiuskyController>();
        _inputHandler = _chiusky.GetComponent<InputsHandler>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!"Chiusky".Equals(other.gameObject.tag)) return;
        
        var lastCamera = _chiuskyController.GetCamera();
        if (cam.Equals(lastCamera)) return;
        cam.SetActive(true);
        InvokeRepeating("WaitForChangeCamera",0,0.4f);
        /*while (_inputHandler.isMove)
        {
            Debug.Log("waiting ...");
        }*/
        
    }

    private void WaitForChangeCamera()
    {
        if (_inputHandler.isMove) return;
        var lastCamera = _chiuskyController.GetCamera();
        _chiuskyController.SetCamera(cam);
        lastCamera.SetActive(false);
        CancelInvoke("WaitForChangeCamera");
    }
}
