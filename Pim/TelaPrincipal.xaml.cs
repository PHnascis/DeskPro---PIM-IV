using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using Pim_04.Views;

namespace Pim_04.Views
{
    public partial class TelaPrincipal : Window
    {
        private readonly int userId;
        private readonly string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DeskProDb;Integrated Security=True;Encrypt=False";
        private ObservableCollection<Chamado> chamados = new ObservableCollection<Chamado>();
        private readonly bool isAdmin;

        public TelaPrincipal(int userId, bool isAdmin)
        {
            InitializeComponent();
            this.userId = userId;
            this.isAdmin = isAdmin;
            dgChamados.ItemsSource = chamados;
            LoadChamados(); // Carrega "Meus Chamados" como padrão
            ToggleAdminFeatures();
        }

        public void LoadChamados()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("SELECT Id, Titulo, Descricao, Status FROM Chamados WHERE UsuarioId = @userId", connection);
                    command.Parameters.AddWithValue("@userId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        chamados.Clear();
                        while (reader.Read())
                        {
                            chamados.Add(new Chamado
                            {
                                Id = reader.GetInt32(0),
                                Titulo = reader.GetString(1),
                                Descricao = reader.GetString(2),
                                Status = reader.GetString(3)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar chamados: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ToggleAdminFeatures()
        {
            if (isAdmin)
            {
                dgChamados.IsReadOnly = false;
                dgChamados.BeginningEdit += DgChamados_BeginningEdit;
                dgChamados.CellEditEnding += DgChamados_CellEditEnding;
                btnTodosChamados.Visibility = Visibility.Visible;
            }
            else
            {
                dgChamados.IsReadOnly = true;
                btnTodosChamados.Visibility = Visibility.Collapsed;
            }
        }

        private void DgChamados_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            // Permitir edição apenas para admin (controlado por IsReadOnly)
        }

        private void DgChamados_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (isAdmin && e.EditAction == DataGridEditAction.Commit)
            {
                var chamado = e.Row.Item as Chamado;
                if (chamado != null)
                {
                    try
                    {
                        using (var connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            var command = new SqlCommand("UPDATE Chamados SET Status = @status WHERE Id = @id", connection);
                            command.Parameters.AddWithValue("@status", chamado.Status);
                            command.Parameters.AddWithValue("@id", chamado.Id);
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao atualizar status: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnNovoChamado_Click(object sender, RoutedEventArgs e)
        {
            NovoChamadoWindow novoChamadoWindow = new NovoChamadoWindow(this, userId);
            novoChamadoWindow.ShowDialog();
            LoadChamados(); // Recarrega "Meus Chamados" após criar um novo
        }

        private void BtnMeusChamados_Click(object sender, RoutedEventArgs e)
        {
            // Recarrega "Meus Chamados" como tela padrão
            LoadChamados();
        }

        private void BtnTodosChamados_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("SELECT Id, Titulo, Descricao, Status FROM Chamados", connection);
                    using (var reader = command.ExecuteReader())
                    {
                        chamados.Clear();
                        while (reader.Read())
                        {
                            chamados.Add(new Chamado
                            {
                                Id = reader.GetInt32(0),
                                Titulo = reader.GetString(1),
                                Descricao = reader.GetString(2),
                                Status = reader.GetString(3)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar todos os chamados: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRelatorios_Click(object sender, RoutedEventArgs e)
        {
            // Sem ação por enquanto
        }

        private void BtnConfiguracoes_Click(object sender, RoutedEventArgs e)
        {
            ConfiguracoesWindow configuracoesWindow = new ConfiguracoesWindow(this, userId, isAdmin);
            configuracoesWindow.ShowDialog();
            LoadChamados(); // Recarrega a tela principal ao voltar
        }

        private void TxtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnBuscar_Click(sender, new RoutedEventArgs());
            }
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string searchTerm = txtBuscar.Text?.Trim() ?? "";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("SELECT Id, Titulo, Descricao, Status FROM Chamados WHERE UsuarioId = @userId AND (Titulo LIKE @term OR Descricao LIKE @term OR Status LIKE @term)", connection);
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@term", $"%{searchTerm}%");
                    using (var reader = command.ExecuteReader())
                    {
                        chamados.Clear();
                        while (reader.Read())
                        {
                            chamados.Add(new Chamado
                            {
                                Id = reader.GetInt32(0),
                                Titulo = reader.GetString(1),
                                Descricao = reader.GetString(2),
                                Status = reader.GetString(3)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao buscar chamados: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSair_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }
    }
}