# TimePoker — Backlog

Itens conhecidos pra resolver depois do MVP. Prioridade: alta → baixa.

---

## 👥 Lista nominal de jogadores ativos

**Hoje:** "Jogadores" e "Rebuys" são só contadores numéricos agregados.

**O que falta:**
- Lista visual com **nomes** dos jogadores ativos
- Importar do PokerInimigos popula essa lista (vs. só setar o contador)
- Cadastrar manualmente (campo de texto + botão de adicionar)
- "Eliminar" jogador (some da lista de ativos ou marca como eliminado)
- Persistência da lista no recovery

## 🪙 Rebuy associado ao jogador

**Hoje:** "+ Rebuy" só incrementa o contador agregado.

**Comportamento desejado:** ao clicar **+ Rebuy** → abre diálogo "Quem fez o rebuy?"
com a lista de jogadores ativos. O rebuy fica registrado na ficha individual:
*"Carlos · 2 rebuys"*. Display continua mostrando o total agregado, mas o
controle mostra os nomes com seus rebuys.

**Decisão pendente** (ver conversa do dia 2026-05-04): a lista deve incluir
apenas ativos ou todos com flag de status. Resolver junto com o item acima
quando entrar no escopo.

## ❌ Eliminação de jogador

Junto com a lista nominal: marcar quem foi eliminado na noite, opcionalmente
com posição final (5º, 4º…) pra alimentar futuras integrações com ranking.

---

## 🔊 Alarme — permitir trocar dispositivo de áudio

**Problema atual:** o alarme toca pelo dispositivo de saída **padrão do Windows**.
Quando o notebook está conectado em TV via HDMI, o Windows pode mudar o
padrão pra a TV automaticamente — e aí o som do alarme vai pra TV em vez
de tocar no speaker do notebook que o anfitrião usa pra ouvir.

**Workaround atual:** trocar o dispositivo padrão em
*Configurações › Sistema › Som › Saída*.

**O que tentamos:** primeira versão da seleção via `WasapiOut(MMDevice)` —
o som continuou indo pra saída padrão mesmo com o device específico. Causa
provável: WASAPI shared mode com formato/sample rate incompatível, ou problema
em como o device é resolvido pelo `MMDeviceEnumerator`.

**Ideias pra resolver:**
- Investigar `WasapiOut` com modo exclusivo + formato negociado
- Testar `DirectSoundOut(deviceGuid)` — API mais antiga mas confiável
- Testar `WaveOutEvent.DeviceNumber` (waveOutXxx) com mapeamento manual
  CoreAudio ID → waveOutDevice ID
- Adicionar persistência da escolha (`%APPDATA%\TimePoker\config.json`)

---

## 🎵 Alarme — som customizável

- Permitir trocar o som padrão por um arquivo `.wav`/`.mp3` do usuário
- Sons pré-definidos opcionais (sino, gong, voz)
- Volume independente do volume da app

## 🎙️ Anúncio falado dos blinds

Quando o nível muda, anúncio em voz: *"Blinds 200 e 400. Ante 50."*
Usar `System.Speech.Synthesis` (TTS do Windows, Brasil-PT).

## 📊 Editor de estrutura (CRUD de níveis)

Hoje só é possível ver e usar pré-sets. Falta um editor pra adicionar /
remover / reordenar / editar SB/BB/Ante/Duração de cada nível ao vivo.

## 💾 Salvar / carregar estruturas nomeadas

`%APPDATA%\TimePoker\structures\<nome>.json`. Lista nomeada na UI,
botões "Salvar como..." e "Carregar...".

## 👥 Importar PokerInimigos

Lê `participantes.md` da pasta vizinha → pré-popula uma lista de jogadores
com checkboxes em vez de só um número de jogadores.

## 💰 Calculadora de premiação

Distribui o prize pool entre top N segundo % configuráveis (50/30/20,
60/40, etc). Mostra os valores no rodapé do display.

## 🃏 Calculadora de stack inicial

Quantas fichas de cada cor distribuir pra cada jogador no buy-in.

## 🎨 Editor de cores/valores das fichas

Mostrar na display as cores e valores das fichas em uso.

## 🪑 Gerenciamento de mesas e assentos

Distribuição aleatória, balanceamento automático, mesa final.

## 📅 Sistema de liga / temporadas

Acumular resultados de várias noites, ranking, fórmula de pontuação.
Possível integração com o painel **PokerInimigos** (que já tem temporada).

## 📡 Casting / streaming pra smart TV

Modo "servidor web local" pra abrir o display num navegador (smart TV,
Chromecast, etc) em vez de usar HDMI.

## 📚 Dicionário, citações e regras TDA

Como no Blinds Are Up! original — recursos secundários, baixa prioridade.

## 🌐 Outros cenários de execução

Conforme a conversa de planejamento (escopo MVP foi notebook + TV via HDMI):
- PC fixo dedicado na sala de jogo
- Tablet / celular
