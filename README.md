# TimePoker

**Cronômetro e gerenciador de torneios de poker ao vivo** — kiosk-style para
exibição em TV via HDMI, com painel de controle separado para o anfitrião.

[![Release](https://img.shields.io/github/v/release/ceseidl/PokerTimeTracker?label=release)](https://github.com/ceseidl/PokerTimeTracker/releases)
[![Build](https://github.com/ceseidl/PokerTimeTracker/actions/workflows/build.yml/badge.svg)](https://github.com/ceseidl/PokerTimeTracker/actions)
[![License](https://img.shields.io/badge/license-Proprietary-blue.svg)](#licença)

---

## ✨ Recursos (v1.0.0)

- ⏱️ **Cronômetro** com tick de 1s, pausar/retomar, ±1min, reset
- 🪧 **Display dedicado** (segunda janela) para projetar em TV — F11 fullscreen
- 🎛️ **Painel de controle** separado: navegar níveis, ajustar tempo, configurar estrutura
- 📋 **Editor de estrutura ao vivo**: adicionar/remover/reordenar níveis e breaks
- 🎚️ **Pré-sets**: Turbo, Padrão, Deep Stack, Customizada (vazia)
- 💾 **Salvar/carregar estruturas nomeadas** em `%APPDATA%\TimePoker\structures\`
- 👥 **Importar PokerInimigos**: lê `participantes.md` da pasta vizinha
- 💰 **Prize pool calculado**: `(Jogadores × BuyIn) + (Rebuys × ValorRebuy)`
- 🔢 **Contadores** de jogadores e rebuys
- 🔔 **Alarme sonoro** ao virar nível (3 beeps via NAudio)
- 🔁 **Auto-recovery**: salva sessão a cada 1min e em cada ação. Restaura ao reabrir
- 🧠 **Sessão de áudio nomeada** "TimePoker — Cronômetro" no mixer do Windows

## 🖥️ Requisitos

- Windows 10/11 x64
- .NET 10 Desktop Runtime (incluso no MSI)
- 2º monitor / TV via HDMI (opcional — o display também roda em janela)

## 📦 Instalação

Baixe o MSI mais recente em [Releases](https://github.com/ceseidl/PokerTimeTracker/releases)
e execute. Atalhos no menu Iniciar e desktop são criados automaticamente.

## 🚀 Uso rápido

1. Abra **TimePoker** — duas janelas aparecem: **Controle** e **Display**
2. No painel de controle, escolha um pré-set (Turbo / Padrão / Deep Stack)
3. Ajuste BuyIn e ValorRebuy
4. Clique em **Importar PokerInimigos** ou ajuste manualmente o nº de jogadores
5. Arraste a janela **Display** para a TV e pressione **F11** para fullscreen
6. Clique **Iniciar** — o tick começa, alarme toca a cada virada de nível

Manual completo: [docs/MANUAL.md](docs/MANUAL.md)

## 🏗️ Arquitetura

```
TimePoker.Domain     → Cronômetro, Torneio, Estrutura, Nível (puro, testado)
TimePoker.Services   → Persistência (sessão, estruturas, importer)
TimePoker.Wpf        → Janelas Control/Display + ViewModels (MVVM)
```

Pilhas usadas: **WPF + .NET 10**, **CommunityToolkit.Mvvm**, **NAudio**, **xUnit**.

## 🔨 Build local

```bash
git clone https://github.com/ceseidl/PokerTimeTracker.git
cd PokerTimeTracker
dotnet build TimePoker.slnx -c Release
dotnet test  TimePoker.slnx -c Release
dotnet run --project src/TimePoker.Wpf -c Release
```

Para gerar o MSI:

```powershell
cd installer
./build-msi.ps1
# saída: installer/out/TimePoker-1.0.0.msi
```

## 📋 Changelog

Ver [CHANGELOG.md](CHANGELOG.md).

## 🗺️ Backlog

Ver [BACKLOG.md](BACKLOG.md). Itens conhecidos para depois do MVP:
seleção de dispositivo de áudio, lista nominal de jogadores, rebuys
associados a jogador, anúncio falado dos blinds, etc.

## 🤝 Projeto irmão

[**Inimigos do Royal Flush**](https://github.com/ceseidl/GestaoRodadasPoker)
— gestor de temporada/liga (separado, mas pode alimentar este via importação
de `participantes.md`).

## Licença

© Seidl Software Ltda. Todos os direitos reservados.
