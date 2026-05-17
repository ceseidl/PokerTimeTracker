# Manual do Usuário — TimePoker

> Versão 1.3.0 · Cronômetro de torneios de poker ao vivo

---

## Sumário

1. [Visão geral](#1-visão-geral)
2. [Primeira execução](#2-primeira-execução)
3. [Janela de Controle](#3-janela-de-controle)
4. [Janela de Display (TV)](#4-janela-de-display-tv)
5. [Estruturas e pré-sets](#5-estruturas-e-pré-sets)
6. [Jogadores, rebuys e prize pool](#6-jogadores-rebuys-e-prize-pool)
7. [Importar do PokerInimigos](#7-importar-do-pokerinimigos)
8. [Integração com o gerenciador (bridge)](#8-integração-com-o-gerenciador-bridge)
9. [Finalizar rodada — enviar tempo total](#9-finalizar-rodada)
10. [Alarme](#10-alarme)
11. [Auto-recovery](#11-auto-recovery)
12. [Atalhos de teclado](#12-atalhos-de-teclado)
13. [Arquivos e pastas](#13-arquivos-e-pastas)
14. [Solução de problemas](#14-solução-de-problemas)

---

## 1. Visão geral

O **TimePoker** roda em **duas janelas simultâneas**:

- **Controle** — onde o anfitrião comanda a noite (botões, ajustes, estrutura)
- **Display** — pra projetar na TV (relógio enorme, blinds, prize pool)

A ideia é arrastar a janela **Display** pra TV (segundo monitor via HDMI) e
deixar a janela de **Controle** no notebook do anfitrião.

## 2. Primeira execução

Ao abrir, aparece um splash com a logo. Em seguida, as duas janelas surgem.

Se você fechou a partida em andamento (X ou crash) com o cronômetro
**Rodando** ou **Pausado**, o app **retoma automaticamente** — sem popup.
Quando estava Rodando, o tempo que o app ficou fechado é descontado do nível
atual (e somado ao DECORRIDO). Veja §11.

## 3. Janela de Controle

```
┌────────────────────────────────────────────────────────────────────┐
│ [logo]  TimePoker — Cronômetro      Nome do torneio: [_]           │
├────────────────────────────────────────────────────────────────────┤
│  TEMPO   NÍVEL   BLINDS      PRIZE POOL  ESTADO   DECORRIDO        │
│  20:00   1/12    50 / 100    R$ 700      PAUSADO  00:25:13         │
├────────────────────────────────────────────────────────────────────┤
│  ◀ Ant │ ▶ Iniciar │ Próx ▶ │ -1min │ +1min │ ⟲ Reset │ 🏁 Final   │
├────────────────────────────────────────────────────────────────────┤
│  JOGADORES: [- 8 +]   📥 Importar PokerInimigos          │
│  REBUYS:    [- 3 +]                                      │
├──────────────────────────────────────────────────────────┤
│  ESTRUTURA: [Turbo] [Padrão] [Deep stack] [Vazia]        │
│             💾 Salvar como...   📂 Carregar              │
│  + Nível │ + Break │ − │ ▲ │ ▼                           │
│  ┌────────────────────────────────────────────────────┐  │
│  │ #  Tipo   SB    BB    Ante  Min   Status           │  │
│  │ 1  Jogo   100   200   0     20    Atual            │  │
│  │ 2  Jogo   200   400   25    20                     │  │
│  │ ...                                                │  │
│  └────────────────────────────────────────────────────┘  │
├──────────────────────────────────────────────────────────┤
│  F11 fullscreen   🔔 Alarme [✓]  Testar    Display ⛶    │
└──────────────────────────────────────────────────────────┘
```

### Botões principais

| Botão | Ação |
|---|---|
| **▶ Iniciar / ⏸ Pausar** | Liga/pausa o tick do cronômetro |
| **◀ Anterior / Próximo ▶** | Pula para o nível anterior/próximo |
| **−1min / +1min** | Ajuste fino do tempo do nível atual |
| **⟲ Reset** | Volta ao nível 1 com tempo cheio (zera o DECORRIDO) |
| **🏁 Finalizar** | Envia o tempo total da partida ao gerenciador (veja §9) |
| **+/− Jogadores** | Ajusta contador de jogadores |
| **+/− Rebuys** | Ajusta contador de rebuys |

### DataGrid de níveis

Clique numa célula para editar **SB**, **BB**, **Ante**, **Min** ou **Tipo**.
Cores:

- 🟢 **Verde** = nível atual
- 🔘 **Cinza** = nível já passado
- 🔴 **Vermelho** = break
- 🟡 **Dourado claro** = linha selecionada (pra mover/remover)

## 4. Janela de Display (TV)

Janela limpa, fonte gigante, alto contraste:

- Tempo em destaque
- Nível atual (X / Total)
- Blinds e ante
- Sidebar com KPIs: jogadores, rebuys, prize pool, tempo até o próximo break

**Controles no canto superior direito** — úteis quando o anfitrião tem
**só uma tela** e quer operar sem alternar para a janela de Controle:
- `⏯` — Iniciar / Pausar
- `🏁 Finalizar` — envia o tempo total da partida ao gerenciador

**F11** — entra/sai fullscreen
**Esc** — sai do fullscreen
**Espaço** — Iniciar / Pausar
**Botão "Display ⛶"** no controle — detecta segundo monitor e joga lá

## 5. Estruturas e pré-sets

Cinco pré-sets prontos:

| Pré-set | Duração | Características |
|---|---|---|
| **Turbo** | 10 min/nível | Curtos, blinds sobem rápido, p/ noites curtas |
| **Padrão** | 15–20 min/nível | Equilibrado (≈3h) |
| **Deep Stack** | 30 min/nível | Estruturas longas, jogo profundo (≈4h+) |
| **Inimigos do Royal Flush** | 20 min/nível | Estrutura oficial da Liga (default ao abrir) — 6 níveis + 2 breaks: 50/100 → 100/200 → 200/400 → **BREAK** → 500/1000 → 1000/2000 → 2000/3000 → **BREAK FINAL** |
| **Vazia** | — | Você monta do zero |

> Novos níveis adicionados na grade (botão **+ Nível**) já vêm com **20 min**
> de duração por padrão. Ajuste depois se quiser.

### Editor

- **+ Nível** / **+ Break** — adiciona ao final
- **−** — remove o selecionado
- **▲ / ▼** — sobe/desce posição
- Edite SB/BB/Ante/Min direto na célula

### Salvar / carregar

**💾 Salvar como...** — pede um nome e salva em
`%APPDATA%\TimePoker\structures\<nome>.json`

**📂 Carregar** — lista estruturas salvas pra escolher

## 6. Jogadores, rebuys e prize pool

- **Jogadores** e **Rebuys** são contadores numéricos (sem nomes na v1.0)
- **Prize pool** é calculado automaticamente:
  `(Jogadores × BuyIn) + (Rebuys × ValorRebuy)`
- Os campos **BuyIn** e **ValorRebuy** ficam configuráveis no controle

> Lista nominal de jogadores e rebuys associados a cada um estão no
> [BACKLOG.md](../BACKLOG.md) para versão futura.

## 7. Importar do PokerInimigos

Se você usa o painel **Inimigos do Royal Flush** na pasta vizinha,
clique em **📥 Importar PokerInimigos**:

1. O app procura o arquivo `participantes.md` subindo pelas pastas
2. Mostra a lista de jogadores com checkboxes (todos marcados por padrão)
3. Você desmarca quem não veio na noite
4. Clica **Importar** — o contador é setado com a quantidade marcada

Se o arquivo não for encontrado, abre um seletor manual.

## 8. Integração com o gerenciador (bridge)

Quando o **Inimigos do Royal Flush** (gerenciador) está instalado na mesma
máquina, os dois apps trocam dados automaticamente via arquivos JSON em
`%APPDATA%\TimePoker\`:

### Gerenciador → Timer  (`bridge.json`)

Ao **Salvar Rascunho** ou **Finalizar Rodada** no gerenciador, ele grava o
estado atual: quantidade de presentes, soma de rebuys, valores do esquema
(buy-in + reserva).

O TimePoker observa o arquivo via `FileSystemWatcher` e atualiza **em tempo
real** — você não precisa recarregar nada:

- **Jogadores** ← número de "Presentes" na rodada
- **Rebuys**   ← soma da coluna Rebuys
- **BuyIn**    ← `Esquema.EntradaTotal` (Padrão = R$50, Campeonato = R$70)
- **ValorRebuy** ← `Esquema.RebuyTotal`

O **prize pool** se recalcula sozinho. Se o gerenciador não está rodando,
o timer continua funcionando normal — integração é best-effort.

### Timer → Gerenciador  (`timer-status.json`)

Veja [§9 Finalizar rodada](#9-finalizar-rodada).

## 9. Finalizar rodada

O botão **🏁 Finalizar** (presente tanto no Painel de Controle quanto no
canto do Display) faz duas coisas:

1. **Pausa** o cronômetro (cortesia visual — você ainda pode continuar).
2. **Grava** `%APPDATA%\TimePoker\timer-status.json` com o tempo total
   decorrido (`HH:MM:SS`), nível atual e estado.

> **Não finaliza a rodada no gerenciador.** Só informa quanto tempo durou.
> No gerenciador, abra o cadastro da rodada e clique **↻ Importar do timer**
> para puxar esse valor — ele entra na planilha (célula E8) e aparece no
> KPI "Duração" do relatório `etapa-NN.html`.

O gerenciador pode estar **fechado** quando você clica 🏁 — o arquivo fica
salvo até a próxima sobrescrita.

O contador **DECORRIDO** no Painel de Controle mostra esse mesmo tempo em
tempo real (HH:MM:SS). Pauses **não contam**. O valor **persiste entre
reaberturas** — se fechar com o cronômetro Rodando, o tempo que o app
ficou fechado também entra no DECORRIDO ao reabrir.

## 10. Alarme

A cada virada de nível, toca **3 beeps**.

- **🔔 Alarme [✓]** — liga/desliga
- **Testar** — toca o som imediatamente

> ⚠️ **Limitação conhecida na v1.0:** o alarme toca pelo **dispositivo
> de saída padrão** do Windows. Se o notebook estiver com a TV via HDMI
> como saída padrão, o som vai pra TV. Workaround: trocar a saída padrão
> em *Configurações › Sistema › Som › Saída*. Seleção de dispositivo
> específico está no backlog.

## 11. Auto-recovery

O app salva um snapshot da sessão:

- A cada **1 minuto** enquanto roda
- Após **cada ação** (mudar nível, ajustar tempo, etc.)
- No fechamento gracioso (X) — snapshot final com timestamp

Localização: `%APPDATA%\TimePoker\session.json`

Ao reabrir, o comportamento é automático — **sem popup**:

| Estado no fechamento | Comportamento na reabertura |
|---|---|
| **Rodando** | Retoma como Pausado, descontando o tempo offline do nível atual e somando ao DECORRIDO |
| **Pausado** | Retoma exatamente onde parou |
| **Aguardando** (não iniciado) | Descarta — começa limpo |
| **Encerrado** | Descarta — começa limpo |

Para zerar manualmente (recomeçar do zero antes da próxima noite), basta usar
o botão **↺ Reset** no painel de Controle ou apagar `session.json`.

## 12. Atalhos de teclado

### Janela Display
| Tecla | Ação |
|---|---|
| **F11** | Fullscreen on/off |
| **Esc** | Sai do fullscreen |

### Janela Controle
| Tecla | Ação |
|---|---|
| **Espaço** | Iniciar / Pausar |
| **←** / **→** | Nível anterior / próximo |

## 13. Arquivos e pastas

| Caminho | O que é |
|---|---|
| `%APPDATA%\TimePoker\session.json` | Snapshot da sessão atual |
| `%APPDATA%\TimePoker\structures\*.json` | Estruturas salvas (inclui "Inimigos do Royal Flush" semeada no primeiro start) |
| `%APPDATA%\TimePoker\bridge.json` | Estado da rodada empurrado pelo gerenciador |
| `%APPDATA%\TimePoker\timer-status.json` | Tempo total da partida — gravado ao clicar 🏁 Finalizar |
| `C:\Program Files\TimePoker\` | Instalação do app |
| `C:\Program Files\TimePoker\docs\MANUAL.html` | Este manual (HTML) |

## 14. Solução de problemas

### "O alarme não toca"
- Verifique o **🔔 Alarme** está marcado
- Clique **Testar** — se não tocar, troque a saída padrão do Windows
  (Configurações › Sistema › Som › Saída)
- Se a TV está conectada via HDMI, ela pode ter virado a saída padrão

### "O app travou no meio do torneio"
- Reabra. Ele oferece **restaurar** a sessão automaticamente

### "Quero começar do zero, sem restaurar"
- Recuse a restauração na caixa de diálogo, ou apague
  `%APPDATA%\TimePoker\session.json`

### "A janela Display não vai pra TV"
- Verifique que a TV está estendida (não duplicada)
- Use o botão **Display ⛶** no controle pra detectar e mover

### "Os blinds saíram errados"
- Verifique a coluna **Tipo** — se for **Break**, BB e SB são ignorados
- Edite as células direto no DataGrid

### "Cliquei Finalizar mas o gerenciador não pegou o tempo"
- Verifique que existe `%APPDATA%\TimePoker\timer-status.json`
- No gerenciador, abra o cadastro da rodada e clique **↻ Importar do timer**
- Se mesmo assim não puxar, confira se o gerenciador é v1.4.0 ou superior

### "Jogadores/Rebuys não atualizam quando salvo no gerenciador"
- A integração precisa do gerenciador v1.3.0+
- Verifique que `%APPDATA%\TimePoker\bridge.json` existe e foi atualizado
  recentemente após o último Salvar Rascunho no gerenciador
- O timer só lê quando o arquivo muda — primeira leitura é na inicialização

---

**Suporte:** abra uma issue em
https://github.com/ceseidl/PokerTimeTracker/issues

© Seidl Software Ltda · Carlos Seidl
