using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Device;

public class EmailContentBlock : ContentBlock
{
    [SerializeField] private TextMeshProUGUI title, subjectLine;
    [SerializeField] private GameObject fullEmail;

    private EmailContentWindow window;

    private Animator animator;
    
    private Email _email;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!EmailManager.instance.emails.Contains(_email))
        {
            Destroy(transform.parent.gameObject);
        }
    }

    public void SetEmail(Email email, EmailContentWindow newWindow)
    {
        _email = email;
        if (email.sender != null)
        {
            title.text = _email.sender;
        }
        else
        {
            title.text = _email.title;
        }
        subjectLine.text = _email.subjectLine;
        FontTools.SizeFont(title);
        FontTools.SizeFont(subjectLine);
        subjectLine.fontSize *= 0.8f;
        window = newWindow;
    }

    public void Click()
    {
        window.OpenEmail(fullEmail, _email, this);
    }

    public void DeleteEmail()
    {
        EmailManager.RemoveEmail(_email);
    }

    public Email GetEmail()
    {
        return _email;
    }

    public Animator GetAnimator() { return animator; }
}
