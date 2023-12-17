using UnityEngine;
using UnityEngine.UI;

class GameMaster : MonoBehaviour
{
    public Text KeyContent;

    public InputField inputField;

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public string GetInput()
    {
        return inputField.text;
    }
}
