using CarePackageMod;
using System;
using System.Collections.Generic;

public class CarePackageModifier
{
    public void OverridePackages()
    {
        try
        {
            if (CarePackageState.StateManager.State.version < 6)
            {
                Debug.Log("Need to update CarePackageMod!");
                return;
            }
        }
        catch (Exception)
        {
            Debug.Log("CarePackageMod not installed or worse.");
            return;
        }

        // public CarePackageContainer(string ID, float amount, int? onlyAfterCycle = null, int? onlyUntilCycle = null)
        // Tag, KG, first cycle, last cycle (note that the game starts at cycle 0), null will always return true on that condition
        List<CarePackageContainer> carePackages = new List<CarePackageContainer>();
        carePackages.Add(new CarePackageContainer("ColdBreatherSeed", 2f, 0, 10));
        carePackages.Add(new CarePackageContainer("ColdBreatherSeed", 2f, 10));
        carePackages.Add(new CarePackageContainer("ColdBreatherSeed", 2f));
        CarePackageAPI.OverridePackages(carePackages.ToArray());

        // or

        CarePackageAPI.OverridePackages(new CarePackageContainer[] {
            new CarePackageContainer("ColdBreatherSeed", 100f, 0, 10),
            new CarePackageContainer("Diamond", 300f, 10)
        });

        // other options
        CarePackageState.StateManager.State.biggerRoster = true;
        CarePackageState.StateManager.State.rosterIsOrdered = true;
        CarePackageState.StateManager.State.multiplier = 1f;    // note: this gets ignored on override anyway

    }

    public void RestoreConfig() // restores whatever is in json config
    {
        CarePackageState.StateManager.TryReloadConfiguratorState();
        CarePackageAPI.Reload();
    }
}