# Changelog

Todas as mudanças notáveis são listadas aqui.

Formato baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/),
versionamento [SemVer](https://semver.org/lang/pt-BR/).

---

## [1.0.0] — 2026-05-04

Primeira release pública. MVP completo (11/11 features).

### Adicionado
- ⏱️ Cronômetro com tick de 1s, pausar/retomar, ±1 minuto, reset
- 🪧 Janela **Display** dedicada (kiosk) para TV — F11 fullscreen, Esc sai
- 🎛️ Janela **Controle** com navegação de níveis, ajuste fino de tempo
- 📋 Editor de estrutura editável ao vivo (DataGrid: SB / BB / Ante / Min / Tipo)
- 🎚️ Pré-sets: Turbo, Padrão, Deep Stack, Customizada (vazia)
- 💾 Salvar / carregar estruturas nomeadas em `%APPDATA%\TimePoker\structures\`
- 👥 Importar lista de jogadores do projeto vizinho **PokerInimigos**
  (lê `participantes.md` automaticamente)
- 💰 Cálculo automático de prize pool: `(Jogadores × BuyIn) + (Rebuys × ValorRebuy)`
- 🔢 Contadores de jogadores ativos e rebuys (+/− no controle)
- 🔔 Alarme sonoro de virada de nível (3 beeps, NAudio WaveOutEvent)
- 🔁 Auto-recovery: snapshot a cada 1 min + em cada ação, em
  `%APPDATA%\TimePoker\session.json`. Pergunta ao reabrir se quer restaurar.
  Estado "Rodando" volta como "Pausado" por segurança
- 🧠 Sessão de áudio nomeada **"TimePoker — Cronômetro"** no mixer do Windows
- 🎨 Visual coerente com o projeto irmão **Inimigos do Royal Flush**
  (paleta dourada, splash, ícone, logo)
- ✅ 38 testes unitários no domínio (`TimePoker.Domain.Tests`)

### Conhecido / no backlog
- Alarme toca apenas no dispositivo de **saída padrão** do Windows
  (tentativa de seletor com WASAPI não funcionou — investigação em andamento,
  ver [BACKLOG.md](BACKLOG.md))
- Rebuys e jogadores são **contadores agregados** — sem nomes individuais ainda
- Sem TTS (anúncio falado dos blinds)
