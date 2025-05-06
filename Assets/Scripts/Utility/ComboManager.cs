using UnityEngine;

public class ComboManager : MonoBehaviour
{

    public static ComboManager instance;

    public bool canReceiveInput;
    public bool inputReceived;

    void Awake()
    {
        instance = this;
    }

    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (canReceiveInput)
            {
                inputReceived = true;
                canReceiveInput = false;
            }
            else { return; }
        }
    }

    public void InputManager()
    {
        if (!canReceiveInput)
        {
            canReceiveInput = true;
        }
        else { canReceiveInput = false; }
    }
}