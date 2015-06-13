using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NihongoAddict
{
    class BuscaKanji : Busca
    {
        public string regexFormato = @"[^\{]*\{([^\}]+)\}";
        public BuscaKanji()
        {
            caminhoArquivo = "kanjidic.txt";
            carregaArquivo();
        }
        
        public string buscaDicionario(System.ComponentModel.BackgroundWorker worker, System.ComponentModel.DoWorkEventArgs e)
        {            
            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {
                foreach (string linhaAtual in arquivo) 
                {
                    if (linhaAtual[0] == termoBusca[0])
                    {
                        return linhaAtual;
                    }                        
                }
            }
            return string.Empty;
            
        }
    }
}
