using System.Collections;
using InputSystem;
using StarterAssets;
using UnityEngine;

public class Pickuble : MonoBehaviour
{
    private Animator _anim;
    private InputsHandler _inputHandler;
    
    private bool _pickedUp;
    private int _animIDTake;

    void Start ()
    {
        GameObject player = GameObject.FindWithTag("Chiusky");
        _anim = player.GetComponent<Animator>();
        _inputHandler = player.GetComponent<InputsHandler>();
        
        AssignAnimationIDs();
    }
    
    private void AssignAnimationIDs()
    {
        _animIDTake = Animator.StringToHash("take");
    }

    void OnTriggerStay(Collider player) {
        Debug.Log(player.tag);
        if (player.CompareTag("Chiusky"))
        {
            if (_inputHandler.interact && !_pickedUp)
            {
                _pickedUp = true;
                StartCoroutine(nameof(PlayAnim));
            }
        }
    }

    private IEnumerator PlayAnim()
    {
        _anim.SetTrigger(_animIDTake);
        yield return new WaitForSeconds(1.3f);
        Destroy(gameObject);
    }
}
