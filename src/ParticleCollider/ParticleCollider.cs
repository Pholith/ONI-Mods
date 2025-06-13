namespace ParticleCollider
{
    public class ParticleCollider : ComplexFabricator
    {
        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            keepAdditionalTag = SimHashes.Hydrogen.CreateTag();
            choreType = Db.Get().ChoreTypes.Cook;
            fetchChoreTypeIdHash = Db.Get().ChoreTypes.CookFetch.IdHash;


        }

    }
}
