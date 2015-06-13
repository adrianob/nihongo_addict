using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace NihongoAddict
{
    public partial class Form2 : Form
    {
        BuscaKanji bk = new BuscaKanji();
        private string _kanji = "";
        public string kanji { get { return _kanji; } set { _kanji = value; } }
        
        public Form2(string k)
        {
            InitializeComponent();
            bk.termoBusca = k;
            iniciaKanjiAssincrono();                                                      
        }

        private void iniciaKanjiAssincrono()
        {
            if (backgroundWorker1.IsBusy)
            {
                backgroundWorker1.CancelAsync();
            }
            else
            {
                backgroundWorker1.RunWorkerAsync(bk);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            System.ComponentModel.BackgroundWorker worker;
            worker = (System.ComponentModel.BackgroundWorker)sender;

            BuscaKanji b = (BuscaKanji)e.Argument;
            e.Result = b.buscaDicionario(worker, e);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                String resultado = (String)e.Result;
                string on = "";
                string kun = "";                
                string significados = "";
                string v = "";
                label1.Text = resultado[0].ToString();
                for (int i = 0; i < resultado.Length; i++)
                {
                    if (Busca.isKatakanaChar(resultado[i]))
                    {
                        on += resultado[i];
                        //verifica se há mais uma leitura on
                        if (resultado[i + 1] == ' ' && Busca.isKatakanaChar(resultado[i + 2]))
                        {
                            on += ", ";
                        }
                        
                    }
                    if (Busca.isHiraganaChar(resultado[i]))
                    {
                        kun += resultado[i];
                        //verifica se há mais uma leitura kun
                        if (resultado[i + 1] == ' ' && 
                            (Busca.isHiraganaChar(resultado[i + 2]) || resultado[i+2] == 'T'))
                        {
                            kun += ", ";
                        }
                    }                      
                }
                foreach (Match match in Regex.Matches(resultado, bk.regexFormato, RegexOptions.IgnoreCase)) 
                {
                    significados += v + match.Groups[1].ToString();
                    v = ", ";
                }
                label5.Text = on;
                label6.Text = kun;
                label7.Text = significados;
            }

        }
    }
}
