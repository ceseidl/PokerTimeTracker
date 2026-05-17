using System.Text.Json;

namespace TimePoker.Services.Integration;

/// <summary>
/// Snapshot que o TimePoker grava para o gerenciador consumir.
///
/// Caminho: <c>%APPDATA%\TimePoker\timer-status.json</c>. Contrato compartilhado
/// com o gerenciador — mudanças aqui exigem atualizar o leitor do lado de lá.
/// </summary>
public sealed class TimerStatusSnapshot
{
    public int Versao { get; set; } = 1;
    public DateTime Atualizado { get; set; } = DateTime.UtcNow;
    /// <summary>Tempo total que o cronômetro ficou rodando, em segundos (pauses não contam).</summary>
    public long TotalDecorridoSegundos { get; set; }
    public int NivelAtual { get; set; }
    public string Estado { get; set; } = string.Empty;
}

/// <summary>
/// Grava o status atual do timer (principalmente o tempo total decorrido)
/// para o gerenciador consumir. Acionado quando o usuário clica em "Finalizar"
/// no painel do timer — não finaliza a rodada do lado de lá, só informa o tempo.
/// </summary>
public sealed class TimerStatusWriter
{
    private static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = true };

    private readonly string _arquivo;

    public TimerStatusWriter()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _arquivo = Path.Combine(appData, "TimePoker", "timer-status.json");
    }

    public string Arquivo => _arquivo;

    public void Escrever(TimeSpan totalDecorrido, int nivelAtual, string estado)
    {
        var snapshot = new TimerStatusSnapshot
        {
            Atualizado = DateTime.UtcNow,
            TotalDecorridoSegundos = (long)totalDecorrido.TotalSeconds,
            NivelAtual = nivelAtual,
            Estado = estado
        };

        Directory.CreateDirectory(Path.GetDirectoryName(_arquivo)!);
        var json = JsonSerializer.Serialize(snapshot, JsonOpts);

        // Escrita atômica (rename do .tmp) — evita leitura parcial pelo gerenciador.
        var tmp = _arquivo + ".tmp";
        File.WriteAllText(tmp, json);
        if (File.Exists(_arquivo)) File.Delete(_arquivo);
        File.Move(tmp, _arquivo);
    }
}
