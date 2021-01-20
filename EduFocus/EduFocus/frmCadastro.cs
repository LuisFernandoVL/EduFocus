using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using Npgsql;

namespace EduFocus
{
    public partial class frmCadastro : Form
    {
        private Form frmLogin;
        private Form frmSplash;
        public frmCadastro(Form frmLogin, Form frmSplash)
        {
            InitializeComponent();
            this.frmLogin = frmLogin;
            this.frmSplash = frmSplash;
        }
        private void frmCadastro_Load(object sender, EventArgs e)
        {
            txtRa.Focus();
        }
        //Funções -------------------------------------------------------------------------------
        private void fechaForm()
        {
            DialogResult resp = MessageBox.Show("Deseja realmente sair do sistema?", "Sair",
                                          MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp == DialogResult.Yes)
            {
                frmSplash.Close();
                frmSplash.Dispose();
                this.Close();
            }
        }
        private void limpaForm()
        {
            txtRa.Clear();
            txtNome.Clear();
            txtInstituicao.Clear();
            txtEmail.Clear();
            txtSenha.Clear();
            txtConfSenha.Clear();
        }
        private void testString(TextBox txt)
        {
            string txt1=txt.Text;
            if (txt1.Where(c => char.IsNumber(c)).Count() > 0)
            {
                return;
            }
            else
            {
                MessageBox.Show("Digite apenas números no para o cód.!", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt.Clear();
                txt.Focus();
            }
        }
        private void testaRa(TextBox txt)
        {
            try
            {
                String sql = "SELECT * FROM aluno WHERE id_matric = "+txt.Text;
                List<object> param = new List<object>();
                param.Add(txt.Text);
                NpgsqlDataReader dr = ConexaoBanco.selecionar(sql, param);
                if (dr.Read())
                {
                    MessageBox.Show("Este cód já foi cadastrado!", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txt.Focus();
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problemas na procura pelo cód!\n\nMais detalhes: " + ex.Message, "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void testaUsuario(TextBox txt)
        {
            try
            {
                String sql = "SELECT * FROM aluno WHERE usuario = '" + txt.Text + "'";
                List<object> param = new List<object>();
                param.Add(txt.Text);
                NpgsqlDataReader dr = ConexaoBanco.selecionar(sql, param);
                if (dr.Read())
                {
                    MessageBox.Show("Este usuário já foi cadastrado!", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txt.Focus();
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problemas na procura pelo usuário!\n\nMais detalhes: " + ex.Message, "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private Boolean testaCampos(TextBox txtRa, TextBox txtNome, TextBox txtUsuario, TextBox txtInstituicao, TextBox txtEmail, TextBox txtSenha, TextBox txtConfSenha)
        {
            int test1 = 0, test2 = 0, test3 = 0, test4 = 0, test5 = 0, test6 = 0, test7 = 0, test8 = 0, test9 = 0;
            if (String.IsNullOrEmpty(txtRa.Text))
            {
                MessageBox.Show("Preencha o cód!", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtRa.Focus();
                test1=0;
            }
            else
            {
                test1 = 1;
            }
            if (String.IsNullOrEmpty(txtNome.Text))
            {
                MessageBox.Show("Preencha o nome!", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtNome.Focus();
                test2 = 0;
            }
            else
            {
                test2 = 1;
            }
            if (String.IsNullOrEmpty(txtUsuario.Text))
            {
                MessageBox.Show("Preencha o nome de usuário!", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtUsuario.Focus();
                test3 = 0;
            }
            else
            {
                test3 = 1;
            }
            if (String.IsNullOrEmpty(txtInstituicao.Text))
            {
                MessageBox.Show("Preencha a instituição!", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtInstituicao.Focus();
                test4 = 0;
            }
            else
            {
                test4 = 1;
            }
            if (String.IsNullOrEmpty(txtEmail.Text))
            {
                MessageBox.Show("Preencha o email!", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtEmail.Focus();
                test5 = 0;
            }
            else
            {
                test5 = 1;
            }
            if (String.IsNullOrEmpty(txtSenha.Text))
            {
                MessageBox.Show("Preencha a senha!", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtSenha.Focus();
                test6 = 0;
            }
            else
            {
                test6 = 1;
            }
            if (String.IsNullOrEmpty(txtConfSenha.Text))
            {
                MessageBox.Show("Preencha o campo confirma senha!", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtConfSenha.Focus();
                test7 = 0;
            }
            else
            {
                test7 = 1;
            }
            if (txtSenha.TextLength < 6)
            {
                MessageBox.Show("A senha deve ter no mínimo 6 caracteres!", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtSenha.Focus();
                test8 =0 ;
            }
            else
            {
                test8 = 1;
            }
            string senha = txtSenha.Text;
            if (senha != txtConfSenha.Text)
            {
                MessageBox.Show("As senhas não coincidem!", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtConfSenha.Focus();
                test9 = 0;
            }
            else
            {
                test9 = 1;
            }
            if(test1==1 && test2 == 1 && test3 == 1 && test4 == 1 && test5 == 1 && test6 == 1 && test7 == 1 && test8 == 1 && test9 == 1)
            {
                return true;
            }
            else
                return false;
        }

        //****Funções do BD
        private void efetuarCadastro()
        {
            string senha;
            senha = txtSenha.Text;

            string sql;
            sql = "INSERT INTO aluno (id_matric, nome, usuario, instituicao, email, senha)" +
               " VALUES(@1,@2,@3,@4,@5,@6)";
            List<object> param = new List<object>();
            param.Clear();
            param.Add(Convert.ToInt32(txtRa.Text));
            param.Add(txtNome.Text);
            param.Add(txtUsuario.Text);
            param.Add(txtInstituicao.Text);
            param.Add(txtEmail.Text);
            param.Add(txtSenha.Text);
            ConexaoBanco.executar(sql, param);
            MessageBox.Show("Cadastro realizado com sucesso!", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            frmLogin Login = new frmLogin(this.frmSplash);
            Login.Show();
            this.Close();
        }
        //*****************
        //Botões Barra Superior -----------------------------------------------------------------
        private void btnSair1_MouseClick(object sender, MouseEventArgs e)
        {
            fechaForm();
        }
        private void btnMinimizar_MouseClick(object sender, MouseEventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        //Movimento da borda -------------------------------------------------------------------
        Point arrastarCursor;
        Point arrastarForm;
        bool arrastando;
        private void btnBorder_MouseDown(object sender, MouseEventArgs e)
        {
            arrastando = true;
            arrastarCursor = Cursor.Position;
            arrastarForm = this.Location;
        }
        private void btnBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (arrastando == true)
            {
                Point diferenca = Point.Subtract(Cursor.Position, new Size(arrastarCursor));
                this.Location = Point.Add(arrastarForm, new Size(diferenca));
            }
        }
        private void btnBorder_MouseUp(object sender, MouseEventArgs e)
        {
            arrastando = false;
        }
        //---------------------------------------------------------------------------------------
        //Botões Cadastrar, Limpar e Sair -------------------------------------------------------------------
        private TextBox[] txts = new TextBox[6];
        private void btnCadastrar_Click(object sender, EventArgs e)
        {
            int paraLooping = 0;
            if (paraLooping == 0)
            {

                bool confCamp;
                try
                {
                    confCamp = testaCampos(txtRa, txtNome, txtUsuario, txtInstituicao, txtEmail, txtSenha, txtConfSenha);
                    testString(txtRa);
                    testaRa(txtRa);
                    testaUsuario(txtUsuario);
                    if (confCamp == true)
                    {
                        efetuarCadastro();
                        paraLooping = 1;
                    }
                    else
                    {
                        paraLooping = 1;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Problemas ao realizar o cadastro!\n\nMais detalhes: " + ex.Message, "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
                return;
        }
        private void btnLimpar_Click(object sender, EventArgs e)
        {
            limpaForm();
        }
        private void btnSair_Click(object sender, EventArgs e)
        {
            fechaForm();
        }
        //---------------------------------------------------------------------------------------
        //LinkLabel (Outros Forms) --------------------------------------------------------------------
        private void linkLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmLogin Login = new frmLogin(this.frmSplash);
            Login.Show();
            this.Close();
        }

        //---------------------------------------------------------------------------------------
    }
}
