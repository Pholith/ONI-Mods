using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using PeterHan.PLib;
using PeterHan.PLib.Options;

//This mod uses PLib library by Peter Han to create in-game Option menus.
//It is available under the MIT License and you can find it here https://github.com/peterhaneve/ONIMods/tree/main/PLib
//Big thanks to Petherhaneve for sharing this library to the community!

namespace High_Pressure_Applications.Components
{
    [JsonObject(MemberSerialization.OptIn)]
    [RestartRequired]
    public class HPA_ModSettings : SingletonOptions<HPA_ModSettings>
    {
        //===> Pressure Gas Pipe Capacity <==============================================       
        [Option("High-Pressure Gas Pipe Capacity", "", null)]
        [Limit(2, 10)]
        [JsonProperty]
        public int HPGas { get; set; }

        //===> Pressure Liquid Pipe Capacity <===========================================
        [Option("High-Pressure Liquid Pipe Capacity", "", null)]
        [Limit(11, 100)]
        [JsonProperty]
        public int HPLiquid { get; set; }


        //===> Default Settings <==========================================================
        public HPA_ModSettings()
        {
            HPGas = 10;
            HPLiquid = 40;
        }
    }
}
