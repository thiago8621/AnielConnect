using Boleto2Net;
using NReco.PdfGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeracaoBoleto
{
    class Program
    {
        static IBanco banco;
        static void Main(string[] args)
        {
            Console.WriteLine("Testando Geração de Boletos Carteira 09");
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "X",
                Conta = "123456",
                CarteiraPadrao = "09",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };

            banco = Banco.Instancia(Bancos.Bradesco);
            banco.Cedente = geraCedente("1213141", "", "", contaBancaria);
            try
            {
                banco.FormataCedente();
            }
            catch (Exception e) {
                Console.WriteLine(e.InnerException);
            }


            //Gerando Arquivo para Remessa 
            var boletos = new Boletos();
            boletos.Add(gerarBoleto(banco, 1, "A", 1));

            var tipoArquivo = TipoArquivo.CNAB400;
            //Definindo o nome dos arquivos
            var nomeArquivoREM = Path.Combine("C:\\boleto_teste", "Boleto2Net", $"{nameof(GeracaoBoleto)}_{tipoArquivo}.REM");
            var nomeArquivoPDF = Path.Combine("C:\\boleto_teste", "Boleto2Net", $"{nameof(GeracaoBoleto)}_{tipoArquivo}.PDF");
            var nomeArquivoHTML = Path.Combine("C:\\boleto_teste", "Boleto2Net", $"{nameof(GeracaoBoleto)}_{tipoArquivo}.html");

            if (!Directory.Exists(Path.GetDirectoryName(nomeArquivoREM)))
                Directory.CreateDirectory(Path.GetDirectoryName(nomeArquivoREM));
            if (File.Exists(nomeArquivoREM)) {
                File.Delete(nomeArquivoREM);
                if (File.Exists(nomeArquivoREM))
                    Console.WriteLine("Arquivo Remessa não foi excluído: " + nomeArquivoREM);
            }
            if (File.Exists(nomeArquivoPDF))
            {
                File.Delete(nomeArquivoPDF);
                if (File.Exists(nomeArquivoPDF))
                    Console.WriteLine("Arquivo boletos (PDF) não foi excluído: " + nomeArquivoPDF);
            }
            if (File.Exists(nomeArquivoHTML))
            {
                File.Delete(nomeArquivoHTML);
                if (File.Exists(nomeArquivoHTML))
                    Console.WriteLine("Arquivo boletos (HTML) não foi excluído: " + nomeArquivoHTML);
            }

            // Arquivo Remessa
            try
            {
                var arquivoRemessa = new ArquivoRemessa(banco, tipoArquivo, 1);
                using (var fileStream = new FileStream(nomeArquivoREM, FileMode.Create))
                    arquivoRemessa.GerarArquivoRemessa(boletos, fileStream);
                if (!File.Exists(nomeArquivoREM))
                    Console.WriteLine("Arquivo Remessa não encontrado: " + nomeArquivoREM);
            }
            catch (Exception e) {
                if (File.Exists(nomeArquivoREM))
                    File.Delete(nomeArquivoREM);
                Console.WriteLine(e.InnerException.ToString());

            }

            //Gerando Arquivo PDF
            try {
                var html = new StringBuilder();
                foreach (var boletotmp in boletos) {
                    using (var boletoParaImpressao = new BoletoBancario
                    {
                        Boleto = boletotmp,
                        OcultarInstrucoes = false,
                        MostrarComprovanteEntrega = false,
                        MostrarEnderecoCedente = true,
                        ExibirDemonstrativo = true
                    })
                    {
                        html.Append("<div style=\"page-break-after: always;\">");
                        html.Append(boletoParaImpressao.MontaHtml());
                        html.Append("</div>");
                    }

                    var boletoHtml = html.ToString();
                    File.WriteAllText(nomeArquivoHTML, boletoHtml);

                    var pdf = new HtmlToPdfConverter().GeneratePdf(boletoHtml);
                    using (var fs = new FileStream(nomeArquivoPDF, FileMode.Create))
                        fs.Write(pdf, 0, pdf.Length);
                    if (!File.Exists(nomeArquivoPDF))
                        Console.WriteLine("Arquivo Boletos (PDF) não encontrado: " + nomeArquivoPDF);


                }

            }
            catch (Exception e) {
                if (File.Exists(nomeArquivoPDF))
                    File.Delete(nomeArquivoPDF);
                Console.WriteLine(e.InnerException);
            }

            //Lendo Arquivo Retorno 
            // Guia https://github.com/BoletoNet/boleto2net/blob/master/Boleto2.Net/Arquivo/ArquivoRetorno.cs
            /*try{
             * 
             *  var arquivoRetorno = new ArquivoRetorno(<Caminho do arquivo do retorno>);
             * }catch(Exception e){
             *      Console.WriteLine(e.innerException);
             * }
             */




        }

        public static Cedente geraCedente(string codigoCedente, string digitoCodigoCedente, string codigoTransmissao, ContaBancaria conta) {
            return new Cedente {
                CPFCNPJ = "86.875.666/0001-09",
                Nome = "Cedente Teste",
                Codigo = codigoCedente,
                CodigoDV = digitoCodigoCedente,
                Endereco = new Endereco {
                    LogradouroEndereco = "Rua Teste do Cedente",
                    LogradouroNumero = "789",
                    LogradouroComplemento = "Bloco A ",
                    Bairro = "Bairro",
                    Cidade = "Cidade",
                    UF = "RJ",
                    CEP = "23010470"  
                 },
                ContaBancaria = conta
            };
        }

        public static Sacado gerarSacado() {
            return new Sacado {
                CPFCNPJ = "443.316.101-28",
                Nome = "Sacado Teste PF",
                Observacoes = "Matricula 678/9",
                Endereco = new Endereco {
                    LogradouroEndereco = "Rua Teste Sacador",
                    LogradouroNumero = "456",
                    Bairro = "Bairro",
                    Cidade = "Cidade",
                    UF = "RJ",
                    CEP = "56789012",
                }
            };
        }

        public static Boleto gerarBoleto(IBanco banco , int quantidadeBoletos, string aceite, int nossoNumero) {
            var boleto = new Boleto(banco)
            {
                Sacado = gerarSacado(),
                DataEmissao = DateTime.Now.AddDays(-3),
                DataProcessamento = DateTime.Now,
                DataVencimento = DateTime.Now.AddMonths(2),
                ValorTitulo = (decimal)150,
                NossoNumero = nossoNumero.ToString(),
                NumeroDocumento = "BB" + nossoNumero.ToString("D6") + (char)(64 + 1),
                EspecieDocumento = TipoEspecieDocumento.DM,
                Aceite = aceite,
                CodigoInstrucao1 = "11",
                CodigoInstrucao2 = "22",
                DataDesconto = DateTime.Now.AddMonths(1),
                ValorDesconto = (decimal)(100 * 0.10),
                DataMulta = DateTime.Now.AddMonths(3),
                PercentualMulta = (decimal) 2.00,
                ValorMulta = (decimal)(100 * (2.00/100)),
                DataJuros = DateTime.Now.AddMonths(3),
                PercentualJurosDia = (decimal)0.2,
                ValorJurosDia = (decimal)(100*(0.2/100)),
                MensagemArquivoRemessa = "Mensagem para o arquivo remessa",
                NumeroControleParticipante = "CHAVEPRIMARIA =" + nossoNumero
               

            };
            //Mensagem - Instrução para o Caixa

            StringBuilder msgCaixa = new StringBuilder();
            if (boleto.ValorDesconto > 0)
                msgCaixa.AppendLine($"Conceder desconto de {boleto.ValorDesconto.ToString("R$ ##,##0.00")} até {boleto.DataDesconto.ToString("dd/MM/yyyy")}. ");
            if (boleto.ValorMulta > 0)
                msgCaixa.AppendLine($"Cobrar multa de {boleto.ValorMulta.ToString("R$ ##,##0.00")} após o vencimento. ");
            if (boleto.ValorJurosDia > 0)
                msgCaixa.AppendLine($"Cobrar juros de {boleto.ValorJurosDia.ToString("R$ ##,##0.00")} por dia de atraso. ");
            boleto.MensagemInstrucoesCaixa = msgCaixa.ToString();
            //Avalista 
            boleto.Avalista = gerarSacado();
            boleto.Avalista.Nome = boleto.Avalista.Nome.Replace("Sacado", "Avalista");

            boleto.ValidarDados();
            //boleto.Banco = banco;
            return boleto;
        }
           
       
    }
}
