using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using Pim_04.Views;

namespace Pim_04.Views
{
    public partial class NovoChamadoWindow : Window
    {
        private readonly string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DeskProDb;Integrated Security=True;Encrypt=False";
        private readonly TelaPrincipal parentWindow;
        private readonly int userId;

        public NovoChamadoWindow(TelaPrincipal parent, int userId)
        {
            InitializeComponent();
            this.parentWindow = parent;
            this.userId = userId;
            cbSituacao.SelectedIndex = 0;

            // Debug: Verifique o userId (remova após testar)
            if (userId <= 0)
            {
                MessageBox.Show($"UserId inválido: {userId}. Verifique o login.", "Erro de Debug", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string titulo = txtTitulo.Text?.Trim() ?? "";
                string descricao = txtDescricao.Text?.Trim() ?? "";
                string status = (cbSituacao.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Aberto";

                if (string.IsNullOrWhiteSpace(titulo) || string.IsNullOrWhiteSpace(descricao))
                {
                    MessageBox.Show("Preencha Título e Descrição.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (userId <= 0)
                {
                    MessageBox.Show("UserId inválido. Não é possível salvar.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "INSERT INTO Chamados (UsuarioId, Titulo, Descricao, Status) " +
                        "VALUES (@usuarioId, @titulo, @descricao, @status)", connection);
                    command.Parameters.AddWithValue("@usuarioId", userId);
                    command.Parameters.AddWithValue("@titulo", titulo);
                    command.Parameters.AddWithValue("@descricao", descricao);
                    command.Parameters.AddWithValue("@status", status);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        MessageBox.Show("Nenhum registro inserido. Verifique o schema ou constraints.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                MessageBox.Show("Chamado salvo com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                parentWindow.LoadChamados();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar chamado: {ex.Message}\nStackTrace: {ex.StackTrace}", "Erro Detalhado", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}