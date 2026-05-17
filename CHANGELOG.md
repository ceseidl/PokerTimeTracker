# Changelog

Todas as mudanças notáveis são listadas aqui.

Formato baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/),
versionamento [SemVer](https://semver.org/lang/pt-BR/).

---

## [1.2.0] — 2026-05-16

### Adicionado
- **🏁 Botão Finalizar** no Painel de Controle e na janela Display (canto
  superior direito, discreto). Grava o tempo total decorrido em
  `%APPDATA%\TimePoker\timer-status.json` para o gerenciador consumir.
  **Não fecha a rodada lá** — só informa quanto tempo durou. Pausa o
  cronômetro como cortesia.
- **Cronômetro de tempo total decorrido** (`Decorrido`). Acumula só
  enquanto está rodando — pauses não contam. Exibido como KPI "DECORRIDO"
  no Painel de Controle.
- **Controles discretos no Display** (canto superior direito do header,
  semitransparentes): `⏯` Iniciar/Pausar e `🏁` Finalizar. Útil quando o
  anfitrião tem só uma tela.
- **Pré-set "Inimigos do Royal Flush"** — estrutura oficial da Liga
  (6 níveis 20min + 2 breaks: 50/100 → 100/200 → 200/400 → BREAK →
  500/1000 → 1000/2000 → 2000/3000 → BREAK FINAL). Aplicada como default
  ao abrir e semeada automaticamente em `%APPDATA%\TimePoker\structures\`.
- **Manual.html autocontido** gerado por `installer/build-manual.py`
  (mesmo modelo do gerenciador).

### Alterado
- **Default duration** de novos níveis adicionados via "+ Nível" passou de
  15 para 20 min — mais alinhado com a estrutura padrão da Liga.

---

## [1.1.0] — 2026-05-16

### Adicionado
- **Integração com o gerenciador (Inimigos do Royal Flush).** TimePoker observa
  `%APPDATA%\TimePoker\bridge.json` via `FileSystemWatcher` e atualiza ao vivo
  `Jogadores`, `Rebuys`, `BuyIn` e `ValorRebuy` quando o gerenciador salva uma
  rodada. Prize pool é recalculado automaticamente. Se o gerenciador não estiver
  rodando, o timer continua funcionando normal — atualização é best-effort.

### Corrigido
- **Janelas se ajustam à área útil do monitor ao abrir.** Em notebooks 1366×768
  com taskbar, a janela de Controle ficava cortada. Agora `ControlWindow` e
  `DisplayWindow` redimensionam para 85% da área útil do monitor onde abriram,
  respeitando `MinWidth`/`MinHeight`.

---

## [1.0.1] — 2026-05-12

### Corrigido
- **`installer/build-msi.ps1` preserva estrutura de subpastas.** O harvest
  emitia todo `<Component>` sem `Subdirectory`, então o WiX achatava toda
  a árvore em `INSTALLFOLDER`. Bug descoberto no PokerInimigos (mesmo
  script-base) — corrigido aqui antes de virar problema real para usuários
  que instalarem este MSI.

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
