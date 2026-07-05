namespace HighTechIndustry
{
    public class NeutronicTransmutationChamber : ComplexFabricator
    {
        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            keepAdditionalTag = SimHashes.Hydrogen.CreateTag();
            choreType = Db.Get().ChoreTypes.Fabricate;
            fetchChoreTypeIdHash = Db.Get().ChoreTypes.FabricateFetch.IdHash;

        }
    }
}
