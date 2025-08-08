using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;

public class ShrimpSelectionPopulation : ContentPopulation
{
    private Request _request;
    private EmailScreen _window;
    private Email _email;

    public void Populate(Request request, EmailScreen window)
    {
        _request = request;
        _window = window;
        _email = EmailManager.instance.emails.Find((x) => { return x.ID == request.emailID; });
        List<Shrimp> shrimp = ShrimpManager.instance.allShrimp;
        if (request.obfstats.tail.obfuscated)
        {
            shrimp = shrimp.Where(x => x.stats.tail.activeGene.ID == request.stats.tail.activeGene.ID).ToList();
        }
        if (request.obfstats.primaryColour.obfuscated)
        {
            shrimp = shrimp.Where(x => x.stats.primaryColour.activeGene.ID == request.stats.primaryColour.activeGene.ID).ToList();
        }
        if (request.obfstats.tailFan.obfuscated)
        {
            shrimp = shrimp.Where(x => x.stats.tailFan.activeGene.ID == request.stats.tailFan.activeGene.ID).ToList();
        }
        if (request.obfstats.body.obfuscated)
        {
            shrimp = shrimp.Where(x => x.stats.body.activeGene.ID == request.stats.body.activeGene.ID).ToList();
        }
        if (request.obfstats.secondaryColour.obfuscated)
        {
            shrimp = shrimp.Where(x => x.stats.secondaryColour.activeGene.ID == request.stats.secondaryColour.activeGene.ID).ToList();
        }
        if (request.obfstats.eyes.obfuscated)
        {
            shrimp = shrimp.Where(x => x.stats.eyes.activeGene.ID == request.stats.eyes.activeGene.ID).ToList();
        }
        if (request.obfstats.pattern.obfuscated)
        {
            shrimp = shrimp.Where(x => x.stats.pattern.activeGene.ID == request.stats.pattern.activeGene.ID).ToList();
        }
        if (request.obfstats.legs.obfuscated)
        {
            shrimp = shrimp.Where(x => x.stats.legs.activeGene.ID == request.stats.legs.activeGene.ID).ToList();
        }

        foreach(Shrimp s in shrimp)
        {
            GameObject block = Instantiate(contentBlock, transform);
            block.GetComponent<ShrimpSelectionBlock>().Populate(s.stats);
            contentBlocks.Add(block.GetComponent<ContentBlock>());
            s.currentValue = EconomyManager.instance.GetObfsShrimpValue(request.obfstats);
            block.GetComponent<Button>().onClick.AddListener(s.SellShrimp);
            block.GetComponent<Button>().onClick.AddListener(CompleteRequest);
        }
    }

    public void PopulateFull(float price, EmailScreen emailScreen, Email email)
    {
        _window = emailScreen;
        _email = email;
        foreach (Shrimp s in ShrimpManager.instance.allShrimp)
        {
            GameObject block = Instantiate(contentBlock, transform);
            block.GetComponent<ShrimpSelectionBlock>().Populate(s.stats);
            contentBlocks.Add(block.GetComponent<ContentBlock>());
            s.currentValue = price;
            Email TempEmail = email;
            TempEmail.sender = _email.sender;
            ShrimpStats TempStats = s.stats;
            block.GetComponent<Button>().onClick.AddListener(s.HardSellShrimp);
            block.GetComponent<Button>().onClick.AddListener(() =>
            {
                if(NPCManager.Instance.GetNPCFromName(TempEmail.sender) != null)
                {
                    NPCManager.Instance.GetNPCFromName(TempEmail.sender).BoughtShrimp(TempStats);
                }
                foreach (Email email in EmailManager.instance.emails)
                {
                    if (email.ID == TempEmail.ID)
                    {
                        EmailManager.RemoveEmail(email);
                        break;
                    }
                }
                emailScreen.CloseSelection();
            });
        }
    }

    public void PopulateExcluding(EmailScreen emailScreen, Email email, List<ShrimpStats> shrimpToExclude)
    {
        _window = emailScreen;
        _email = email;
        List<Shrimp> shrimpToShow = new();
        if(shrimpToExclude.Count > 0)
        {
            foreach(Shrimp s in ShrimpManager.instance.allShrimp)
            {
                bool flag = true;
                foreach(ShrimpStats check in shrimpToExclude)
                {
                    if (s.stats.CompareTraits(check)) flag = false;
                }
                if (flag) shrimpToShow.Add(s);
            }
        }
        else
        {
            shrimpToShow = ShrimpManager.instance.allShrimp;
        }

        foreach (Shrimp s in shrimpToShow)
        {
            GameObject block = Instantiate(contentBlock, transform);
            block.GetComponent<ShrimpSelectionBlock>().Populate(s.stats);
            contentBlocks.Add(block.GetComponent<ContentBlock>());
            s.currentValue = EconomyManager.instance.GetShrimpValue(s.stats) * 1.5f;
            Email TempEmail = _email;
            TempEmail.sender = _email.sender;
            ShrimpStats TempStats = s.stats;
            block.GetComponent<Button>().onClick.AddListener(s.SellShrimp);
            block.GetComponent<Button>().onClick.AddListener(() =>
            {
                if(NPCManager.Instance.GetNPCFromName(TempEmail.sender) != null)
                {
                    NPCManager.Instance.GetNPCFromName(TempEmail.sender).BoughtShrimp(TempStats);
                }
                foreach (Email email in EmailManager.instance.emails)
                {
                    if (email.ID == TempEmail.ID)
                    {
                        EmailManager.RemoveEmail(email);
                        break;
                    }
                }
                emailScreen.CloseSelection();
            });
        }
    }
    protected void CreateContent()
    {
        foreach ( Shrimp shrimp in ShrimpManager.instance.allShrimp)
        {
            GameObject block = Instantiate(contentBlock, transform);
            block.GetComponent<ShrimpSelectionBlock>().Populate(shrimp.stats);
            contentBlocks.Add(block.GetComponent<ContentBlock>());
        }
    }

    public void CompleteRequest()
    {
        CustomerManager.Instance.CompleteRequest(_request);
        PlayerStats.stats.requestsCompleted++;
        foreach(Email email in EmailManager.instance.emails)
        {
            if(email.ID == _email.ID)
            {
                EmailManager.RemoveEmail(email);
                break;
            }
        }
        _window.CloseSelection();
    }

    public void CloseScreen()
    {
        _window.CloseSelection();
    }
}
