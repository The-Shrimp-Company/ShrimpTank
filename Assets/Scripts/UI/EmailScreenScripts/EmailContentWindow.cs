using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailContentWindow : MonoBehaviour
{
    private GameObject openEmail;

    public void OpenEmail(GameObject fullEmail, Email email)
    {
        if(openEmail != null)
        {
            Destroy(openEmail);
        }
        openEmail = Instantiate(fullEmail, transform);
        openEmail.GetComponent<FullEmail>().SetEmail(email);
    }
}
