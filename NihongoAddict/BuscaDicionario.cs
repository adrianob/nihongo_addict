using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;


namespace NihongoAddict
{
    /// <summary>
    /// Busca e formata o arquivo de dicionário
    /// </summary>
    class BuscaDicionario : Busca
    {
        /* formato do dicionário
        KANJI [KANA] /(general information) gloss/gloss/.../              
        KANA /(general information) gloss/gloss/.../ 
        */
        public string regexFormato = @"^(?<kanji>[^\s]+\s)(\[(?<kana>[^\]]*)\])?\s?/(?<informacao>\([^\)]+\))(?<traducao>.*)";                        
                
        public BuscaDicionario()
        {
            caminhoArquivo = "dicionario.txt";
            carregaArquivo();
        }

        private void constroiRegex()
        {
            //busca a palavra sozinha
            regexBuscaExata = @"(\)|\/|^|\[)\s?" + termoBusca + @"\s?(\(|\/|\[|\])";
            //busca em início de palavra
            regexBuscaInicio = @"(\)|\/|^|\[)\s?" + termoBusca + @".*";
            //busca no fim da palavra
            regexBuscaFim = @"[^\)\(\[\]\s]*" + termoBusca + @"(\(|\/|\[|\]|\s\[)";
            //busca em qualquer posicao
            regexBuscaMeio = termoBusca;
            mudaRegex();        
        }

        public estadoAtual buscaDicionario(System.ComponentModel.BackgroundWorker worker, System.ComponentModel.DoWorkEventArgs e)
        {                        
            estadoAtual estado = new estadoAtual();                                                         
            estado.resultados = new List<string>();
            constroiRegex();
           
            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {
                foreach(string linhaAtual in arquivo)
                {
                    if (buscaPalavra(linhaAtual.ToLowerInvariant()) == true)//filtro preliminar, mais rápido que uma regex
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(linhaAtual, regexEscolhida, RegexOptions.IgnoreCase))//segundo filtro com uma regex
                        {
                            estado.qtResultados++;
                            if (linhaAtual.EndsWith("(P)/"))//ordena por popularidade                            
                                estado.resultados.Insert(0, linhaAtual);                                
                            else
                                estado.resultados.Add(linhaAtual);
                        }
                    }                                     
                }                
            }            
            return estado;                
        }
    }
}