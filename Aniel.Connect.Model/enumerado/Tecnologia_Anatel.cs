using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Tecnologia_Anatel
    {
        [Display(Name = "ADSL1", ResourceType = typeof(Resources.RsCadastros))]
        ADSL1 = 0,
        [Display(Name = "ADSL2", ResourceType = typeof(Resources.RsCadastros))]
        ADSL2 = 1,
        [Display(Name = "AMPS", ResourceType = typeof(Resources.RsCadastros))]
        AMPS = 2,
        [Display(Name = "ATM", ResourceType = typeof(Resources.RsCadastros))]
        ATM = 3,
        [Display(Name = "CABLE_MODEM", ResourceType = typeof(Resources.RsCadastros))]
        Cable_Modem = 4,
        [Display(Name = "CDMA_IS95", ResourceType = typeof(Resources.RsCadastros))]
        CDMA_IS95 = 5,
        [Display(Name = "CDMA2000", ResourceType = typeof(Resources.RsCadastros))]
        CDMA2000 = 6,
        [Display(Name = "DTH", ResourceType = typeof(Resources.RsCadastros))]
        DTH = 7,
        [Display(Name = "EDGE", ResourceType = typeof(Resources.RsCadastros))]
        EDGE = 8,
        [Display(Name = "ETHERNET", ResourceType = typeof(Resources.RsCadastros))]
        ETHERNET = 9,
        [Display(Name = "FR", ResourceType = typeof(Resources.RsCadastros))]
        FR = 10,
        [Display(Name = "FTTB", ResourceType = typeof(Resources.RsCadastros))]
        FTTB = 11,
        [Display(Name = "FWA", ResourceType = typeof(Resources.RsCadastros))]
        FWA = 11,
        [Display(Name = "GPRS", ResourceType = typeof(Resources.RsCadastros))]
        GPRS = 12,
        [Display(Name = "GSW", ResourceType = typeof(Resources.RsCadastros))]
        GSW = 13,
        [Display(Name = "HDSL", ResourceType = typeof(Resources.RsCadastros))]
        HDSL = 14,
        [Display(Name = "HFC", ResourceType = typeof(Resources.RsCadastros))]
        HFC = 15,
        [Display(Name = "HSDPA", ResourceType = typeof(Resources.RsCadastros))]
        HSDPA = 16,
        [Display(Name = "HSPA1", ResourceType = typeof(Resources.RsCadastros))]
        HSPA1 = 17,
        [Display(Name = "HSPAP", ResourceType = typeof(Resources.RsCadastros))]
        HSPAP = 18,
        [Display(Name = "HSUPA", ResourceType = typeof(Resources.RsCadastros))]
        HSUPA = 19,
        [Display(Name = "LTE", ResourceType = typeof(Resources.RsCadastros))]
        LTE = 20,
        [Display(Name = "NR", ResourceType = typeof(Resources.RsCadastros))]
        NR = 22,
        [Display(Name = "OFDMATDD", ResourceType = typeof(Resources.RsCadastros))]
        OFDMATDD = 23,
        [Display(Name = "PLC", ResourceType = typeof(Resources.RsCadastros))]
        PLC = 24,
        [Display(Name = "TDMA", ResourceType = typeof(Resources.RsCadastros))]
        TDMA = 25,
        [Display(Name = "VDSL", ResourceType = typeof(Resources.RsCadastros))]
        VDSL = 26,
        [Display(Name = "VSAT", ResourceType = typeof(Resources.RsCadastros))]
        VSAT = 27,
        [Display(Name = "WDCMA", ResourceType = typeof(Resources.RsCadastros))]
        WDCMA = 28,
        [Display(Name = "WIFI", ResourceType = typeof(Resources.RsCadastros))]
        WIFI = 29,
        [Display(Name = "WIMAX", ResourceType = typeof(Resources.RsCadastros))]
        WIMAX = 30,
        [Display(Name = "MMDS", ResourceType = typeof(Resources.RsCadastros))]
        MMDS = 31,
        [Display(Name = "HSPA", ResourceType = typeof(Resources.RsCadastros))]
        HSPA = 31,

    }
}
