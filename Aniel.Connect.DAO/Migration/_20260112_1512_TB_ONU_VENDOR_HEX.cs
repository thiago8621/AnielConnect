using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.Migration
{
    public class _20260112_1512_TB_ONU_VENDOR_HEX : AnielMigration
    {

        public _20260112_1512_TB_ONU_VENDOR_HEX()
        {
            Codigo = "20260112_1512";
            Descricao = "Dados para conversão de serial ONU";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder(@"UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (1, 'ALCL', '414c434c', 'Nokia/Alcatel-Lucent');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (2, 'ALLG', '414c4c47', 'ALLNET');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (3, 'AVMG', '41564d47', 'AVM (FRITZ!Box)');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (4, 'ASKY', '41534b59', 'Askey');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (5, 'CDKT', '43444B54', 'KingType');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (6, 'CIGG', '43494747', 'Cig');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (7, 'CXNK', '43584e4b', 'Calix');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (8, 'DDKT', '44444b54', 'DKT');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (9, 'DLNK', '444c4e4b', 'Dlink');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (10, 'DSNW', '44534e57', 'DASAN');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (11, 'ELTX', '454c5458', 'Eltex');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (12, 'FHTT', '46485454', 'FiberHome');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (13, 'GMTK', '474d544b', 'GemTek');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (14, 'GNXS', '474e5853', 'Genexis');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (15, 'GPNC', '47504E43', 'NuCom');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (16, 'GPON', '47504f4e', 'Generic vendor name');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (17, 'GTHG', '47544847', 'Alcatel-Lucent (ODM)');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (18, 'HALN', '48414c4e', 'HALNy');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (19, 'HBMT', '48424d54', 'HiSense');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (20, 'HUMA', '48554d41', 'Humax');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (21, 'HWTC', '48575443', 'Huawei');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (22, 'ICTR', '49435452', 'Icotera');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (23, 'ISKT', '49534b54', 'Iskratel');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (24, 'KAON', '4b414f4e', 'KAONMEDIA');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (25, 'LEOX', '4c454f58', 'LEOX');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (26, 'LQDE', '4c514445', 'Lantiq');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (27, 'NOKG', '4e4f4b47', 'Nokia (GemTek ODM)');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (28, 'NOKW', '4e4f4b57', 'Nokia (GemTek ODM)');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (29, 'MSTC', '4d535443', 'Mitrastar');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (30, 'PTIN', '5054494e', 'Altice/PT Inovação');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (31, 'RTKG', '52544b47', 'Realtek');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (32, 'SCOM', '53434f4d', 'Sercomm');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (33, 'SKYW', '534b5957', 'Skyworth');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (34, 'SMBS', '534d4253', 'Sagemcom');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (35, 'SPGA', '53504741', 'SourcePhotonics');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (36, 'TMBB', '544d4242', 'Technicolor');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (37, 'TPLG', '54504c47', 'TP-Link');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (38, 'UBNT', '55424e54', 'Ubiquiti');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (39, 'UGRD', '55475244', 'UGrid');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (40, 'YHTC', '59485443', 'Youhua');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (41, 'ZNTS', '5a4e5453', 'DZS');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (42, 'ZRMT', '5a524d54', 'Zaram');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (43, 'ZTEG', '5a544547', 'ZTE');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (44, 'ZYWN', '5a59574e', 'Zyxel');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (45, 'ZYXE', '5a595845', 'Zyxel');
                                      UPDATE OR INSERT INTO TB_ONU_VENDOR_HEX (ID, VENDOR, HEXADECIMAL, DESCRICAO)
                                                        VALUES (46, 'ITBS', '49544253', 'Intelbras');

").ToString();
        }

    }
}
