using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundAlarm
{
    public class SoundAlarm : KMonoBehaviour
    {
        protected override void OnSpawn()
        {
            base.OnSpawn();
            Building component = base.GetComponent<Building>();
        }
        protected override void OnCleanUp()
        {
            base.OnCleanUp();
        }
        private void UpdateSounds()
        {
            if (IsPlaying)
            {
                GetComponent<LoopingSounds>().PlayEvent(new GameSoundEvents.Event(sound));
            }
            /*
            if (percentFull == 0f && previousPercentFull != 0f)
            {
                GetComponent<LoopingSounds>().PlayEvent(GameSoundEvents.BatteryDischarged);
            }
            if (percentFull > 0.999f && previousPercentFull <= 0.999f)
            {
                GetComponent<LoopingSounds>().PlayEvent(GameSoundEvents.BatteryFull);
            }
            if (percentFull < 0.25f && previousPercentFull >= 0.25f)
            {
                GetComponent<LoopingSounds>().PlayEvent(GameSoundEvents.BatteryWarning);
            }
            */
        }

        public virtual void EnergySim200ms(float dt)
        {
            UpdateSounds();
        }

        public bool IsPlaying { get; set; }

        private string sound = "";
    }
}
