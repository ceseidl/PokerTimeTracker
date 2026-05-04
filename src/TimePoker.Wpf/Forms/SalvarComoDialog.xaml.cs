using System.Windows;

namespace TimePoker.Wpf.Forms;

public partial class SalvarComoDialog : Window
{
    public string NomeEscolhido { get; private set; } = string.Empty;

    public SalvarComoDialog(string sugestao = "")
    {
        InitializeComponent();
        TxtNome.Text = sugestao;
        Loaded += (_, _) => { TxtNome.Focus(); TxtNome.SelectAll(); };
    }

    private void AoSalvar(object sender, RoutedEventArgs e)
    {
        var nome = TxtNome.Text.Trim();
        if (string.IsNullOrEmpty(nome))
        {
            System.Windows.MessageBox.Show(this, "Digite um nome.", "Validação",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }
        NomeEscolhido = nome;
        DialogResult = true;
    }

    private void AoCancelar(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
