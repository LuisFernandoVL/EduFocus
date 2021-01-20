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
    public partial class frmLogin : Form
    {
        private Form frmSplash;
        public frmLogin(Form frmSplash)
        {
            this.frmSplash = frmSplash;
            InitializeComponent();
        }
        private void frmLogin_Load(object sender, EventArgs e)
        {
            txtUsuario.Focus();
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
        private void testaCampos(TextBox txtUsuario, TextBox txtEmail, TextBox txtSenha)
        {
            if (String.IsNullOrEmpty(txtUsuario.Text))
            {
                MessageBox.Show("Preencha o nome de usuário!", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtUsuario.Focus();
                return;
            }
            if (String.IsNullOrEmpty(txtEmail.Text))
            {
                MessageBox.Show("Preencha o e-mail!", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtEmail.Focus();
                return;
            }
            if (String.IsNullOrEmpty(txtSenha.Text))
            {
                MessageBox.Show("Preencha a senha!", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtSenha.Focus();
                return;
            }
        }
        //---------------------------------------------------------------------------------------
        //Botões Barra Superior -----------------------------------------------------------------
        private void btnSair1_MouseClick(object sender, MouseEventArgs e)
        {
            fechaForm();
        }
        private void btnMinimizar_MouseClick(object sender, MouseEventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        //---------------------------------------------------------------------------------------
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
        //Botões Login e Sair -------------------------------------------------------------------
        private TextBox[] txts = new TextBox[1];
        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string senha=txtSenha.Text;
                string email = txtEmail.Text;
                testaCampos(txtUsuario,txtEmail, txtSenha);               
                //senha = HashMD5(txtSenha.Text);
                string sql = "select id_matric,nome,usuario,instituicao,email,senha " +
                             "from aluno where usuario = '"+txtUsuario.Text+"'";
                List<object> param = new List<object>();
                param.Add(txtUsuario.Text);

                NpgsqlDataReader dr = ConexaoBanco.selecionar(sql, param);

                if (dr.Read())
                {
                    if (senha == ((string)dr["senha"]) && email == ((string)dr["email"]))
                    {
                        frmMenu Menu = new frmMenu(this, this.frmSplash, txtUsuario.Text);
                        dr.Close();
                        Menu.Show();
                        this.Hide();
                        
                    }
                    else
                        MessageBox.Show("Usuário, e-mail ou senha incorreto", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Usuário, e-mail ou senha incorreto", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problemas ao realizar o login!\n\nMais detalhes: " + ex.Message, "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        private void btnSair_Click(object sender, EventArgs e)
        {
            fechaForm();
        }

        private void linkCadastro_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmCadastro Cadastro = new frmCadastro(this, this.frmSplash);
            Cadastro.Show();
            this.Hide();
        }

        private void linkADM_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }
        //---------------------------------------------------------------------------------------


    }
}
