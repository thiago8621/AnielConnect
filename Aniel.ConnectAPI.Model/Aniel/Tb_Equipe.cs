using Aniel.ConnectAPI.Model.Atributo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.ConnectAPI.Model
{
    [PVTabela("TB_PV_EQUIPE")]
    public class Tb_Equipe
    {
        [PVChave("Equipe")]
        public String Equipe { get; set; }
        public String EquipeAux { get; set; }
        public String Num_Fre { get; set; }
        public String Nome { get; set; }
        public String Cod_Superv { get; set; }
        public Int16? Cod_Perfil_Equipe { get; set; }
        public Int16? Cod_Regiao { get; set; }
        public DateTime? Encerramento { get; set; }
    }
}
