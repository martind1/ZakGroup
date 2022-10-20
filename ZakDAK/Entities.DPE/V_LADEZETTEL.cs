﻿using System;
using System.Collections.Generic;

namespace ZakDAK.Entities.DPE
{
    public partial class V_LADEZETTEL
    {
        public int? AANL_NR { get; set; }
        public double? EURBRPRS { get; set; }
        public int? KASS_NR { get; set; }
        public string KUGR_NR { get; set; }
        public int? PRVE_NR { get; set; }
        public string REST_ADR_FAHRER { get; set; }
        public string REST_ADR_FIRMA { get; set; }
        public string REST_AUSWEIS { get; set; }
        public int? STORNO_KASS_NR { get; set; }
        public string SUM_FLAG { get; set; }
        public string ATm { get; set; }
        public string ETm { get; set; }
        public string lityp { get; set; }
        public int vorf_nr { get; set; }
        public string zatyp { get; set; }
        public string sta { get; set; }
        public string mod { get; set; }
        public DateTime? edt { get; set; }
        public DateTime? adt { get; set; }
        public int? ZEITKAT_NR { get; set; }
        public string ZEITKAT_BEZ { get; set; }
        public short? fahr_nr { get; set; }
        public string fahr_knz { get; set; }
        public string fahr_vanr { get; set; }
        public short? anh_nr { get; set; }
        public string anh_knz { get; set; }
        public string anh_vanr { get; set; }
        public string bgl_nr { get; set; }
        public string ents_nr { get; set; }
        public short? ents_lfnr { get; set; }
        public string ents_typ { get; set; }
        public int? anl_nr { get; set; }
        public int? faanl_nr { get; set; }
        public int? erz_nr { get; set; }
        public int? ent_nr { get; set; }
        public string zahler { get; set; }
        public int? zah_nr { get; set; }
        public string bezi_nr { get; set; }
        public string vbez_nr { get; set; }
        public string srte_nr { get; set; }
        public string sogr_nr { get; set; }
        public short? abfa_nr { get; set; }
        public string laga_code { get; set; }
        public string ewc_code { get; set; }
        public string prco_nr { get; set; }
        public int? erlkto { get; set; }
        public string lort_nr { get; set; }
        public string kata_nr { get; set; }
        public string depo_nr { get; set; }
        public string mand_nr { get; set; }
        public string ma_srte_nr { get; set; }
        public string ma_txt { get; set; }
        public short? brwanr { get; set; }
        public string brspnr { get; set; }
        public int? brwpnr { get; set; }
        public double? brgew { get; set; }
        public short? tawanr { get; set; }
        public string taspnr { get; set; }
        public int? tawpnr { get; set; }
        public double? tagew { get; set; }
        public double? men { get; set; }
        public double? vol { get; set; }
        public string me { get; set; }
        public string ge { get; set; }
        public double? kufr_men { get; set; }
        public double? meab_men { get; set; }
        public double? eiprs { get; set; }
        public double? neprs { get; set; }
        public double? brprs { get; set; }
        public double? bar_btr { get; set; }
        public string pe { get; set; }
        public string mw_knz { get; set; }
        public string mw_nr { get; set; }
        public double? mw_prz { get; set; }
        public int? rech_nr { get; set; }
        public short? rech_pos { get; set; }
        public string fkgru { get; set; }
        public string fksor { get; set; }
        public string sam_knz { get; set; }
        public int? sam_nr { get; set; }
        public short? sam_ps { get; set; }
        public string abgsta { get; set; }
        public string arcsta { get; set; }
        public string expsta { get; set; }
        public string fksta { get; set; }
        public string masta { get; set; }
        public string anl_na1 { get; set; }
        public string anl_na2 { get; set; }
        public string anl_str { get; set; }
        public string anl_hnr { get; set; }
        public string anl_lnd { get; set; }
        public string anl_plz { get; set; }
        public string anl_ort { get; set; }
        public string erz_na1 { get; set; }
        public string erz_na2 { get; set; }
        public string erz_str { get; set; }
        public string erz_hnr { get; set; }
        public string erz_lnd { get; set; }
        public string erz_plz { get; set; }
        public string erz_ort { get; set; }
        public string idknz { get; set; }
        public double? prs2 { get; set; }
        public string erfasst_von { get; set; }
        public DateTime? erfasst_am { get; set; }
        public string geaendert_von { get; set; }
        public DateTime? geaendert_am { get; set; }
        public int? anzahl_aenderungen { get; set; }
        public string bemerkung { get; set; }
        public string srte_bez { get; set; }
        public short? KSTE_NR { get; set; }
        public short? KART_NR { get; set; }
        public string WE { get; set; }
        public short? BEZI_KST { get; set; }
        public short? LORT_KST { get; set; }
        public double? KOS_SATZ { get; set; }
        public double? SU_KOS_SATZ { get; set; }
        public double? DMBRPRS { get; set; }
        public double? DMEIPRS { get; set; }
        public short? MEKO_NR { get; set; }
        public string FREMD_NR { get; set; }
        public string SATZART { get; set; }
        public string UEB_NR { get; set; }
        public double? AANL_MEN { get; set; }
        public string ERZD_NA1 { get; set; }
        public string ERZD_NA2 { get; set; }
        public string ERZD_STR { get; set; }
        public string ERZD_HNR { get; set; }
        public string ERZD_LND { get; set; }
        public string ERZD_PLZ { get; set; }
        public string ERZD_ORT { get; set; }
        public string FTXT_KUERZEL { get; set; }
        public string ZAMITTEL { get; set; }
        public string ANL_BEFNR { get; set; }
        public string ERZ_ERZNR { get; set; }
        public string ENT_ENTNR { get; set; }
        public short? ENCH_POS { get; set; }
        public string ER_TRANSPORT { get; set; }
        public string ER_ENTSORGUNG { get; set; }
        public string ER_GEBUEHR { get; set; }
        public string ER_GEBSONST { get; set; }
        public string ER_MIETE { get; set; }
        public string ER_ANKAUF { get; set; }
        public string ER_GUTSCHRIFT { get; set; }
        public string ER_UMLAGE { get; set; }
        public string ER_BEMERKUNG { get; set; }
        public string LABORANALYSE { get; set; }
        public string LABOREMAIL { get; set; }
        public string LZET { get; set; }
        public string ENTS_LITYP { get; set; }
        public double? MEN_KG { get; set; }
        public double? MEN_TO { get; set; }
        public string LAGER_NR { get; set; }
        public double? ENTS_TO { get; set; }
        public string HANDEINGABE { get; set; }
        public string VERT_NR { get; set; }
        public double? VG_BETRAG { get; set; }
        public int? VG_RECH_NR { get; set; }
        public double EFF_NEPRS { get; set; }
        public double? EFF_BRPRS { get; set; }
        public string ANNE_NR { get; set; }
        public string AUVO_ID { get; set; }
        public string DKAT_NR { get; set; }
        public string EANV_ID { get; set; }
        public string EANV_STA { get; set; }
        public string EANV_REGISTER { get; set; }
        public string EANV_SIGNIERT { get; set; }
        public string EWC_GEFAHR { get; set; }
        public string EANV_KNZ { get; set; }
        public short? VFUE_NR { get; set; }
        public int? HAUPT_VORF_NR { get; set; }
        public string CHARGENUMMER { get; set; }
        public string LCHA_NR { get; set; }
        public string HOFL_KTRL { get; set; }
        public string BUNDESLAND { get; set; }
        public string PAUBER { get; set; }
        public string SENT_ERZ { get; set; }
        public string SENT_BEF { get; set; }
        public string SENT_ENT { get; set; }
        public string SENT_BEH { get; set; }
        public string PROJEKTNR { get; set; }
        public string MATKENN { get; set; }
        public string SGD_ABD_EIG { get; set; }
        public string SGD_JBERICHT { get; set; }
        public string SGD_JBERICHT_K { get; set; }
        public string LAGER_BEZ { get; set; }
        public string VERFUELLABSCHNITT { get; set; }
        public string LAGER_BEMERKUNG { get; set; }
        public string EWC_BEZ { get; set; }
        public string ENTS_ENTS_TYP { get; set; }
        public int? ENTS_ZAH_NR { get; set; }
        public string PROBE_NR { get; set; }
        public string HOFL_OK { get; set; }
        public string PROBENAHME_OK { get; set; }
        public string ANL_ADRE { get; set; }
        public string ERZ_ADRE { get; set; }
        public string MKEN_BEZ { get; set; }
        public string ENCH_NAME { get; set; }
        public string KATASTER_NR { get; set; }
        public string KATASTER_BEZ { get; set; }
    }
}
