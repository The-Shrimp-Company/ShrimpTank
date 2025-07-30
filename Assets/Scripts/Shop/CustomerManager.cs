using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Rendering;
using System.IO;
using System.Text.Json.Serialization;

public class CustomerManager : MonoBehaviour 
{
    static public CustomerManager Instance;

    private int count = 0;
    private int coolDown = 0;
    private float openSaleCoolDownCount = 0;

    [SerializeField]
    private float openSaleCoolDown = 5;

    public List<TankController> openTanks = new();
    public List<Shrimp> ToPurchase { get; private set; } = new List<Shrimp>();


    public List<Request> requests = new() ;

    private List<string> RandomEmails = new();

    public EmailScreen emailScreen;
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        foreach (string line in File.ReadLines(Path.Combine(Application.streamingAssetsPath, "NPCEmailAddresses.txt")))
        {
            RandomEmails.Add(line);
        }
    }

    private void Update()
    {
        if (openSaleCoolDownCount > (openSaleCoolDown/openTanks.Count))
        {
            if (count < openTanks.Count)
            {
                TankController currentTank = openTanks[count];
                foreach (Shrimp shrimp in currentTank.shrimpInTank)
                {
                    float value = EconomyManager.instance.GetShrimpValue(shrimp.stats);
                    float chance = currentTank.openTankPrice / value;
                    //Debug.Log(chance);
                    if (Random.value * 2 > chance)
                    {
                        PurchaseShrimp(shrimp);
                        break;
                    }
                }
            }

            count++;


            if (count >= openTanks.Count)
            {
                count = 0;
            }
            openSaleCoolDownCount = 0;
        }
        openSaleCoolDownCount += Time.deltaTime;

        if(Random.Range(0, 1000) == 1 && requests.Count < 5 && coolDown < 0 && ShrimpManager.instance.allShrimp.Count > 5)
        {
            coolDown = 300;
            MakeRequest();
        }
        coolDown--;
    }



    public void AddShrimpToPurchase(Shrimp shrimp)
    {
        ToPurchase.Add(shrimp);

        UIManager.instance.GetScreen()?.GetComponent<SellScreenView>()?.UpdateList(shrimp);
    }

    public void PurchaseShrimp(Shrimp shrimp)
    {
        if (shrimp != null)
        {
            shrimp.tank.shrimpToRemove.Add(shrimp);
            Money.instance.AddMoney(shrimp.tank.openTankPrice);
            Reputation.AddReputation(0.6f - shrimp.stats.illnessLevel / 100);
            EconomyManager.instance.UpdateTraitValues(false, shrimp.stats);

            Email email = EmailTools.CreateEmail();
            email.title = "Admin@admin.ShrimpCo.com";
            email.subjectLine = shrimp.stats.name + " has been sold";
            email.mainText = shrimp.stats.name + " was in " + shrimp.tank.tankName + "\n£" + shrimp.tank.openTankPrice + " has been deposited into your account";
            EmailManager.SendEmail(email);

            PlayerStats.stats.shrimpSold++;
            Destroy(shrimp.gameObject);
        }
    }

    public void PurchaseShrimp(Shrimp shrimp, float value)
    {
        if (shrimp != null)
        {
            ToPurchase.Remove(shrimp);
            shrimp.tank.shrimpToRemove.Add(shrimp);
            Debug.Log(value);
            Money.instance.AddMoney(value);
            EconomyManager.instance.UpdateTraitValues(false, shrimp.stats);
            Destroy(shrimp.gameObject);

            Reputation.AddReputation(0.6f - shrimp.stats.illnessLevel / 100);
            Debug.Log("Reputation: " + Reputation.GetReputation());

            Email email = EmailTools.CreateEmail();
            int index = Random.Range(0, RandomEmails.Count);
            email.title = RandomEmails[index];
            email.subjectLine = "I Love this shrimp!";
            email.mainText = "It's just what I wanted, so I got you this bonus!";
            email.value = Mathf.RoundToInt(shrimp.GetValue());
            MyButton button = email.CreateEmailButton("Add money", true);
            button.SetFunc(EmailFunctions.FunctionIndexes.AddMoney, email.value);
            EmailManager.SendEmail(email, true, Random.Range(10, 30));
        }
    }

    public void HardPurchaseShrimp(Shrimp shrimp, float value)
    {
        if (shrimp != null)
        {
            ToPurchase.Remove(shrimp);
            shrimp.tank.shrimpToRemove.Add(shrimp);
            Money.instance.AddMoney(value);
            EconomyManager.instance.UpdateTraitValues(false, shrimp.stats);
            Destroy(shrimp.gameObject);

            Reputation.AddReputation(0.6f - shrimp.stats.illnessLevel / 100);
        }
    }

    public void MakeRequest()
    {
        ShrimpStats s = ShrimpManager.instance.CreateRequestShrimp();
        ShrimpStats obfs = s;
        string message = "";

        List<int> traits = new List<int>{ 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        int loop = 0;

        do
        {
            loop += 1;
            int index = traits[Random.Range(0, traits.Count)];
            traits.Remove(index);
            message += RequestPrepend();
            switch (index)
            {
                case 1:
                    obfs.pattern.obfuscated = true;
                    message += GeneManager.instance.GetTraitSO(obfs.pattern.activeGene.ID).traitName;
                    break;
                case 2:
                    obfs.body.obfuscated = true;
                    message += GeneManager.instance.GetTraitSO(obfs.body.activeGene.ID).traitName;
                    break;
                case 3:
                    obfs.head.obfuscated = true;
                    message += GeneManager.instance.GetTraitSO(obfs.head.activeGene.ID).traitName;
                    break;
                case 4:
                    obfs.secondaryColour.obfuscated = true;
                    message += GeneManager.instance.GetTraitSO(obfs.secondaryColour.activeGene.ID).traitName + " secondary colour";
                    break;
                case 5:
                    obfs.primaryColour.obfuscated = true;
                    message += GeneManager.instance.GetTraitSO(obfs.primaryColour.activeGene.ID).traitName + " primary colour";
                    break;
                case 6:
                    obfs.legs.obfuscated = true;
                    message += GeneManager.instance.GetTraitSO(obfs.legs.activeGene.ID).traitName;
                    break;
                case 7:
                    obfs.tail.obfuscated = true;
                    message += GeneManager.instance.GetTraitSO(obfs.tail.activeGene.ID).traitName;
                    break;
                case 8:
                    obfs.tailFan.obfuscated = true;
                    message += GeneManager.instance.GetTraitSO(obfs.tailFan.activeGene.ID).traitName;
                    break;
                case 9:
                    obfs.eyes.obfuscated = true;
                    message += GeneManager.instance.GetTraitSO(obfs.eyes.activeGene.ID).traitName;
                    break;

            }
            message += ".\n";
        } while (loop < 4 && Random.Range(1, 5) > 1);


        float value = EconomyManager.instance.GetObfsShrimpValue(obfs);
        message += "I will pay " + value + " plus a bonus if the shrimp is very good";

        Request request = new () {
            stats = s,
            obfstats = obfs,
            value = value
        };
        Email email = EmailTools.CreateEmail();
        int emailIndex = Random.Range(0, RandomEmails.Count);
        email.title = RandomEmails[emailIndex];
        email.subjectLine = "I would like a shrimp";
        email.mainText = message;
        MyButton button = email.CreateEmailButton("Choose Shrimp");
        button.SetFunc(EmailFunctions.FunctionIndexes.CompleteRequest, request);
        EmailManager.SendEmail(email, true);
        request.emailID = email.ID;
        requests.Add(request);
    }

    public void EmailOpen(EmailScreen newEmailScreen)
    {
        Debug.Log("Recieved");
        emailScreen = newEmailScreen;
    }

    public void CompleteRequest(Request request)
    {
        requests.Remove(request);
    }
    
    private string RequestPrepend()
    {
        string message;

        message = Request.Words[Random.Range(0, Request.Words.Length)];

        return message;
    }
}



[System.Serializable]
public class Request
{
    public float value;

    public ShrimpStats stats;
    public ShrimpStats obfstats;

    public int emailID;


    static public string[] Words = { 
    "I would like a shrimp with ",
    "It should have ",
    "Please can it have ",
    "My most favorite type of shrimp have ",
    "Please make sure it has ",
    "I will cry if it doesn't have "
    };


    public void OpenShrimpSelection()
    {
        CustomerManager.Instance.emailScreen.OpenSelection(this);
    }

    
}
