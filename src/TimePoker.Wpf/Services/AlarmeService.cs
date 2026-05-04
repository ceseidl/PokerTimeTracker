using System.Diagnostics;
using System.IO;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace TimePoker.Wpf.Services;

/// <summary>
/// Toca alarme curto ao fim de nível usando NAudio (WaveOutEvent), pelo dispositivo
/// padrão do Windows. Renomeia a sessão de áudio para "TimePoker — Cronômetro" no
/// Mixer do Windows logo no startup.
///
/// Limitação conhecida (ver BACKLOG): se o Windows mudar o dispositivo padrão (ex.:
/// TV via HDMI vira playback principal), o som vai pra ele em vez do speaker do
/// notebook. Workaround: trocar o dispositivo padrão do Windows em Configurações ›
/// Sistema › Som. A escolha de dispositivo dentro do app está no backlog.
/// </summary>
public sealed class AlarmeService : IDisposable
{
    private static readonly string Log =
        Path.Combine(Path.GetTempPath(), "TimePoker_alarme.log");

    private readonly MemoryStream _pcm;
    private readonly RawSourceWaveStream _source;
    private readonly WaveOutEvent _waveOut;
    private volatile bool _tocando;
    private bool _sessaoConfigurada;

    public bool Habilitado { get; set; } = true;

    public AlarmeService(int frequenciaHz = 880,
                          int duracaoMs = 250,
                          int repetir = 3,
                          int intervaloMs = 120,
                          double amplitude = 0.85)
    {
        var pcm = GerarPcmRepetido(frequenciaHz, duracaoMs, repetir, intervaloMs, amplitude);
        _pcm = new MemoryStream(pcm);
        _source = new RawSourceWaveStream(_pcm, new WaveFormat(44100, 16, 1));
        _waveOut = new WaveOutEvent
        {
            DesiredLatency = 80,
            Volume = 1.0f
        };
        _waveOut.PlaybackStopped += (_, _) => _tocando = false;
        _waveOut.Init(_source);
        Logar($"NAudio iniciado · {pcm.Length} bytes PCM ({repetir} bipes a {frequenciaHz}Hz, amp={amplitude:F2})");

        // Toca silencioso pra registrar a sessão e renomeá-la antes do primeiro uso.
        Task.Run(InicializarSessaoSilenciosa);
    }

    public Task TocarAsync()
    {
        if (!Habilitado || _tocando)
        {
            Logar(_tocando ? "ignorado (ainda tocando)" : "alarme desabilitado");
            return Task.CompletedTask;
        }
        return Task.Run(() =>
        {
            try
            {
                _tocando = true;
                _pcm.Position = 0;
                _waveOut.Play();
                Logar("Play()");
                ConfigurarSessaoDeAudio();
            }
            catch (Exception ex)
            {
                _tocando = false;
                Logar("erro: " + ex.Message);
            }
        });
    }

    private void InicializarSessaoSilenciosa()
    {
        try
        {
            var volumeOriginal = _waveOut.Volume;
            _waveOut.Volume = 0f;
            _pcm.Position = 0;
            _waveOut.Play();
            Thread.Sleep(120);
            _waveOut.Stop();
            _pcm.Position = 0;
            _waveOut.Volume = volumeOriginal;
            _tocando = false;
            ConfigurarSessaoDeAudio();
        }
        catch (Exception ex)
        {
            Logar("InicializarSessaoSilenciosa falhou: " + ex.Message);
        }
    }

    private void ConfigurarSessaoDeAudio()
    {
        if (_sessaoConfigurada) return;
        try
        {
            var pid = (uint)Process.GetCurrentProcess().Id;
            using var enumerator = new MMDeviceEnumerator();
            var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            var sessions = device.AudioSessionManager.Sessions;
            for (int i = 0; i < sessions.Count; i++)
            {
                var s = sessions[i];
                if (s.GetProcessID == pid)
                {
                    s.DisplayName = "TimePoker — Cronômetro";
                    s.SimpleAudioVolume.Volume = 1.0f;
                    s.SimpleAudioVolume.Mute = false;
                    Logar("sessão configurada · vol=1.0 · mute=false");
                }
            }
            _sessaoConfigurada = true;
        }
        catch (Exception ex)
        {
            Logar("ConfigurarSessaoDeAudio falhou: " + ex.Message);
        }
    }

    public void Dispose()
    {
        _waveOut.Dispose();
        _source.Dispose();
        _pcm.Dispose();
    }

    private static byte[] GerarPcmRepetido(int freqHz, int duracaoMs, int repetir, int intervaloMs, double amplitude)
    {
        const int sampleRate = 44100;
        int samplesPorBipe = sampleRate * duracaoMs / 1000;
        int samplesPorIntervalo = sampleRate * intervaloMs / 1000;
        int totalSamples = repetir * samplesPorBipe + (repetir - 1) * samplesPorIntervalo;
        int fadeSamples = sampleRate / 100;

        using var ms = new MemoryStream(totalSamples * 2);
        using var w = new BinaryWriter(ms);

        double amp = amplitude * short.MaxValue;
        double twoPiF = 2.0 * Math.PI * freqHz;

        for (int rep = 0; rep < repetir; rep++)
        {
            for (int i = 0; i < samplesPorBipe; i++)
            {
                double env = 1.0;
                if (i < fadeSamples) env = (double)i / fadeSamples;
                else if (i > samplesPorBipe - fadeSamples) env = (double)(samplesPorBipe - i) / fadeSamples;
                double sample = Math.Sin(twoPiF * i / sampleRate) * amp * env;
                w.Write((short)sample);
            }
            if (rep < repetir - 1)
                for (int i = 0; i < samplesPorIntervalo; i++) w.Write((short)0);
        }
        return ms.ToArray();
    }

    private static void Logar(string msg)
    {
        try { File.AppendAllText(Log, $"[{DateTime.Now:HH:mm:ss.fff}] {msg}\n"); }
        catch { /* ignora */ }
    }
}
