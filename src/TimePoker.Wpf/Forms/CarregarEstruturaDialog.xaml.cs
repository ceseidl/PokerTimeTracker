using System.Windows;
using TimePoker.Services.Persistence;

namespace TimePoker.Wpf.Forms;

public partial class CarregarEstruturaDialog : Window
{
    private readonly EstruturaStore _store;
    public string? NomeEscolhido { get; private set; }

    public CarregarEstruturaDialog(EstruturaStore store)
    {
        InitializeComponent();
        _store = store;
        RecarregarLista();
    }

    private void RecarregarLista()
    {
        Lista.ItemsSource = _store.Listar();
        if (Lista.Items.Count > 0) Lista.SelectedIndex = 0;
    }

    private void AoCarregar(object sender, RoutedEventArgs e)
    {
        if (Lista.SelectedItem is not string nome) return;
        NomeEscolhido = nome;
        DialogResult = true;
    }

    private void AoCancelar(object sender, RoutedEventArgs e) => DialogResult = false;

    private void AoExcluir(object sender, RoutedEventArgs e)
    {
        if (Lista.SelectedItem is not string nome) return;
        var resp = System.Windows.MessageBox.Show(this,
            $"Excluir a estrutura \"{nome}\"?",
            "Confirmar exclusão",
            System.Windows.MessageBoxButton.OKCancel,
            System.Windows.MessageBoxImage.Question);
        if (resp != System.Windows.MessageBoxResult.OK) return;
        _store.Excluir(nome);
        RecarregarLista();
    }
}
