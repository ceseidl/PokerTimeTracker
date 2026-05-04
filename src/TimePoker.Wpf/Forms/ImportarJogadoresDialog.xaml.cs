using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using TimePoker.Services.Persistence;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace TimePoker.Wpf.Forms;

public partial class ImportarJogadoresDialog : Window
{
    private readonly ParticipantesImporter _importer = new();
    public ObservableCollection<ItemJogador> Itens { get; } = new();

    public int QuantidadeMarcada { get; private set; }
    public List<string> NomesMarcados { get; } = new();

    public ImportarJogadoresDialog()
    {
        InitializeComponent();
        Lista.ItemsSource = Itens;
        Loaded += (_, _) => CarregarParticipantes();
    }

    private void CarregarParticipantes()
    {
        var caminho = _importer.LocalizarArquivo();
        if (caminho == null)
        {
            // Não achou — abre seletor manual
            var dlg = new OpenFileDialog
            {
                Title = "Selecione o arquivo participantes.md do PokerInimigos",
                Filter = "Markdown (*.md)|*.md|Todos|*.*",
                CheckFileExists = true
            };
            if (dlg.ShowDialog(this) != true)
            {
                DialogResult = false;
                return;
            }
            caminho = dlg.FileName;
        }

        try
        {
            var nomes = _importer.Ler(caminho);
            foreach (var nome in nomes) Itens.Add(new ItemJogador(nome) { Marcado = true });
            LblOrigem.Text = $"Origem: {caminho}";
            AtualizarContador();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(this, "Falha ao ler o arquivo:\n" + ex.Message,
                "Erro", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            DialogResult = false;
        }
    }

    private void AoMarcarTodos(object sender, RoutedEventArgs e) { foreach (var i in Itens) i.Marcado = true; AtualizarContador(); }
    private void AoLimpar(object sender, RoutedEventArgs e)      { foreach (var i in Itens) i.Marcado = false; AtualizarContador(); }
    private void AoMudarSelecao(object sender, RoutedEventArgs e) => AtualizarContador();

    private void AtualizarContador()
    {
        var n = Itens.Count(i => i.Marcado);
        LblContador.Text = $"{n} jogador{(n == 1 ? "" : "es")} marcado{(n == 1 ? "" : "s")}";
    }

    private void AoImportar(object sender, RoutedEventArgs e)
    {
        var marcados = Itens.Where(i => i.Marcado).ToList();
        if (marcados.Count == 0)
        {
            System.Windows.MessageBox.Show(this,
                "Marque pelo menos um jogador.",
                "Validação", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }
        QuantidadeMarcada = marcados.Count;
        NomesMarcados.AddRange(marcados.Select(m => m.Nome));
        DialogResult = true;
    }

    private void AoCancelar(object sender, RoutedEventArgs e) => DialogResult = false;
}

public sealed class ItemJogador : INotifyPropertyChanged
{
    public string Nome { get; }
    private bool _marcado;
    public bool Marcado
    {
        get => _marcado;
        set { if (_marcado != value) { _marcado = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Marcado))); } }
    }
    public ItemJogador(string nome) { Nome = nome; }
    public event PropertyChangedEventHandler? PropertyChanged;
}
