using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinForm_Controle_De_Estoque.Formularios.Modelos
{
    public partial class frmBase : Form
    {
        public frmBase()
        {
            InitializeComponent();
        }


        public int nCodGenerico;
        public DataTable dtGenerico = new DataTable();
        public enum StatusCadastro
        {
            scIncluindo,
            scConsultando,
            scAlterando
        }
        public StatusCadastro sStatus;


        private void LimpaControles()
        {
            foreach (Control ctr in this.grbPesquisa.Controls)
            {
                if (ctr is TextBox)
                    (ctr as TextBox).Text = "";

                if (ctr is ComboBox)
                    (ctr as ComboBox).SelectedIndex = -1;

                if (ctr is ListBox)
                    (ctr as ListBox).SelectedIndex = -1;

                if (ctr is RadioButton)
                    (ctr as RadioButton).Checked = false;

                if (ctr is CheckBox)
                    (ctr as CheckBox).Checked = false;
            }
        }

        private void HabilitaDesabilitaControles(bool bValue)
        {
            // habilitar os botões

            // Novo - será habilitado somente quando for navegação
            btnIncluir.Enabled = (sStatus == StatusCadastro.scConsultando);

            // Salvar
            btnGravar.Enabled = (sStatus == StatusCadastro.scAlterando ||
                sStatus == StatusCadastro.scIncluindo);

            // Excluir
            btnExcluir.Enabled = (sStatus == StatusCadastro.scConsultando);

            // Localizar
            btnPesquisar.Enabled = (sStatus == StatusCadastro.scConsultando);

            // Cancelar
            btnCancelar.Visible = (sStatus == StatusCadastro.scAlterando ||
                sStatus == StatusCadastro.scIncluindo);

            // Fechar
            btnFechar.Enabled = true;
        }

        public virtual void CarregaValores()
        {

        }
        public virtual void Atualiza_Grid()
        {

        }
        public virtual bool Salvar()
        {
            return false;
        }
        public virtual bool Excluir()
        {
            return false;
        }


        private void btnIncluir_Click(object sender, EventArgs e)
        {
            LimpaControles();
            sStatus = StatusCadastro.scIncluindo;
            lblModo.Text = "Incluindo";
            HabilitaDesabilitaControles(true);
            tabControl1.SelectTab(1);
            LimpaControles();
        }

        private void btnGravar_Click(object sender, EventArgs e)
        {
            if (Salvar())
            {
                Atualiza_Grid();
                // Quando for inclusão, selectiona a última linha do DataGridView
                if (sStatus == StatusCadastro.scIncluindo)
                    dataSetDadosDoBancoBindingSource.Position = dataSetDadosDoBancoBindingSource.Count - 1;
                sStatus = StatusCadastro.scConsultando;
                HabilitaDesabilitaControles(false);
                // MessageBox.Show("Registro salvo com sucesso", "Aviso do Sistema", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("O registro não foi salvo, por favor verifique os erros!", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            tabControl1.SelectTab(0);
        }
        
        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Confirma Exclusão?", "Excluindo...", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                if (Excluir())
                {
                    sStatus = StatusCadastro.scConsultando;
                    //MessageBox.Show("Registro excluído com sucesso", "Aviso do sistema", ???)
                    Atualiza_Grid();
                }
                else
                {
                    MessageBox.Show("O registro não foi excluído, por favor verifique os erros!", "Erro!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(0);
            sStatus = StatusCadastro.scConsultando;
            lblModo.Text = "Consultando";
            HabilitaDesabilitaControles(true);
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            if (grbPesquisa.Visible == true)
            {
                this.Height = this.Height - grbPesquisa.Height;
                grbPesquisa.Visible = false;
            }
            else
            {
                this.Height = this.Height + grbPesquisa.Height;
                grbPesquisa.Visible = true;
                cmbColuna.SelectedIndex = -1;
                cmbBuscar.SelectedIndex = -1;
                txtParametro1.Text = "1";
                cmbColuna.Focus();
                txtParametro1.Text = "";
                lblParametro1.Visible = false;
                txtParametro1.Visible = false;
                lblParametro2.Visible = false;
                txtParametro2.Visible = false;
                tabControl1.SelectTab(0);
            }
        }

        private void btnLocalizar_Click(object sender, EventArgs e)
        {
            string vFiltro = "", vOperacao = " = ", vCampo = "";

            if (cmbBuscar.Text == "Todos")
                cmbColuna.SelectedIndex = -1;
            else if (cmbColuna.SelectedIndex == -1 || cmbBuscar.SelectedIndex == -1)
            {
                MessageBox.Show("Informe os parâmetros corretamente");
                return;
            }

            // Definindo a operação
            if (cmbBuscar.Text == "Igual a")
                vOperacao = " = ";
            else if (cmbBuscar.Text == "Maior que")
                vOperacao = " > ";
            else if (cmbBuscar.Text == "Menor que")
                vOperacao = " < ";
            else if (cmbBuscar.Text == "Maior ou igual a")
                vOperacao = " >= ";
            else if (cmbBuscar.Text == "Menor ou igual a")
                vOperacao = " <= ";
            else if (cmbBuscar.Text == "Diferente de")
                vOperacao = " <> ";

            // elimina apóstrofos caso existam
            txtParametro1.Text = txtParametro1.Text.Replace("", "");
            txtParametro2.Text = txtParametro2.Text.Replace("", "");
            if (cmbBuscar.Text == "Todos")
            {
                vFiltro = "";
            }
            else
            {
                vCampo = cmbColuna.Text;
                vFiltro = vCampo;
                if (dtGenerico.Columns[cmbColuna.SelectedIndex].DataType.Name == "String")
                {
                    if (cmbBuscar.Text == "Que começa com")
                        vFiltro = vFiltro + " like " + txtParametro1.Text + "%'";
                    else if (cmbBuscar.Text == "Que contém")
                        vFiltro = vFiltro + " like '%" + txtParametro1.Text + "%'";
                    else
                        vFiltro = vFiltro + vOperacao + "'" + txtParametro1.Text + "'";
                }
                else if (dtGenerico.Columns[cmbColuna.SelectedIndex].DataType.Name == "Int32")
                {
                    if (cmbBuscar.Text == "Que esteja entre")
                        vFiltro = vFiltro + " >= " + txtParametro1.Text + " and " + vCampo + " <= " + txtParametro2.Text;
                    else
                        vFiltro = vFiltro + vOperacao + txtParametro1.Text;
                }
                else if (dtGenerico.Columns[cmbColuna.SelectedIndex].DataType.Name == "DateTime")
                {
                    if (cmbBuscar.Text == "Que esteja entre")
                        vFiltro = vFiltro + " >= " + txtParametro1.Text + " and " + vCampo + " <= " + txtParametro2.Text + "";
                    else
                        vFiltro = vFiltro + vOperacao + "" + txtParametro1.Text + "";
                }
            }
            dataSetDadosDoBancoBindingSource.RemoveFilter();
            dataSetDadosDoBancoBindingSource.Filter = vFiltro;
        }
      
private void frmBase_Load(object sender, EventArgs e)
        {
            lblModo.Text = "Consultando";
            sStatus = StatusCadastro.scConsultando;
            Atualiza_Grid();
        }

        private void frmBase_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                if (sStatus == StatusCadastro.scIncluindo)
                {
                    lblModo.Text = "Incluindo";
                    LimpaControles();
                    tabControl1.SelectNextControl(groupBox1, true, true, true, true);
                }
                else
                {
                    lblModo.Text = "Alterando";
                    sStatus = StatusCadastro.scAlterando;
                }
            }
            else
            {
                sStatus = StatusCadastro.scConsultando;
                lblModo.Text = "Consultando";
            }
            HabilitaDesabilitaControles(true);     
        }

        private void dgvGrid_DoubleClick(object sender, EventArgs e)
        {
            CarregaValores();
            if (dgvGrid.Rows.Count != 0)
                nCodGenerico = (int)dgvGrid.CurrentRow.Cells[0].Value;
            tabControl1.SelectTab(1);
        }

        private void dgvGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvGrid.Rows.Count != 0)
            {
                CarregaValores();
                nCodGenerico = (int)dgvGrid.CurrentRow.Cells[0].Value;
            }
        }

        private void cmbBuscar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBuscar.Text == "Todos")
            {
                lblParametro1.Visible = false;
                txtParametro1.Visible = false;
                lblParametro2.Visible = false;
                txtParametro2.Visible = false;
                btnLocalizar_Click(sender, e);
            }
            else if (cmbBuscar.Text == "Que esteja entre")
            {
                lblParametro1.Visible = true;
                txtParametro1.Visible = true;
                txtParametro1.Text = "";
                txtParametro1.Focus();
                lblParametro2.Visible = true;
                txtParametro2.Visible = true;
                txtParametro2.Text = "";
            }
            else
            {
                lblParametro1.Visible = true;
                txtParametro1.Visible = true;
                txtParametro1.Text = "";
                txtParametro1.Focus();
                lblParametro2.Visible = true;
                txtParametro2.Visible = true;
            }
        }

        private void txtParametro1_Validating(object sender, CancelEventArgs e)
        {
            if (txtParametro1.Text == "")
            {
                errErro.SetError(txtParametro1, "Informe o parâmetro para pesquisa");
                e.Cancel = true;
            }
            else
            {
                errErro.SetError(txtParametro1, "");
            }
        }

        private void txtParametro2_Validating(object sender, CancelEventArgs e)
        {
            if (txtParametro2.Text == "")
            {
                errErro.SetError(txtParametro2, "Informe o parâmetro para pesquisa");
                e.Cancel = true;
            }
            else
            {
                errErro.SetError(txtParametro2, "");
            }
        }

        private void txtParametro1_Leave(object sender, EventArgs e)
        {
            if (txtParametro2.Visible)
                txtParametro2.Focus();
        }
    }

}

