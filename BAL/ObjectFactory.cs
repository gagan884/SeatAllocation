using AppFramework;
using BAL;
using System;
namespace BAL
{
    public class ObjectFactory
    {
        public static IUserInterface GetUIObject(string boardId, int roundno)
        {
            switch (boardId)
            {
                case "CSAB":
                    return new CSABSplAllocation();
                case "CSABSPL22":
                    return new CSABSpecialAllocation2022();
                default:
                    return new ComCouns21UI();
            }
        }

        public static IValidation GetValidationObject(string boardId, int roundno)
        {
            switch (boardId)
            {
                default:
                    return new ComCouns21Validation();
            }
        }


        public static IAllocation GetAllocationObject(string boardId, int roundno)
        {
            switch (boardId)
            {
                case "TestProj":
                    return new TestProjAllocation();
                case "CSAB":
                    return new CSABSplAllocation();
                case "JECA":
                    return new JECA2021Allocation();
                case "JAC":
                    return new JACAllocation();
                case "MCCPG":
                    return new MCCPGAllocation();
                case "MCCUG":
                    return new MCCUGAllocation();
                case "AyushPG":
                    return new AYUSHPGAllocation();
                case "AYUSHUG":
                    return new AYUSHUGAllocation();
                /*Added by Shivam on 05May2022 for Test Purpose Start*/
                case "Shreshta":
                    return new ShreshtaAllocation2022();
                case "PUNJABITI":
                    return new ITIPunjabAllocation2022();
                case "NIMCET_22":
                    return new NIMCETAllocation2022();
                case "CCMT_21":
                    return new TestCCMTAllocation2021();
                case "CCMT_22":
                    return new CCMTAllocation2022();
                case "CCMN_22":
                    return new CCMNAllocation2022();
                case "NEETSS_21":
                    return new TestNEETSSAllocation2021();
                case "NEETMDS21":
                    return new TestNEETMDSAllocation2021();
                case "NCHM2021":
                    return new TestNCHMAllocation2021();
                case "NCHMBSC_22":
                    return new NCHMBSCAllocation2022();
                case "NCHMMSC_22":
                    return new NCHMMSCAllocation2022();
                case "APJEE22":
                    return new APJEEAllocation2022();
                case "JELET22":
                    return new JELETAllocation2022();
                case "JENPASUG":
                    return new JENPASUGAllocation2022();
                case "ANMGNM22":
                    return new ANMGNMAllocation2022();
                case "JEPBN22":
                    return new JEPBNAllocation2022();
                case "WBJEE22":
                    return new WBJEEAllocation2022();
                case "JEECUP22":
                    return new JEECUPAllocation2022();
                case "JEMSCN22":
                    return new JEMSCNAllocation2022();
                case "HBTUMCA22":
                    return new HBTUMCAAllocation2022();
                case "JACCHD_22":
                    return new JACChandigarhAllocation2022();
                case "JECA22":
                    return new JECAAllocation2022();
                case "NEETMDS22":
                    return new NEETMDSAllocation2022();
                case "NEETPG22":
                    return new NEETPGAllocation2022();
                case "UPTACArch":
                    return new UPTACArchAllocation2022();
                case "IIMC22":
                    return new IIMCAllocation2022();
                case "NEETUG22":
                    return new MCCUGAllocation2022();
                case "WBJEEMN22":
                    return new WBJEEMainAllocation2022();
                case "UPSEEUG22":
                    return new UPSEECUETUGAllocation2022();
                case "UPTACPG22":
                    return new UPTACPGAllocation2022();
                case "UPTACBT22":
                    return new UPTACBTECHAllocation2022();
                case "CSABSPL22":
                    return new CSABSpecialAllocation2022();
                case "HBTUBTech22":
                    return new HBTUBTechAllocation2022();

                /*Added by Shivam on 05May2022 for Test Purpose End*/
                default:
                    return new ComCouns21Allocation();
            }
        }


        public static IDownload GetDownloadObject(string boardId, int roundno)
        {
            switch (boardId)
            {
                default:
                    return new ComCouns21Download();
            }
        }

        public static ICommon GetCommonObject()
        {
            return new DefaultCommon();
        }

    }
}
