using System;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.Model
{
    public class DiagramaFusaoConexoesModel
    {
        public long? Id_Elemento { get; set; }
        public String Json_order { get; set; }

        private IEnumerable<PonCadastroModel> lista_PON;
        private IEnumerable<Elemento_RedeCadastroModel> lista_Caixas;
        private IEnumerable<Elemento_Rede_PortaCadastroModel> lista_Portas;
        private IEnumerable<Elemento_Rede_CaboModel> lista_Cabos;
        private IEnumerable<OLTCadastroModel> lista_Olts;
        //private IEnumerable<TuboLoose_DefinicoesModel> lista_TuboLoose;
        public IEnumerable<Elemento_RedeConexoes> listagem_Conexoes;

        public IEnumerable<PonCadastroModel> listagem_PON
        {
            get
            {
                if (lista_PON == null)
                {
                    lista_PON = new List<PonCadastroModel>();
                }
                return lista_PON;
            }
            set
            {
                lista_PON = value;
            }
        }

        public IEnumerable<Elemento_RedeCadastroModel> listagem_Caixas
        {
            get
            {
                if (lista_Caixas == null)
                {
                    lista_Caixas = new List<Elemento_RedeCadastroModel>();
                }
                return lista_Caixas;
            }
            set
            {
                lista_Caixas = value;
            }
        }
        public IEnumerable<Elemento_Rede_PortaCadastroModel> listagem_Portas
        {
            get
            {
                if (lista_Portas == null)
                {
                    lista_Portas = new List<Elemento_Rede_PortaCadastroModel>();
                }
                return lista_Portas;
            }
            set
            {
                lista_Portas = value;
            }
        }

        public IEnumerable<Elemento_Rede_CaboModel> listagem_Cabos
        {
            get
            {
                if (lista_Cabos == null)
                {
                    lista_Cabos = new List<Elemento_Rede_CaboModel>();
                }
                return lista_Cabos;
            }
            set
            {
                lista_Cabos = value;
            }
        }

        public IEnumerable<OLTCadastroModel> listagem_Olts
        {
            get
            {
                if (lista_Olts == null)
                {
                    lista_Olts = new List<OLTCadastroModel>();
                }
                return lista_Olts;
            }
            set
            {
                lista_Olts = value;
            }
        }

        //public IEnumerable<TuboLoose_DefinicoesModel> listagem_Tubo_Loose
        //{
        //    get
        //    {
        //        if (lista_TuboLoose == null)
        //        {
        //            lista_TuboLoose = new List<TuboLoose_DefinicoesModel>();
        //        }
        //        return lista_TuboLoose;
        //    }
        //    set
        //    {
        //        lista_TuboLoose = value;
        //    }
        //}

    }
    public class Elemento_Rede_CaboModel
    {
        public long Id { get; set; }
        private IEnumerable<TuboLoose_DefinicoesModel> lista_TuboLoose;
        public IEnumerable<TuboLoose_DefinicoesModel> listagem_Tubo_Loose
        {
            get
            {
                if (lista_TuboLoose == null)
                {
                    lista_TuboLoose = new List<TuboLoose_DefinicoesModel>();
                }
                return lista_TuboLoose;
            }
            set
            {
                lista_TuboLoose = value;
            }
        }
    }
    public class DiagramaFusaoRequestModel
    {
        public long Id { get; set; }
        public string Origem { get; set; }

    }

    public interface ICaboDefinicoesModel
    {
        long Id { get; set; }
        string Cor { get; set; }
    }
    public class Fibra_DefinicoesModel : ICaboDefinicoesModel
    {
        public long Id { get; set; }
        public string Cor { get; set; }
        public int NumeroFibra { get; set; }
        public int NumeroLoose { get; set; }

    }
    public class TuboLoose_DefinicoesModel : ICaboDefinicoesModel
    {
        public long Id { get; set; }
        public string Cor { get; set; }
        public IEnumerable<Fibra_DefinicoesModel> Fibras
        {
            get
            {
                if (ListagemFibras == null)
                {
                    ListagemFibras = new List<Fibra_DefinicoesModel>();
                }
                return ListagemFibras;
            }
            set { ListagemFibras = value; }
        }
        private IEnumerable<Fibra_DefinicoesModel> ListagemFibras;
    }

}
