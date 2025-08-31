using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FullEmail : MonoBehaviour
{
    [SerializeField] private GameObject _button;
    private List<Button> buttons = new List<Button>();
    [SerializeField] private Transform buttonParent;
    [SerializeField] private GameObject deleteButton;

    [SerializeField] private TextMeshProUGUI title, subject, body;

    public string ID;

    private Email _email;

    public void SetEmail(Email email)
    {
        _email = email;
        ID = _email.ID;
        EmailManager.instance.openEmail = this;
        body.text = email.mainText;
        title.text = "From:" + email.sender ?? email.title ?? "WhoKnows";
        subject.text = "Subject Line:" + email.subjectLine;
        body.fontSize = 30;
        if(email.buttons != null)
        {
            foreach(MyButton button in email.buttons)
            {
                GameObject obj = Instantiate(_button, buttonParent);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = button.text;
                FontTools.SizeFont(obj.GetComponentInChildren<TextMeshProUGUI>());
                button.ButtonFunc();
                foreach(UnityAction action in button.actions)
                {
                    obj.GetComponent<Button>().onClick.AddListener(action);
                }
                if(button.destroy == true)
                {
                    obj.GetComponent<Button>().onClick.AddListener(DeleteEmail);
                }
                buttons.Add(obj.GetComponent<Button>());
            }
        }
        if (email.tag == Email.EmailTags.Spam)
        {
            deleteButton.SetActive(true);
        }
        else
        {
            deleteButton.SetActive(false);
        }
    }

    private void Update()
    {
        body.text = _email.mainText;
        title.text = "From:" + _email.sender ?? _email.title ?? "WhoKnows";
        subject.text = "Subject Line:" + _email.subjectLine;
        if(_email.buttons.Count != buttons.Count)
        {
            if (_email.buttons != null)
            {
                foreach (MyButton button in _email.buttons)
                {
                    GameObject obj = Instantiate(_button, buttonParent);
                    obj.GetComponentInChildren<TextMeshProUGUI>().text = button.text;
                    FontTools.SizeFont(obj.GetComponentInChildren<TextMeshProUGUI>());
                    button.ButtonFunc();
                    foreach (UnityAction action in button.actions)
                    {
                        obj.GetComponent<Button>().onClick.AddListener(action);
                    }
                    if (button.destroy == true)
                    {
                        obj.GetComponent<Button>().onClick.AddListener(DeleteEmail);
                    }
                    buttons.Add(obj.GetComponent<Button>());
                }
            }
            if (_email.tag == Email.EmailTags.Spam)
            {
                deleteButton.SetActive(true);
            }
            else
            {
                deleteButton.SetActive(false);
            }
        }
    }

    public void DeleteEmail()
    {
        EmailManager.RemoveEmail(_email);
        Destroy(transform.gameObject);
    }

}
