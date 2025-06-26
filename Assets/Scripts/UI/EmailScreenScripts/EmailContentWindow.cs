using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailContentWindow : MonoBehaviour
{
    private GameObject openEmail;
    private EmailContentBlock oldParent;

    public void OpenEmail(GameObject fullEmail, Email email, EmailContentBlock parent)
    {
        if(openEmail != null)
        {
            Destroy(openEmail);
        }
        if (oldParent != null)
        {
            oldParent.GetAnimator().SetBool("Open", false);
        }
        oldParent = parent;
        openEmail = Instantiate(fullEmail, transform);
        openEmail.GetComponent<FullEmail>().SetEmail(email);
        oldParent.GetAnimator().SetBool("Open", true);
    }
}
