using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NihongoAddict
{
    public partial class Form1 : Form
    {        
        BuscaDicionario dic = new BuscaDicionario();
        BuscaExemplo exemplo = new BuscaExemplo();
        private int posicao = -1;
        private string _regexEscolhida;
        private List<string> _palavrasPesquisadas;

        private string regexEscolhida
        {
            get { return _regexEscolhida; }
            set { _regexEscolhida = value; dic.regexEscolhida = value; exemplo.regexEscolhida = value; }
        }

        public List<string> palavrasPesquisadas 
        {
            get {return _palavrasPesquisadas;} 
            set {_palavrasPesquisadas = value;} 
        }
        
        Color corTextoDicionario
        {
            get { return richTextBox1.SelectionColor; }
            set { richTextBox1.SelectionColor = value; }
        }

        Color corExemplos
        {
            get { return richTextBox2.SelectionColor; }
            set { richTextBox2.SelectionColor = value; }
        }
        
        public Form1()
        {
            InitializeComponent();            
            palavrasPesquisadas = new List<string>();
        }

        private void pegaRegex()
        {
            if (radioButton1.Checked == true)
                regexEscolhida = "exata";
            else if (radioButton2.Checked == true)
                regexEscolhida = "inicio";
            else if (radioButton3.Checked == true)
                regexEscolhida = "fim";
            else
                regexEscolhida = "meio";
        }

        private void iniciaExemploAssincrono()
        {
            pegaRegex();            
            if (backgroundWorker2.IsBusy)
            {
                backgroundWorker2.CancelAsync();
            }
            else
            {
                richTextBox2.Clear();
                backgroundWorker2.RunWorkerAsync(exemplo);
            }
        }

        private void iniciaDicionarioAssincrono()
        {
            pegaRegex();            
            if (backgroundWorker1.IsBusy)
            {
                backgroundWorker1.CancelAsync();
            }
            else
            {
                richTextBox1.Clear();
                backgroundWorker1.RunWorkerAsync(dic);
            }
        }
                
        private void buscaArquivos(string busca)
        {
            dic.termoBusca = busca.Trim();
            exemplo.termoBusca = busca.Trim();                  
            iniciaExemploAssincrono();
            iniciaDicionarioAssincrono();
        }

        private void novaBusca(string b)
        {
            palavrasPesquisadas.Add(b);
            posicao++;
            buscaArquivos(b);
            textBox1.SelectAll();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            novaBusca(textBox1.Text.Trim());            
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {            
            if (e.KeyCode == Keys.Enter && e.Modifiers == Keys.None)
            {
                novaBusca(textBox1.Text.Trim());                                                                
                e.SuppressKeyPress = true;
            }
        }

        private void richTextBox1_DoubleClick(object sender, EventArgs e)
        {
            buscaKanjiClicado(richTextBox1);
        }
        
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            System.ComponentModel.BackgroundWorker worker;
            worker = (System.ComponentModel.BackgroundWorker)sender;

            BuscaDicionario dic = (BuscaDicionario)e.Argument;
            e.Result = dic.buscaDicionario(worker, e);
        }

        private string formataTraducao(string t)
        {            
            return t.TrimEnd('/').Replace("/", ", ");
        }

        //exibe palavras encontradas
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {                
                BuscaDicionario.estadoAtual estado = (BuscaDicionario.estadoAtual)e.Result;
                Font fonteTrad = new Font("Meiryo", 10, FontStyle.Regular);
                Font fonteInf = new Font("Meiryo", 7, FontStyle.Regular);
                Font fonteJp = new Font("Meiryo", 12, FontStyle.Regular);
                foreach (string res in estado.resultados)
                {
                    foreach (Match match in Regex.Matches(res, dic.regexFormato, RegexOptions.IgnoreCase))
                    {
                        richTextBox1.SelectionFont = fonteJp;
                        this.corTextoDicionario = Color.Blue;
                        richTextBox1.AppendText(match.Groups["kanji"].Value);
                        if (match.Groups["kana"].Length > 0)
                        {
                            this.corTextoDicionario = Color.Black;
                            richTextBox1.AppendText("[");
                            this.corTextoDicionario = Color.Green;
                            richTextBox1.AppendText(match.Groups["kana"].Value);
                            this.corTextoDicionario = Color.Black;
                            richTextBox1.AppendText("]");
                        }
                        this.corTextoDicionario = Color.Black;
                        richTextBox1.SelectionFont = fonteInf;
                        this.corTextoDicionario = Color.Gray;                        
                        richTextBox1.AppendText(Environment.NewLine + match.Groups["informacao"].ToString());
                        richTextBox1.SelectionFont = fonteTrad;
                        this.corTextoDicionario = Color.Black;                        
                        if (match.Groups["traducao"].Value.EndsWith("(P)/"))
                        {
                            richTextBox1.AppendText(formataTraducao(match.Groups["traducao"].Value.Replace("/(P)/", "")));
                            richTextBox1.SelectionFont = fonteInf;
                            this.corTextoDicionario = Color.Red;
                            richTextBox1.AppendText(" pop" + Environment.NewLine);
                        }
                        else
                        {
                            richTextBox1.AppendText(formataTraducao(match.Groups["traducao"].Value) + Environment.NewLine);
                        }
                        
                    }
                }
                label1.Text = "Resultados: " + estado.qtResultados.ToString();
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            System.ComponentModel.BackgroundWorker worker;
            worker = (System.ComponentModel.BackgroundWorker)sender;

            BuscaExemplo exemplo = (BuscaExemplo)e.Argument;
            e.Result = exemplo.buscaExemplos(worker, e);
        }

        //exibe exemplos encontrados
        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                BuscaExemplo.estadoAtual estado = (BuscaExemplo.estadoAtual)e.Result;
                bool mudaCor = false;
                this.corExemplos = Color.Blue;
                label2.Text = "Exemplos: " + estado.qtResultados.ToString();
                foreach (string res in estado.resultados)
                {
                    richTextBox2.AppendText(res + Environment.NewLine);
                    if (mudaCor == true)
                    {
                        corExemplos = Color.Blue;
                        mudaCor = false;
                    }
                    else
                        mudaCor = true;
                }
            }            
        }

        private void contextMenuStrip1_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectedText != null && richTextBox1.SelectedText.Trim() != String.Empty)
            {
                textBox1.Text = richTextBox1.SelectedText.Trim();
                novaBusca(richTextBox1.SelectedText.Trim());                                
            }
            if (richTextBox2.SelectedText != null && richTextBox2.SelectedText.Trim() != String.Empty)
            {
                textBox1.Text = richTextBox2.SelectedText.Trim();
                novaBusca(richTextBox2.SelectedText.Trim());                
            }                        
        }

        //volta a busca
        private void button2_Click(object sender, EventArgs e)
        {            
            if (posicao >= 1)
            {
                textBox1.Text = palavrasPesquisadas[posicao - 1];                
                buscaArquivos(palavrasPesquisadas[posicao - 1]);
                posicao--;
                textBox1.SelectAll();                
            }
        }

        //avança a busca
        private void button3_Click(object sender, EventArgs e)
        {            
            if (posicao < (palavrasPesquisadas.ToArray().Length - 1))
            {
                textBox1.Text = palavrasPesquisadas[posicao + 1];
                textBox1.SelectAll();
                buscaArquivos(palavrasPesquisadas[posicao + 1]);
                posicao++;
            }
        }

        private void buscaKanjiClicado(RichTextBox r)
        {
            if (r.SelectedText.Trim() != String.Empty)
            {
                string textoSelecionado = r.SelectedText.Trim();                
                if (Busca.isKanjiChar(textoSelecionado.ToCharArray()[0]))
                {
                    Form2 kanjiInfo = new Form2(textoSelecionado);
                    kanjiInfo.Show();
                }
                else if (textoSelecionado.Length == 1 &&
                    (Busca.isKatakanaChar(textoSelecionado.ToCharArray()[0]) || Busca.isHiraganaChar(textoSelecionado.ToCharArray()[0])))
                {
                    //não faz nada
                }
                else
                {
                    textBox1.Text = textoSelecionado;
                    palavrasPesquisadas.Add(textoSelecionado);
                    posicao++;
                    buscaArquivos(textoSelecionado);
                }

                textBox1.SelectAll();
            }            
        }

        private void richTextBox2_DoubleClick(object sender, EventArgs e)
        {
            buscaKanjiClicado(richTextBox2);
        }                        
    }
}
