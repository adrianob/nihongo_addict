using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NihongoAddict
{
    /// <summary>
    /// Realiza a busca e guarda o estado do backgroundworker
    /// </summary>
    abstract class Busca
    {        
        private char[] palavra;
        private int pSize;
        private string _termoBusca;
        private string _caminhoArquivo;

        public string regexBuscaExata;
        public string regexBuscaInicio;
        public string regexBuscaFim;
        public string regexBuscaMeio;
        private string _regexEscolhida;        
                
        public List<string> arquivo = new List<string>();

        public Busca() { }

        public Busca(string pl)
        {
            termoBusca = pl;
        }

        public string regexEscolhida
        {
            get { return _regexEscolhida; }
            set { _regexEscolhida = value; }
        }
        
        public string caminhoArquivo
        {
            get { return _caminhoArquivo; }
            set {_caminhoArquivo = value;}
        }

        public void carregaArquivo()
        {
            if (! File.Exists(this.caminhoArquivo))
            {
                throw new ArgumentException("Arquivo não encontrado.");
            }
            arquivo.AddRange(File.ReadAllLines(this.caminhoArquivo));
        }

        public void mudaRegex()
        {
            switch (regexEscolhida)
            {
                case "exata":
                    regexEscolhida = regexBuscaExata;
                    break;
                case "inicio":
                    regexEscolhida = regexBuscaInicio;
                    break;
                case "fim":
                    regexEscolhida = regexBuscaFim;
                    break;
                case "meio":
                    regexEscolhida = regexBuscaMeio;
                    break;
                
                default:
                    regexEscolhida = regexBuscaExata;
                    break;
            }                
        }
        
        public string termoBusca 
        { 
            get { return _termoBusca; }
            set 
            {
                if (value == null || value.Trim() == System.String.Empty)
                {
                    throw new ArgumentException("Escolha um termo de busca.");
                }
                else
                {                    
                    _termoBusca = value.ToLowerInvariant();
                    palavra = termoBusca.ToCharArray();
                    pSize = palavra.Count();                    
                }                
            } 
        }
                        
        public class estadoAtual
        {
            public List<string> resultados;
            public int qtResultados = 0;
        }

        static public bool isKatakanaChar(char c)
        {
            if ((c >= 0x30A0) && (c <= 0x30FF)) return true; // Full and half Katakana
            if ((c >= 0xFF65) && (c <= 0xFF9F)) return true; // Narrow Katakana
            return false;
        }

        static public bool isHiraganaChar(char c)
        {
            if ((c >= 0x3040) && (c <= 0x309F)) return true; // Hiragana
            return false;
        }

        static public bool isKanjiChar(char c)
        {
            if ((c >= 0x3300) && (c <= 0x33FF)) return true; //cjk compatibility
            if ((c >= 0x3400) && (c <= 0x4DBF)) return true; //cjk ext A
            if ((c >= 0x4E00) && (c <= 0x9FAF)) return true; // cjk unified
            return false;
        }

        public bool buscaPalavra(string linha)
        {            
            bool encontrado = false;
            int j = 0;
            foreach (char c in linha)
            {
                if (c == palavra[j])
                {
                    encontrado = true;
                    j++;
                    if (j == pSize)
                    {
                        break;
                    }
                    else
                    {
                        encontrado = false;
                    }
                }
                else
                {
                    encontrado = false;
                    j = 0;
                }                
            }            
            return encontrado;
        }
    }
}
