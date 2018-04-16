using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WinForm_Controle_De_Estoque.Dados;
using WinForm_Controle_De_Estoque.Dados.DataSet_Dados_do_BancoTableAdapters;

namespace WinForm_Controle_De_Estoque.Formularios.Cadastros
{
    public partial class frmCadCategoria : WinForm_Controle_De_Estoque.Formularios.Modelos.frmBase
    {
        public frmCadCategoria()
        {
            InitializeComponent();
        }
        public override void Atualiza_Grid()
        {
            try
            {
                this.Text = lblTitulo.Text = "Categorias de Produtos";
                DataSet_Dados_do_Banco.CategoriaDataTable dt = new DataSet_Dados_do_Banco.CategoriaDataTable();
                CategoriaTableAdapter ta = new CategoriaTableAdapter();
                //dt=ta.GetData();
                DataSet_Dados_do_Banco ds = new DataSet_Dados_do_Banco();
                ta.Fill(ds.Categoria);
                dataSetDadosDoBancoBindingSource.DataSource = ds.Categoria;
                dgvGrid.DataSource = dataSetDadosDoBancoBindingSource;
                bindingNavigator.BindingSource = dataSetDadosDoBancoBindingSource;

                //Configuração das colunas do DataGridView
                if (cmbColuna.Items.Count == 0)
                {
                    foreach (DataColumn coluna in ds.Categoria.Columns)
                    {
                        cmbColuna.Items.Add(coluna.ColumnName);
                    }

                }
                dtGenerico = ds.Categoria;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public override void CarregaValores()
        {
            try
            {
                txtCodigo.Text = dgvGrid.CurrentRow.Cells[0].Value.ToString();
                txtNome.Text = dgvGrid.CurrentRow.Cells["cat_Descricao"].Value.ToString();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public override bool Salvar()
        {
            bool bSalvar = false;
            CategoriaTableAdapter ta = new CategoriaTableAdapter();
            if (sStatus == StatusCadastro.scIncluindo)
            {
                bSalvar = (ta.Insert(txtNome.Text) > 0);
            }
            else if(sStatus == StatusCadastro.scAlterando)
            {
                bSalvar = (ta.Update(txtNome.Text, nCodGenerico) > 0);
            }
            return bSalvar;
        }
        public override bool Excluir()
        {
            bool bExcluir = false;
            CategoriaTableAdapter ta = new CategoriaTableAdapter();
            bExcluir = (ta.Delete(nCodGenerico) > 0);
            return bExcluir;
        }
        public void txtNome_Validating(object sender, CancelEventArgs e)
        {
            if (txtNome.Text == string.Empty)
            {
                errErro.SetError(txtNome, "O campo nome precisa ser preenchido");
                e.Cancel = true;
            }
            else
            {
                errErro.SetError(txtNome, "");
            }
        }

        private void lblNome_Click(object sender, EventArgs e)
        {

        }
    }
}

