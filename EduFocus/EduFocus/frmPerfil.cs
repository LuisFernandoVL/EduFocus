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
    public partial class frmPerfil : Form
    {
        private Form frmLogin;
        private Form frmSplash;
        private String txtUsuarioLog;
        private Form frmMenu;

        private int id_matric;
        private string nome, usuario, instituicao, email, senha;

        public frmPerfil(Form frmLogin, Form frmSplash, String txtUsuarioLog, Form frmMenu)
        {
            this.frmLogin = frmLogin;
            this.frmSplash = frmSplash;
            this.txtUsuarioLog = txtUsuarioLog;
            this.frmMenu = frmMenu;
            InitializeComponent();
        }
        private void frmPerfil_Load(object sender, EventArgs e)
        {
            carregaInfos();
            txtUsuarioLog = usuario;
            txtNome.Text = nome;
            txtUsuario.Text = usuario;
            txtInstituicao.Text = instituicao;
            txtEmail.Text = email;
            txtSenha.Text = senha;

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
                frmLogin.Close();
                frmLogin.Dispose();
                frmMenu.Close();
                frmMenu.Dispose();
                this.Close();
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
        private Boolean testaCampos(TextBox txtNome, TextBox txtUsuario, TextBox txtInstituicao, TextBox txtEmail, TextBox txtSenha, TextBox txtConfSenha)
        {
            int test2 = 0, test3 = 0, test4 = 0, test5 = 0, test6 = 0, test7 = 0, test8 = 0, test9 = 0;
            
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
                test8 = 0;
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
            if (test2 == 1 && test3 == 1 && test4 == 1 && test5 == 1 && test6 == 1 && test7 == 1 && test8 == 1 && test9 == 1)
            {
                return true;
            }
            else
                return false;
        }
        private void ativa_desativaEditar(Boolean AeD)
        {
            txtNome.Enabled = AeD;
            txtUsuario.Enabled = AeD;
            txtInstituicao.Enabled = AeD;
            txtEmail.Enabled = AeD;
            txtSenha.Enabled = AeD;
            txtConfSenha.Enabled = AeD;

            btnSalvar.Enabled = AeD;
            btnSalvar.Visible = AeD;
        }

        //****Funções do BD
        // Funções BD **************************************************************************
        private void carregaInfos()
        {
            try
            {
                string sql = "select id_matric,nome,usuario,instituicao,email,senha " +
                             "from aluno where usuario = '" + txtUsuarioLog + "'";
                List<object> param = new List<object>();
                param.Add(txtUsuarioLog);
                NpgsqlDataReader dr = ConexaoBanco.selecionar(sql, param);
                
                if (dr.Read())
                {
                    if (txtUsuarioLog == ((string)dr["usuario"]))
                    {
                        id_matric = (int)dr["id_matric"];
                        nome = (string)dr["nome"];
                        usuario = (string)dr["usuario"];
                        instituicao = (string)dr["instituicao"];
                        email = (string)dr["email"];
                        senha = (string)dr["senha"];
                        
                    }
                    dr.Close();
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problemas com as informações pessoais!\n\nMais detalhes: " + ex.Message, "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void efetuarCadastro(String txtNome, String txtUsuario, String txtInstituicao, String txtEmail, String txtSenha)
        {

            int id_matric = this.id_matric;
            string sql;
            sql = "INSERT INTO aluno (nome, usuario, instituicao, email, senha)" +
               " VALUES(@1,@2,@3,@4,@5,@6)";
            sql = "UPDATE aluno SET nome='" + txtNome + "', usuario='" + txtUsuario + "'," +
                   " instituicao='" + txtInstituicao + "', email='" + txtEmail + "', senha='" + txtSenha + "'" +
                  "WHERE id_matric = " + id_matric + ";";
            List<object> param = new List<object>();
            param.Clear();
            param.Add(txtNome);
            param.Add(txtUsuario);
            param.Add(txtInstituicao);
            param.Add(txtEmail);
            param.Add(txtSenha);
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
        //Botões Editar e Cancelar ------------------------------------------------------------------------
        private void btnEditar_Click(object sender, EventArgs e)
        {
            ativa_desativaEditar(true);
            
        }
        private void btnSalvar_Click(object sender, EventArgs e)
        {
            int paraLooping = 0;
            if (paraLooping == 0)
            {

                bool confCamp;
                try
                {
                    confCamp = testaCampos(txtNome, txtUsuario, txtInstituicao, txtEmail, txtSenha, txtConfSenha);
                    if(txtUsuario.Text!=txtUsuarioLog)
                    {
                        testaUsuario(txtUsuario);
                    }
                    
                    if (confCamp == true)
                    {
                        efetuarCadastro(txtNome.Text,txtUsuario.Text,txtInstituicao.Text,txtEmail.Text,txtSenha.Text);
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
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            ativa_desativaEditar(false);
        }
        //-------------------------------------------------------------------------------------------------
        //Links(Outros forms)------------------------------------------------------------------------------
        private void linkMenu_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmMenu Menu = new frmMenu(this.frmLogin, this.frmSplash, this.txtUsuarioLog);
            Menu.Show();
            this.Close();
        }
    }
}
