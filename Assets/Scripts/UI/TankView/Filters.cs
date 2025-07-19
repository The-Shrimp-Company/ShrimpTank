using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

public class Filters : MonoBehaviour
{
    private List<MyDropdown> filters = new List<MyDropdown>();

    private List<string> traits = new List<string>();

    [SerializeField] private TankViewScript tank;

    private int maxTraitCount;

    // Start is called before the first frame update
    void Start()
    {
        filters = GetComponentsInChildren<MyDropdown>().ToList();
        traits = GeneManager.instance.allTraits;
        traits.Add("A");
        traits.Add("C");
        maxTraitCount = traits.Count;
        traits.Clear();

        foreach(MyDropdown filter in filters)
        {
            switch (filter.name)
            {
                case "LegsFilter":
                    foreach(TraitSO trait in GeneManager.instance.legsSOs)
                    {
                        MyDropdown.OptionData option = new MyDropdown.OptionData(trait.traitName);
                        SetOptionFunc(option, trait.ID);
                        filter.options.Add(option);
                    }
                    break;
                case "BodyFilter":
                    foreach (TraitSO trait in GeneManager.instance.bodySOs)
                    {
                        MyDropdown.OptionData option = new MyDropdown.OptionData(trait.traitName);
                        SetOptionFunc(option, trait.ID);
                        filter.options.Add(option);
                    }
                    break;
                case "EyesFilter":
                    foreach (TraitSO trait in GeneManager.instance.eyeSOs)
                    {
                        MyDropdown.OptionData option = new MyDropdown.OptionData(trait.traitName);
                        SetOptionFunc(option, trait.ID);
                        filter.options.Add(option);
                    }
                    break;
                case "HeadFilter":
                    foreach (TraitSO trait in GeneManager.instance.headSOs)
                    {
                        MyDropdown.OptionData option = new MyDropdown.OptionData(trait.traitName);
                        SetOptionFunc(option, trait.ID);
                        filter.options.Add(option);
                    }
                    break;
                case "TailFilter":
                    foreach (TraitSO trait in GeneManager.instance.tailSOs)
                    {
                        MyDropdown.OptionData option = new MyDropdown.OptionData(trait.traitName);
                        SetOptionFunc(option, trait.ID);
                        filter.options.Add(option);
                    }
                    break;
                case "TFanFilter":
                    foreach (TraitSO trait in GeneManager.instance.tailFanSOs)
                    {
                        MyDropdown.OptionData option = new MyDropdown.OptionData(trait.traitName);
                        SetOptionFunc(option, trait.ID);
                        filter.options.Add(option);
                    }
                    break;
                case "PatternFilter":
                    foreach (TraitSO trait in GeneManager.instance.patternSOs)
                    {
                        MyDropdown.OptionData option = new MyDropdown.OptionData(trait.traitName);
                        SetOptionFunc(option, trait.ID);
                        filter.options.Add(option);
                    }
                    break;
                case "PColourFilter":
                    foreach (TraitSO trait in GeneManager.instance.colourSOs)
                    {
                        MyDropdown.OptionData option = new MyDropdown.OptionData(trait.traitName);
                        SetOptionFunc(option, trait.ID);
                        filter.options.Add(option);
                    }
                    break;
                case "AgeFilter":
                    MyDropdown.OptionData childOption = new MyDropdown.OptionData("Child");
                    SetOptionFunc(childOption, "C");
                    filter.options.Add(childOption);
                    MyDropdown.OptionData adultOption = new MyDropdown.OptionData("Adult");
                    SetOptionFunc(adultOption, "A");
                    filter.options.Add(adultOption);
                    break;
                default:
                    break;
            }
        }
    }

    private void SetOptionFunc(MyDropdown.OptionData option, string traitID)
    {
        option.func = (set) =>
        {
            if (set)
            {
                traits.Add(traitID);
            }
            else
            {
                traits.Remove(traitID);
            }

            tank.ApplyFilters();
        };
    }

    public List<string> getFilterList()
    {
        if (traits.Count == maxTraitCount || traits.Count == 0)
        {
            Debug.Log("Returning null");
            return null;
        }
        else
        {
            return traits;
        }
    }
}
