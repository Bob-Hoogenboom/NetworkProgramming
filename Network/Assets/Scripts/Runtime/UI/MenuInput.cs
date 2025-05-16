using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class MenuInput : MonoBehaviour
{
    private EventSystem _system;
    public Selectable _firstElement;

    private void Start()
    {
        _system = EventSystem.current;
        //_firstElement = FindObjectOfType<Selectable>();
        _firstElement.Select();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKeyDown(KeyCode.LeftShift))
        {
            Selectable previousElement = _system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
            if(previousElement != null)
            {
                previousElement.Select();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable nextElement = _system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (nextElement != null)
            {
                nextElement.Select();
            }
        }
    }
}
