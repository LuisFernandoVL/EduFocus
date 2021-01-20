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
using Npgsql;

namespace EduFocus
{
    public partial class frmMenu : Form
    {
        private Form frmLogin;
        private Form frmSplash;
        private String txtUsuarioLog;
        
        private int id_matric;
        private string nome, usuario, instituicao, email, senha;

        private int rdbS;
        private Int64 id_tarefa;
        private bool sEditar=false;
        public frmMenu(Form frmLogin, Form frmSplash, String txtUsuarioLog)
        {
            InitializeComponent();
            this.frmLogin = frmLogin;
            this.frmSplash = frmSplash;
            this.txtUsuarioLog = txtUsuarioLog;
        }
        private void frmMenu_Load(object sender, EventArgs e)
        {
            carregaInfos();
            lblUsuario.Text = usuario;
            lblid_Matric.Text = Convert.ToString(id_matric);
            lblInstituicao.Text = instituicao;
            carregarTabs();
        }
        //Funções -------------------------------------------------------------------------------
        private Boolean testaCampos(TextBox txtDisciplina, TextBox txtDescricao)
        {
            int test1 = 0, test2 = 0;
            if (String.IsNullOrEmpty(txtDisciplina.Text))
            {
                MessageBox.Show("Preencha o nome da disciplina!", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtDisciplina.Focus();
                test1 = 0;
            }
            else
            {
                test1 = 1;
            }
            if (String.IsNullOrEmpty(txtDescricao.Text))
            {
                MessageBox.Show("Preencha a descrição da atividade!", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtDescricao.Focus();
                test2 = 0;
            }
            else
            {
                test2 = 1;
            }
            if (test1 == 1 && test2 == 1)
            {
                return true;
            }
            else
                return false;
        }
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
                this.Close();
            }
        }
        private void limpaCampo(TextBox txtCampo)
        {
            txtCampo.Clear();
        }
        private int testRadioButton(RadioButton rdbIniciar, RadioButton rdbContinuar, RadioButton rdbTerminar)
        {
            if (rdbIniciar.Checked == true)
            {
                return 0;
            }
            else if (rdbContinuar.Checked == true)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        private void editar(DataGridView dtgGenerico)
        {
            sEditar = true;
            string rdbI, rdbC, rdbT, data;
            if (dtgGenerico.SelectedRows.Count > 0)
            {
                id_tarefa = (long)dtgGenerico.CurrentRow.Cells["id_tarefa"].Value;
                txtDisciplina.Text = (string)dtgGenerico.CurrentRow.Cells["disciplina"].Value;
                txtDescricao.Text = (string)dtgGenerico.CurrentRow.Cells["descricao"].Value;
                rdbI = (string)dtgGenerico.CurrentRow.Cells["iniciar"].Value;
                rdbC = (string)dtgGenerico.CurrentRow.Cells["continuar"].Value;
                rdbT = (string)dtgGenerico.CurrentRow.Cells["terminar"].Value;
                data = (string)dtgGenerico.CurrentRow.Cells["datadeentrega"].Value;
                dtpDataEntrega.Value = Convert.ToDateTime(data);
                if (rdbI == "■")
                {
                    rdbIniciar.Checked = true;
                }
                if (rdbC == "■")
                {
                    rdbContinuar.Checked = true;
                }
                if (rdbT == "■")
                {
                    rdbTerminar.Checked = true;
                }
            }
            else
            {
                MessageBox.Show("Selecione uma linha");
            }
        }
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
        private void cadastrarAtividade(String txtDisciplina, String txtDescricao, DateTimePicker dtpDataEntrega, int rdbS)
        {
            string rdbI = "", rdbC = "", rdbT = "";
            if (rdbS == 0)
            {
                rdbI = "■";
            }
            if (rdbS == 1)
            {
                rdbC = "■";
            }
            if (rdbS == 2)
            {
                rdbT = "■";
            }
            string dataFinal = dtpDataEntrega.Value.ToString("dd/MM/yyyy");
            DateTime dataAgr = DateTime.Now;

            TimeSpan date = Convert.ToDateTime(dataFinal).Subtract(dataAgr);
            int qtdTempo = date.Days;
            string situacao = "";
            if (qtdTempo >= 4)
            {
                situacao = "Atenção";
            }
            if (qtdTempo == 3 && rdbI == "■")
            {
                situacao = "Cuidado";
            }
            if (qtdTempo == 3 && rdbC == "■")
            {
                situacao = "Atenção";
            }
            if (qtdTempo == 3 && rdbT == "■")
            {
                situacao = "Atenção";
            }
            if (qtdTempo < 3 && rdbI == "■")
            {
                situacao = "SEM TEMPO";
            }
            if (qtdTempo < 3 && rdbC == "■")
            {
                situacao = "Cuidado";
            }
            if (qtdTempo < 3 && rdbT == "■")
            {
                situacao = "Atenção";
            }

            int id_matric = this.id_matric;
            string sql;
            sql = "INSERT INTO tarefa (id_tarefa, disciplina, descricao, iniciar, continuar, terminar, datadeentrega, situacao, id_matric)" +
               " VALUES(nextval('tarefa_id_tarefa_seq'::regclass),'" + txtDisciplina + "','" + txtDescricao + "','" + rdbI + "','" + rdbC + "','" + rdbT + "','" + dataFinal + "','" + situacao + "'," + id_matric + ")";

            List<object> param = new List<object>();
            param.Clear();
            param.Add(txtDisciplina);
            param.Add(txtDescricao);
            param.Add(rdbI);
            param.Add(rdbC);
            param.Add(rdbT);
            param.Add(dataFinal);
            param.Add(situacao);
            param.Add(id_matric);

            ConexaoBanco.executar(sql, param);
            MessageBox.Show("Cadastro realizado com sucesso!", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void atualizarAtividade(String txtDisciplina, String txtDescricao, DateTimePicker dtpDataEntrega, int rdbS)
        {
            string rdbI = "", rdbC = "", rdbT = "";
            if (rdbS == 0)
            {
                rdbI = "■";
            }
            if (rdbS == 1)
            {
                rdbC = "■";
            }
            if (rdbS == 2)
            {
                rdbT = "■";
            }
            string dataFinal = dtpDataEntrega.Value.ToString("dd/MM/yyyy");
            DateTime dataAgr = DateTime.Now;

            TimeSpan date = Convert.ToDateTime(dataFinal).Subtract(dataAgr);
            int qtdTempo = date.Days;
            string situacao = "";
            if (qtdTempo >= 4)
            {
                situacao = "Atenção";
            }
            if (qtdTempo == 3 && rdbI =="■")
            {
                situacao = "Cuidado";
            }
            if (qtdTempo == 3 && rdbC == "■")
            {
                situacao = "Atenção";
            }
            if (qtdTempo == 3 && rdbT == "■")
            {
                situacao = "Atenção";
            }
            if (qtdTempo < 3 && rdbI=="■")
            {
                situacao = "SEM TEMPO";
            }
            if (qtdTempo < 3 && rdbC == "■")
            {
                situacao = "Cuidado";
            }
            if (qtdTempo < 3 && rdbT == "■")
            {
                situacao = "Atenção";
            }

            int id_matric = this.id_matric;
            string sql;
            Int64 id_tarefa = this.id_tarefa;
            sql = "UPDATE tarefa SET disciplina='" + txtDisciplina + "', descricao='" + txtDescricao + "'," +
                    " iniciar='" + rdbI + "', continuar='" + rdbC + "', terminar='" + rdbT + "', datadeentrega='" + dataFinal + "', situacao='" + situacao + "'" +
                   "WHERE id_tarefa = " + id_tarefa + ";";

            List<object> param = new List<object>();
            param.Clear();
            param.Add(txtDisciplina);
            param.Add(txtDescricao);
            param.Add(rdbI);
            param.Add(rdbC);
            param.Add(rdbT);
            param.Add(dataFinal);
            param.Add(situacao);
            param.Add(id_matric);
            param.Add(id_tarefa);

            ConexaoBanco.executar(sql, param);
            MessageBox.Show("Atualização realizada com sucesso!", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void finalizarAtividade()
        {
            string sql;
            Int64 id_tarefa = this.id_tarefa;
            sql = "DELETE FROM tarefa WHERE id_tarefa ="+id_tarefa+"";
            List<object> param = new List<object>();
            param.Clear();
            param.Add(id_tarefa);
            ConexaoBanco.executar(sql,param);
            MessageBox.Show("Atividade finalizada com sucesso!", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void carregarTabs()
        {
            int id_matric = this.id_matric;
            string sql1 = "select id_tarefa, disciplina, descricao, iniciar, continuar,terminar, datadeentrega, situacao from tarefa where id_matric='" + id_matric + "'";
            string sql2 = "select id_tarefa, disciplina, descricao, iniciar, continuar,terminar, datadeentrega, situacao from tarefa where id_matric='" + id_matric + "' order by disciplina";
            string sql3 = "select id_tarefa, disciplina, descricao, iniciar, continuar,terminar, datadeentrega, situacao from tarefa where id_matric='" + id_matric + "' order by situacao DESC";

            List<object> param = new List<object>();
            param.Clear();
            param.Add(id_matric);

            //dtgDisciplina.RowHeadersVisible = false;
            dtgGeral.DataSource = ConexaoBanco.selecionarDataTable(sql1,param);
            dtgGeral.RowHeadersWidth = 30;
            dtgGeral.Columns[0].Visible = true;
            dtgGeral.Columns[0].Width = 0;
            dtgGeral.Columns[1].Width = 140;
            dtgGeral.Columns[2].Width = 140;
            dtgGeral.Columns[3].Width = 130;
            dtgGeral.Columns[4].Width = 130;
            dtgGeral.Columns[5].Width = 130;
            dtgGeral.Columns[6].Width = 100;
            dtgGeral.Columns[7].Width = 100;

            dtgDisciplinas.DataSource = ConexaoBanco.selecionarDataTable(sql2, param);
            dtgDisciplinas.RowHeadersWidth = 30;
            dtgDisciplinas.Columns[0].Visible = true;
            dtgDisciplinas.Columns[0].Width = 0;
            dtgDisciplinas.Columns[1].Width = 140;
            dtgDisciplinas.Columns[2].Width = 140;
            dtgDisciplinas.Columns[3].Width = 130;
            dtgDisciplinas.Columns[4].Width = 130;
            dtgDisciplinas.Columns[5].Width = 130;
            dtgDisciplinas.Columns[6].Width = 100;
            dtgDisciplinas.Columns[7].Width = 100;

            dtgSituacao.DataSource = ConexaoBanco.selecionarDataTable(sql3,param);
            dtgSituacao.RowHeadersWidth = 30;
            dtgSituacao.Columns[0].Visible = true;
            dtgSituacao.Columns[0].Width = 0;
            dtgSituacao.Columns[1].Width = 140;
            dtgSituacao.Columns[2].Width = 140;
            dtgSituacao.Columns[3].Width = 130;
            dtgSituacao.Columns[4].Width = 130;
            dtgSituacao.Columns[5].Width = 130;
            dtgSituacao.Columns[6].Width = 100;
            dtgSituacao.Columns[7].Width = 100;
        }
        //**************************************************************************************
        //Botões Barra Superior ---------------------------------------------------------------- 
        private void btnSair1_MouseClick(object sender, MouseEventArgs e)
        {
            fechaForm();
        }
        private void btnMinimizar_MouseClick(object sender, MouseEventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        //--------------------------------------------------------------------------------------
        //Movimento da Borda -------------------------------------------------------------------  
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
        //Painel de Usuário ---------------------------------------------------------------------
        private void linkPerfil_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmPerfil Perfil = new frmPerfil(this.frmLogin, this.frmSplash, this.usuario, this);
            Perfil.Show();
            this.Hide();
        }
        private void linkLogout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmLogin Login = new frmLogin(this.frmSplash);
            Login.Show();
            this.Close();
        }
        //---------------------------------------------------------------------------------------
        //Painel de Cadastro --------------------------------------------------------------------
        private void btnLimpaDis_Click(object sender, EventArgs e)
        {
            limpaCampo(txtDisciplina);
        }
        private void btnLimpaAti_Click(object sender, EventArgs e)
        {
            limpaCampo(txtDescricao);
        }
        private void btnSalvar_Click(object sender, EventArgs e)
        {
            DialogResult resp = MessageBox.Show("Deseja realmente cadastrar a atividade?", "Cadastrar Atividade",
                                          MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp == DialogResult.Yes)
            {
                int paraLooping=0;
                if (paraLooping == 0)
                {
                    bool confCamp;
                    try
                    {
                        rdbS = testRadioButton(rdbIniciar, rdbContinuar, rdbTerminar);
                        confCamp = testaCampos(txtDisciplina, txtDescricao);
                        if (confCamp == true)
                        {
                            cadastrarAtividade(txtDisciplina.Text, txtDescricao.Text, dtpDataEntrega, rdbS);
                            carregarTabs();
                            limpaCampo(txtDisciplina);
                            limpaCampo(txtDescricao);
                            rdbIniciar.Checked = true;
                            dtpDataEntrega.Value = DateTime.Now;
                            txtDisciplina.Focus();
                            paraLooping = 1;
                        }
                        else
                        {
                            txtDisciplina.Focus();
                            paraLooping = 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Problemas ao realizar o cadastro!\n\nMais detalhes: " + ex.Message, "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                    return;
            }
        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            sEditar = false;
            if(sEditar==false)
            {
                id_tarefa = 0;
            }
            limpaCampo(txtDisciplina);
            limpaCampo(txtDescricao);

           
            btnAtualizar.Enabled = false;
            btnAtualizar.Visible = false;

            btnFinalizarTarefa.Enabled = false;
            btnFinalizarTarefa.Visible = false;

            btnSalvar.Enabled = true;
            btnSalvar.Visible = true;
        }

        private void btnAtualizar_Click(object sender, EventArgs e)
        {
            DialogResult resp = MessageBox.Show("Deseja realmente atualizar a atividade?", "Cadastrar Atividade",
                                         MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp == DialogResult.Yes)
            {
                int paraLooping = 0;
                if (paraLooping == 0)
                {


                    bool confCamp;
                    try
                    {
                        rdbS = testRadioButton(rdbIniciar, rdbContinuar, rdbTerminar);
                        confCamp = testaCampos(txtDisciplina, txtDescricao);
                        if (confCamp == true)
                        {
                            atualizarAtividade(txtDisciplina.Text, txtDescricao.Text, dtpDataEntrega, rdbS);
                            sEditar = false;
                            id_tarefa = 0;
                            carregarTabs();
                            limpaCampo(txtDisciplina);
                            limpaCampo(txtDescricao);
                            dtpDataEntrega.Value = DateTime.Now;
                            rdbIniciar.Checked = true;
                            txtDisciplina.Focus();

                            btnAtualizar.Enabled = false;
                            btnAtualizar.Visible = false;
                            btnFinalizarTarefa.Enabled = false;
                            btnFinalizarTarefa.Visible = false;
                            btnSalvar.Enabled = true;
                            btnSalvar.Visible = true;
                            dtpDataEntrega.Value = DateTime.Now;
                            paraLooping = 1;
                        }
                        else
                        {
                            txtDisciplina.Focus();
                            paraLooping = 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Problemas ao realizar a atualização!\n\nMais detalhes: " + ex.Message, "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                    return;
            }
        }

        

        private void btnFinalizarTarefa_Click(object sender, EventArgs e)
        {
            DialogResult resp = MessageBox.Show("Deseja realmente finalizar a atividade?", "Finalizar Atividade",
                                          MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp == DialogResult.Yes)
            {
                try
                {
                    finalizarAtividade();
                    sEditar = false;
                    id_tarefa = 0;
                    carregarTabs();
                    limpaCampo(txtDisciplina);
                    limpaCampo(txtDescricao);
                    rdbIniciar.Checked = true;
                    txtDisciplina.Focus();
                    btnAtualizar.Enabled = false;
                    btnAtualizar.Visible = false;
                    btnFinalizarTarefa.Enabled = false;
                    btnFinalizarTarefa.Visible = false;
                    btnSalvar.Enabled = true;
                    btnSalvar.Visible = true;
                    dtpDataEntrega.Value = DateTime.Now;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Problemas ao realizar o cadastro!\n\nMais detalhes: " + ex.Message, "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dtgGeral_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }



        //---------------------------------------------------------------------------------------

        //Painel de Atividades ------------------------------------------------------------------

        private void dpsdEditar()
        {
            btnSalvar.Enabled = false;
            btnSalvar.Visible = false;

            btnAtualizar.Enabled = true;
            btnAtualizar.Visible = true;

            btnFinalizarTarefa.Enabled = true;
            btnFinalizarTarefa.Visible = true;
        }
        private void btnEdit1_Click(object sender, EventArgs e)
        {
            editar(dtgGeral);
            dpsdEditar();
        }
        private void btnEdit2_Click(object sender, EventArgs e)
        {
            editar(dtgDisciplinas);
            dpsdEditar();
        }
        private void btnEdit3_Click(object sender, EventArgs e)
        {
            editar(dtgSituacao);
            dpsdEditar();
        }
        //---------------------------------------------------------------------------------------
    }
}
