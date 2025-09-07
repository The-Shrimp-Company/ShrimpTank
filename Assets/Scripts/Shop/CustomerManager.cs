using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using System.IO;
using System.Text.Json.Serialization;
using System;

public class ShrimpShopSlot
{
    public Shrimp shrimp;
    public float value;
}

public class CustomerManager : MonoBehaviour 
{

    static public CustomerManager Instance;

    private int count = 0;
    private int coolDown = 0;
    private float openSaleCoolDownCount = 0;

    [SerializeField]
    private float openSaleCoolDown = 10;

    [SerializeField] private float playerStoreTimer = 40;
    private float playerStoreTime = 0;


    public List<TankController> openTanks = new();
    public List<Shrimp> ToPurchase { get; private set; } = new List<Shrimp>();


    public List<Request> requests = new() ;

    private List<string> RandomEmails = new();

    public EmailScreen emailScreen;

    public ShrimpShopSlot[] shrimpSaleSlots = new ShrimpShopSlot[10];
    public int numSlots = 3;


    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    private void Update()
    {
        // Section looking at selling from open tanks
        if (openSaleCoolDownCount > (openSaleCoolDown/openTanks.Count))
        {
            if (count < openTanks.Count)
            {
                TankController currentTank = openTanks[count];
                foreach (Shrimp shrimp in currentTank.shrimpInTank)
                {
                    float value = EconomyManager.instance.GetShrimpValue(shrimp.stats);
                    if (!shrimp.stats.bornInStore)
                    {
                        value /= 4;
                    }
                    if (UnityEngine.Random.value * 20 < value)
                    {
                        PurchaseShrimp(shrimp, value / UnityEngine.Random.Range(0.8f, 1.5f));
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

        // Section handling requests given to the player
        if(UnityEngine.Random.Range(0, 1000) == 1 && requests.Count < 5 && coolDown < 0 && ShrimpManager.instance.allShrimp.Count > 5 && Reputation.GetReputation() > 40)
        {
            coolDown = 300;
            MakeRequest();
        }
        coolDown--;

        // Section handling player store
        if(playerStoreTime > playerStoreTimer)
        {
            // Only run if there are slots available, and if any of those slots have shrimp in them
            List<ShrimpShopSlot> activeSlots = shrimpSaleSlots.Where(x => x is not null && x.value != 0).ToList();
            if(numSlots != 0 && activeSlots.Count > 0)
            {
                ShrimpShopSlot currentSlot = null;
                int lookedAtIndex;
                do
                {
                    lookedAtIndex = UnityEngine.Random.Range(0, activeSlots.Count);
                    currentSlot = activeSlots[lookedAtIndex];
                } while (currentSlot.shrimp == null && currentSlot.value == 0);
                if (EconomyManager.instance.GetShrimpValue(currentSlot.shrimp.stats) > currentSlot.value - UnityEngine.Random.Range(1, Reputation.GetReputation() / 10 + 1))
                {
                    currentSlot.shrimp.illnessCont.RemoveShrimp();
                    PurchaseShrimp(currentSlot.shrimp, currentSlot.value);
                    shrimpSaleSlots[Array.IndexOf(shrimpSaleSlots, currentSlot)] = new();
                }
                else
                {
                    Email complaint = EmailTools.CreateEmail();
                    complaint.title = RandomEmails[UnityEngine.Random.Range(0, RandomEmails.Count)];
                    complaint.subjectLine = currentSlot.shrimp.stats.name + " cost too much, so I didn't buy it";
                    complaint.mainText = "I'm mad " + currentSlot.shrimp.stats.name +  " cost so much. Grr";
                    complaint.CreateEmailButton("I don't care!", true);
                    EmailManager.SendEmail(complaint, true);
                }
            }
            playerStoreTime = 0;
            
        }
        else
        {
            playerStoreTime += Time.deltaTime;
        }
    }

    public void Initialize(Request[] requests = null)
    {

        Instance.requests = requests?.ToList() ?? Instance.requests;

        foreach (string line in File.ReadLines(Path.Combine(Application.streamingAssetsPath, "NPCEmailAddresses.txt")))
        {
            Instance.RandomEmails.Add(line);
        }
    }

    

    public void PurchaseShrimp(Shrimp shrimp, float value = 1)
    {
        if (shrimp != null)
        {
            shrimp.tank.shrimpToRemove.Add(shrimp);
            Money.instance.AddMoney(value);
            Reputation.AddReputation(0.6f - shrimp.stats.illnessLevel / 100);
            EconomyManager.instance.UpdateTraitValues(false, shrimp.stats);

            Email email = EmailTools.CreateEmail();
            email.title = "Admin@admin.ShrimpCo.com";
            email.subjectLine = shrimp.stats.name + " has been sold";
            email.mainText = shrimp.stats.name + " was in " + shrimp.tank.tankName;
            EmailManager.SendEmail(email);

            PlayerStats.stats.shrimpSold++;
        }
    }

    public void PurchaseShrimpThroughRequest(Shrimp shrimp, float value)
    {
        if (shrimp != null)
        {
            ToPurchase.Remove(shrimp);
            shrimp.tank.shrimpToRemove.Add(shrimp);
            Money.instance.AddMoney(value);
            EconomyManager.instance.UpdateTraitValues(false, shrimp.stats);

            Reputation.AddReputation(0.6f - shrimp.stats.illnessLevel / 100);

            Email email = EmailTools.CreateEmail();
            int index = UnityEngine.Random.Range(0, RandomEmails.Count);
            email.title = RandomEmails[index];
            email.subjectLine = "I Love this shrimp!";
            email.mainText = "It's just what I wanted, so I got you this bonus!";
            email.value = Mathf.RoundToInt(shrimp.GetValue());
            MyButton button = email.CreateEmailButton("Add money", true);
            button.SetFunc(EmailFunctions.FunctionIndexes.AddMoney, email.value);
            EmailManager.SendEmail(email, true, UnityEngine.Random.Range(10, 30));
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

            Reputation.AddReputation(0.6f - shrimp.stats.illnessLevel / 100);
        }
    }

    public void MakeRequest()
    {
        ShrimpStats s = ShrimpManager.instance.CreateRequestShrimp();
        ShrimpStats obfs = s;
        string message = "";

        /*
        List<int> traits = new List<int>{ 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        int loop = 0;

        do
        {
            loop += 1;
            int index = traits[Random.Range(0, traits.Count)];
            traits.Remove(index);
            switch (index)
            {
                case 1:
                    obfs.pattern.obfuscated = true;
                    break;
                case 2:
                    obfs.body.obfuscated = true;
                    break;
                case 3:
                    obfs.head.obfuscated = true;
                    break;
                case 4:
                    obfs.secondaryColour.obfuscated = true;
                    break;
                case 5:
                    obfs.primaryColour.obfuscated = true;
                    break;
                case 6:
                    obfs.legs.obfuscated = true;
                    break;
                case 7:
                    obfs.tail.obfuscated = true;
                    break;
                case 8:
                    obfs.tailFan.obfuscated = true;
                    break;
                case 9:
                    obfs.eyes.obfuscated = true;
                    break;

            }
        } while (loop < 4 && Random.Range(1, 5) > 1);
        */

        message += "I would like a " + obfs.GetBreedname();

        float value = EconomyManager.instance.GetObfsShrimpValue(obfs);
        message += "\nI will pay " + value + " plus a bonus if the shrimp is very good";

        Request request = new () {
            stats = s,
            obfstats = obfs,
            value = value
        };
        Email email = EmailTools.CreateEmail();
        int emailIndex = UnityEngine.Random.Range(0, RandomEmails.Count);
        email.title = RandomEmails[emailIndex];
        email.subjectLine = "I would like a shrimp";
        email.mainText = message;
        email.CreateEmailButton("Choose Shrimp")
            .SetFunc(EmailFunctions.FunctionIndexes.CompleteRequest, request);
        email.CreateEmailButton("I don't want to sell you that, actually.", true)
            .SetFunc(EmailFunctions.FunctionIndexes.RejectRequest, request);
        EmailManager.SendEmail(email, true);
        request.emailID = email.ID;
        requests.Add(request);
    }

    public void EmailOpen(EmailScreen newEmailScreen)
    {
        emailScreen = newEmailScreen;
    }

    public void CompleteRequest(Request request)
    {
        requests.Remove(request);
    }
    
    private string RequestPrepend()
    {
        string message;

        message = Request.Words[UnityEngine.Random.Range(0, Request.Words.Length)];

        return message;
    }
}



[System.Serializable]
public class Request
{
    public float value;

    public ShrimpStats stats;
    public ShrimpStats obfstats;

    public string emailID;


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
