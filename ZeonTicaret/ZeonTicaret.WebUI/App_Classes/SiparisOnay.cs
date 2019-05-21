using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZeonTicaret.WebUI.App_Classes
{
    public enum SiparisOnay
    {
        OnayBekliyor = 1,
        Onaylandi = 2,
        PaketHazırlanıyor=3,
        KargoMerkezeUlaştırılıyor=5,
        KargoUlasti=6,
        Reddedildi = 7
    }
}