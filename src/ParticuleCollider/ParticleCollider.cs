using TUNING;

namespace ParticleCollider
{
    public class ParticleCollider : ComplexFabricator
    {
        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.keepAdditionalTag = SimHashes.Hydrogen.CreateTag();
            this.choreType = Db.Get().ChoreTypes.Cook;
            this.fetchChoreTypeIdHash = Db.Get().ChoreTypes.CookFetch.IdHash;


        }

    }
}
