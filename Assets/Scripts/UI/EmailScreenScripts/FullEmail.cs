using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FullEmail : MonoBehaviour
{
    [SerializeField] private GameObject _button;
    private List<Button> buttons = new List<Button>();
    [SerializeField] private Transform buttonParent;
    [SerializeField] private GameObject deleteButton;

    [SerializeField] private TextMeshProUGUI title, subject, body;

    private Email _email;

    public void SetEmail(Email email)
    {
        _email = email;
        EmailManager.instance.openEmail = this;
        body.text = email.mainText;
        title.text = email.title;
        subject.text = email.subjectLine;
        body.fontSize = 30;
        if(email.buttons != null)
        {
            Debug.Log("Buttons");
            foreach(MyButton button in email.buttons)
            {
                Debug.Log("One button");
                GameObject obj = Instantiate(_button, buttonParent);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = button.text;
                FontTools.SizeFont(obj.GetComponentInChildren<TextMeshProUGUI>());
                obj.GetComponent<Button>().onClick.AddListener(button.action);
                if(button.destroy == true)
                {
                    obj.GetComponent<Button>().onClick.AddListener(DeleteEmail);
                }
                buttons.Add(obj.GetComponent<Button>());
            }
        }
        if (!email.important)
        {
            deleteButton.SetActive(true);
        }
        else
        {
            deleteButton.SetActive(false);
        }
    }

    public void DeleteEmail()
    {
        EmailManager.RemoveEmail(_email);
        Destroy(transform.gameObject);
    }
}
