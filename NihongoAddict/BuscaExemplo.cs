using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NihongoAddict
{
    /// <summary>
    /// Busca e formata o arquivo de exemplos
    /// </summary>
    class BuscaExemplo : Busca
    {
        private string regexB = @"^(?<traducao>[^\#]*)";

        public BuscaExemplo() 
        {
            caminhoArquivo = "exemplos.txt";
            carregaArquivo();
        }

        private void constroiRegex()
        {
            //busca a palavra sozinha
            regexBuscaExata = @"(\s|\.|\,|^)" + termoBusca + @"(\s|\.|\,|$)";
            //busca em início de palavra
            regexBuscaInicio = @"(\s|\.|\,|^)" + termoBusca + @".*";
            //busca no fim da palavra
            regexBuscaFim = @".*" + termoBusca + @"(\s|\.|\,|$)";
            //busca em qualquer posicao
            regexBuscaMeio = termoBusca;
            mudaRegex();
        }

        public estadoAtual buscaExemplos(System.ComponentModel.BackgroundWorker worker, System.ComponentModel.DoWorkEventArgs e)
        {
            estadoAtual estado = new estadoAtual();
            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {
                constroiRegex();
                estado.resultados = new List<string>();
                estado.qtResultados = 0;                
                string linhaTemp = "";                
                bool isASCII = termoBusca[0] <= 127;

                foreach (string linhaAtual in arquivo)
                {                                            
                        if (linhaAtual[0] == 'A')
                        {
                            if (isASCII)
                            {
                                if (buscaPalavra(linhaAtual.ToLowerInvariant()) == true)
                                {
                                    if (System.Text.RegularExpressions.Regex.IsMatch(linhaAtual, regexEscolhida, RegexOptions.IgnoreCase))
                                    {                                        
                                        estado.qtResultados++;
                                        string[] linhas = linhaAtual.Split('\t');
                                        //remove "A: "
                                        estado.resultados.Add(linhas[0].Substring(3));
                                        foreach (Match match in Regex.Matches(linhas[1], regexB, RegexOptions.IgnoreCase))
                                        {
                                            estado.resultados.Add(match.Groups["traducao"].ToString());
                                        }                                        
                                    }
                                }                                
                            }
                            linhaTemp = linhaAtual;                                                                 
                        }
                        else
                        {                            
                            if (linhaAtual[0] == 'B')
                            {
                                if (buscaPalavra(linhaAtual) == true)
                                {
                                    estado.qtResultados++;
                                    string[] linhas = linhaTemp.Split('\t');
                                    //remove "A: "
                                    estado.resultados.Add(linhas[0].Substring(3));
                                    foreach (Match match in Regex.Matches(linhas[1], regexB, RegexOptions.IgnoreCase))
                                    {
                                        estado.resultados.Add(match.Groups["traducao"].ToString());
                                    }                                    
                                }
                            }
                        }                        
                }                                            
            }
            return estado;            
        }
    }
}
