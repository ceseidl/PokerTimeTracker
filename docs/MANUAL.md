# Manual do Usuário — TimePoker

> Versão 1.0.0 · Cronômetro de torneios de poker ao vivo

---

## Sumário

1. [Visão geral](#1-visão-geral)
2. [Primeira execução](#2-primeira-execução)
3. [Janela de Controle](#3-janela-de-controle)
4. [Janela de Display (TV)](#4-janela-de-display-tv)
5. [Estruturas e pré-sets](#5-estruturas-e-pré-sets)
6. [Jogadores, rebuys e prize pool](#6-jogadores-rebuys-e-prize-pool)
7. [Importar do PokerInimigos](#7-importar-do-pokerinimigos)
8. [Alarme](#8-alarme)
9. [Auto-recovery](#9-auto-recovery)
10. [Atalhos de teclado](#10-atalhos-de-teclado)
11. [Arquivos e pastas](#11-arquivos-e-pastas)
12. [Solução de problemas](#12-solução-de-problemas)

---

## 1. Visão geral

O **TimePoker** roda em **duas janelas simultâneas**:

- **Controle** — onde o anfitrião comanda a noite (botões, ajustes, estrutura)
- **Display** — pra projetar na TV (relógio enorme, blinds, prize pool)

A ideia é arrastar a janela **Display** pra TV (segundo monitor via HDMI) e
deixar a janela de **Controle** no notebook do anfitrião.

## 2. Primeira execução

Ao abrir, aparece um splash com a logo. Em seguida, as duas janelas surgem.
Se o app detectar uma **sessão anterior**, ele pergunta se quer **restaurar**
(útil quando trava ou você fecha sem querer no meio do torneio).

## 3. Janela de Controle

```
┌──────────────────────────────────────────────────────────┐
│ [logo]  TimePoker — Cronômetro      Nome do torneio: [_] │
├──────────────────────────────────────────────────────────┤
│  TEMPO     NÍVEL      BLINDS         PRIZE POOL  ESTADO  │
│  20:00     1 / 12     100 / 200      R$ 1.200    PAUSADO │
├──────────────────────────────────────────────────────────┤
│  ◀ Anterior │ ▶ Iniciar │ Próximo ▶ │ -1min │ +1min │ ⟲  │
├──────────────────────────────────────────────────────────┤
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
| **⟲ Reset** | Volta ao nível 1 com tempo cheio |
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
- Botões discretos: Anterior / Próximo

**F11** — entra/sai fullscreen
**Esc** — sai do fullscreen
**Botão "Display ⛶"** no controle — detecta segundo monitor e joga lá

## 5. Estruturas e pré-sets

Quatro pré-sets prontos:

| Pré-set | Duração | Características |
|---|---|---|
| **Turbo** | 10 min/nível | Curtos, blinds sobem rápido, p/ noites curtas |
| **Padrão** | 20 min/nível | Equilibrado |
| **Deep Stack** | 30 min/nível | Estruturas longas, jogo profundo |
| **Vazia** | — | Você monta do zero |

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

## 8. Alarme

A cada virada de nível, toca **3 beeps**.

- **🔔 Alarme [✓]** — liga/desliga
- **Testar** — toca o som imediatamente

> ⚠️ **Limitação conhecida na v1.0:** o alarme toca pelo **dispositivo
> de saída padrão** do Windows. Se o notebook estiver com a TV via HDMI
> como saída padrão, o som vai pra TV. Workaround: trocar a saída padrão
> em *Configurações › Sistema › Som › Saída*. Seleção de dispositivo
> específico está no backlog.

## 9. Auto-recovery

O app salva um snapshot da sessão:

- A cada **1 minuto**
- Após **cada ação** (mudar nível, ajustar tempo, etc.)

Localização: `%APPDATA%\TimePoker\session.json`

Ao reabrir o app, se houver snapshot válido, ele pergunta:
*"Encontramos uma sessão anterior. Restaurar?"*

> Por segurança, o estado **Rodando** sempre volta como **Pausado** —
> você confirma manualmente que quer retomar.

## 10. Atalhos de teclado

### Janela Display
| Tecla | Ação |
|---|---|
| **F11** | Fullscreen on/off |
| **Esc** | Sai do fullscreen |

### Janela Controle
Os botões da interface são a interface oficial — sem atalhos globais
para evitar disparos acidentais durante o jogo.

## 11. Arquivos e pastas

| Caminho | O que é |
|---|---|
| `%APPDATA%\TimePoker\session.json` | Snapshot da sessão atual |
| `%APPDATA%\TimePoker\structures\*.json` | Estruturas salvas |
| `C:\Program Files\TimePoker\` | Instalação do app |
| `C:\Program Files\TimePoker\docs\MANUAL.html` | Este manual (HTML) |

## 12. Solução de problemas

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

---

**Suporte:** abra uma issue em
https://github.com/ceseidl/PokerTimeTracker/issues

© Seidl Software Ltda · Carlos Seidl
