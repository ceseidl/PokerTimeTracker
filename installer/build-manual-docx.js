// Gera docs/Manual_Usuario.docx do TimePoker — linguagem simples para o usuário final.
//
// Uso:  node installer/build-manual-docx.js
// Requer:  npm install -g docx
const fs = require('fs');
const path = require('path');
const {
  Document, Packer, Paragraph, TextRun, Table, TableRow, TableCell,
  AlignmentType, LevelFormat, HeadingLevel, BorderStyle, WidthType,
  ShadingType, ExternalHyperlink
} = require('docx');

const GOLD = "B07F1A";
const GREEN = "1F3A2A";
const GREY = "555555";

const heading = (text, level) => new Paragraph({ heading: level, children: [new TextRun({ text })] });
const p = (text, opts = {}) => new Paragraph({ spacing: { after: 120 }, children: [new TextRun({ text, ...opts })] });
const bullet = (text) => new Paragraph({ numbering: { reference: "bullets", level: 0 }, children: [new TextRun({ text })] });
const step = (text) => new Paragraph({ numbering: { reference: "steps", level: 0 }, children: [new TextRun({ text })] });
const note = (text) => new Paragraph({
  spacing: { before: 80, after: 160 },
  shading: { fill: "FFF6DC", type: ShadingType.CLEAR },
  border: { left: { style: BorderStyle.SINGLE, size: 24, color: GOLD, space: 8 } },
  children: [new TextRun({ text: "Dica: ", bold: true, color: GOLD }), new TextRun({ text })]
});
const link = (text, url) => new Paragraph({
  spacing: { after: 120 },
  children: [new ExternalHyperlink({ children: [new TextRun({ text, style: "Hyperlink" })], link: url })]
});

const tableBorder = { style: BorderStyle.SINGLE, size: 4, color: "CCCCCC" };
const tableBorders = { top: tableBorder, bottom: tableBorder, left: tableBorder, right: tableBorder, insideHorizontal: tableBorder, insideVertical: tableBorder };

function tbl(headers, rows, columnWidths) {
  const total = columnWidths.reduce((a, b) => a + b, 0);
  const headerRow = new TableRow({ tableHeader: true,
    children: headers.map((h, i) => new TableCell({
      width: { size: columnWidths[i], type: WidthType.DXA },
      shading: { fill: GREEN, type: ShadingType.CLEAR },
      margins: { top: 80, bottom: 80, left: 120, right: 120 },
      children: [new Paragraph({ children: [new TextRun({ text: h, bold: true, color: "FFFFFF" })] })]
    }))
  });
  const dataRows = rows.map(row => new TableRow({
    children: row.map((cell, i) => new TableCell({
      width: { size: columnWidths[i], type: WidthType.DXA },
      margins: { top: 80, bottom: 80, left: 120, right: 120 },
      children: [new Paragraph({ children: [new TextRun({ text: cell })] })]
    }))
  }));
  return new Table({ width: { size: total, type: WidthType.DXA }, columnWidths, borders: tableBorders, rows: [headerRow, ...dataRows] });
}

const doc = new Document({
  styles: {
    default: { document: { run: { font: "Calibri", size: 22 } } },
    paragraphStyles: [
      { id: "Heading1", name: "Heading 1", basedOn: "Normal", next: "Normal", quickFormat: true,
        run: { size: 40, bold: true, font: "Calibri", color: GREEN },
        paragraph: { spacing: { before: 360, after: 200 }, outlineLevel: 0 } },
      { id: "Heading2", name: "Heading 2", basedOn: "Normal", next: "Normal", quickFormat: true,
        run: { size: 30, bold: true, font: "Calibri", color: GREEN },
        paragraph: { spacing: { before: 320, after: 160 }, outlineLevel: 1 } },
      { id: "Heading3", name: "Heading 3", basedOn: "Normal", next: "Normal", quickFormat: true,
        run: { size: 26, bold: true, font: "Calibri", color: GOLD },
        paragraph: { spacing: { before: 240, after: 120 }, outlineLevel: 2 } },
      { id: "Title", name: "Title", basedOn: "Normal", next: "Normal", quickFormat: true,
        run: { size: 56, bold: true, font: "Calibri", color: GREEN },
        paragraph: { spacing: { after: 120 }, alignment: AlignmentType.CENTER } },
      { id: "Subtitle", name: "Subtitle", basedOn: "Normal", next: "Normal", quickFormat: true,
        run: { size: 26, italics: true, color: GREY, font: "Calibri" },
        paragraph: { spacing: { after: 240 }, alignment: AlignmentType.CENTER } },
    ]
  },
  numbering: {
    config: [
      { reference: "bullets",
        levels: [{ level: 0, format: LevelFormat.BULLET, text: "•", alignment: AlignmentType.LEFT,
          style: { paragraph: { indent: { left: 720, hanging: 360 } } } }] },
      { reference: "steps",
        levels: [{ level: 0, format: LevelFormat.DECIMAL, text: "%1.", alignment: AlignmentType.LEFT,
          style: { paragraph: { indent: { left: 720, hanging: 360 } } } }] },
    ]
  },
  sections: [{
    properties: {
      page: { size: { width: 12240, height: 15840 },
              margin: { top: 1440, right: 1440, bottom: 1440, left: 1440 } }
    },
    children: [
      new Paragraph({ style: "Title", children: [new TextRun({ text: "TimePoker" })] }),
      new Paragraph({ style: "Subtitle", children: [new TextRun({ text: "Manual do Usuário — Versão 1.2.0" })] }),
      new Paragraph({ alignment: AlignmentType.CENTER, spacing: { after: 480 },
        children: [new TextRun({ text: "Cronômetro de torneios de poker ao vivo", italics: true, color: GREY })] }),

      heading("O que é o TimePoker?", HeadingLevel.HEADING_1),
      p("É um aplicativo simples para o Windows que serve como cronômetro oficial das noites de torneio. Ele mostra na TV o tempo do nível, os blinds atuais, o próximo nível, o prêmio acumulado e o número de jogadores ativos."),
      p("O TimePoker abre em duas janelas:"),
      bullet("Controle: a janela onde você comanda o jogo (iniciar, pausar, mudar de nível)."),
      bullet("Display: a tela cheia para a TV — letras enormes, fácil de ler de longe."),
      note("Quando o gerenciador Inimigos do Royal Flush está instalado no mesmo computador, os dois conversam automaticamente: o gerenciador envia presentes/rebuys ao timer, e o timer devolve a duração da partida ao gerenciador."),

      heading("Antes de começar — o que você precisa", HeadingLevel.HEADING_1),
      bullet("Um computador com Windows 10 ou 11 (64 bits)."),
      bullet("Espaço em disco: aproximadamente 200 MB."),
      bullet("Opcional: uma TV ou segundo monitor ligado por HDMI para mostrar o display."),
      p("Você NÃO precisa instalar o .NET nem nenhum outro programa — o instalador já vem com tudo dentro."),

      heading("1. Como baixar", HeadingLevel.HEADING_1),
      step("Abra o navegador no link abaixo."),
      link("https://github.com/ceseidl/PokerTimeTracker/releases/latest", "https://github.com/ceseidl/PokerTimeTracker/releases/latest"),
      step("Procure a seção \"Assets\" (Anexos) no final da página."),
      step("Clique no arquivo que termina em .msi — algo como TimePoker-1.2.0.0.msi."),
      step("Aguarde o download terminar."),
      note("Se o navegador avisar \"este arquivo não é baixado com frequência\", clique em Manter / Confiar — é seguro."),

      heading("2. Como instalar", HeadingLevel.HEADING_1),
      step("Vá até a pasta Downloads e dê dois cliques no arquivo TimePoker-...msi."),
      step("Pode ser que o Windows mostre um aviso azul (\"O Windows protegeu o computador\"). Clique em Mais informações → Executar assim mesmo."),
      step("Vai abrir o instalador. Clique em Avançar."),
      step("Aceite os termos da licença, clique em Avançar."),
      step("Confirme a pasta de instalação, clique em Avançar."),
      step("Clique em Instalar. O Windows pode pedir permissão de administrador — clique em Sim."),
      step("Aguarde a barra encher. Quando terminar, clique em Concluir."),
      p("Pronto. Vai aparecer um atalho TimePoker na Área de Trabalho e no menu Iniciar."),

      heading("3. A primeira vez que abrir", HeadingLevel.HEADING_1),
      step("Clique duas vezes no ícone do TimePoker."),
      step("Aparece uma splash screen dourada por alguns segundos."),
      step("Em seguida abrem duas janelas ao mesmo tempo: Controle (menor) e Display (maior)."),
      step("Se você usa só o notebook, arraste o Display para um cantinho. Se tem TV via HDMI, arraste o Display para a TV e pressione F11 para ele virar tela cheia."),
      note("A estrutura já vem pronta com o pré-set oficial da Liga: \"Inimigos do Royal Flush\" (6 níveis de 20 minutos + 2 intervalos). Você só precisa apertar Iniciar."),

      heading("4. Operando uma noite de torneio", HeadingLevel.HEADING_1),

      heading("Iniciar e pausar", HeadingLevel.HEADING_2),
      bullet("Botão ▶ Iniciar / Pausar: começa ou pausa o cronômetro. (Ou aperte Espaço.)"),
      bullet("No Display: tem um ⏯ pequenininho no canto superior direito que faz a mesma coisa — útil quando você está com só uma tela."),

      heading("Avançar ou voltar de nível", HeadingLevel.HEADING_2),
      bullet("◀ Anterior: volta um nível."),
      bullet("Próximo ▶: pula para o nível seguinte (cronômetro reinicia com o tempo daquele nível)."),
      bullet("− 1 min / + 1 min: ajusta o tempo do nível atual se precisar dar mais alguns minutos ou cortar."),

      heading("Jogadores e rebuys", HeadingLevel.HEADING_2),
      bullet("Use + e − ao lado de \"Jogadores\" pra ajustar quantos estão na mesa."),
      bullet("Use + e − ao lado de \"Rebuys\" cada vez que alguém recomprar."),
      bullet("O Prize Pool é calculado sozinho: (Jogadores × Buy-in) + (Rebuys × ValorRebuy)."),
      note("Se você usa o gerenciador Inimigos do Royal Flush, nem precisa mexer aqui: o timer recebe os números do gerenciador automaticamente toda vez que você Salvar Rascunho lá."),

      heading("Finalizar a noite", HeadingLevel.HEADING_2),
      p("Quando o torneio acabar, clique no botão 🏁 Finalizar (tem no painel de Controle e no canto do Display)."),
      p("Isso faz duas coisas:"),
      bullet("Pausa o cronômetro."),
      bullet("Envia para o gerenciador (se ele estiver instalado) o tempo total que a partida durou."),
      p("Você verá uma janelinha confirmando \"Tempo total enviado: HH:MM:SS\"."),
      note("Não fecha a rodada no gerenciador — só manda a informação do tempo. O anfitrião continua precisando \"Finalizar Rodada\" lá no gerenciador depois de lançar os resultados."),

      heading("5. Atalhos úteis", HeadingLevel.HEADING_1),
      tbl(
        ["Tecla", "O que faz"],
        [
          ["Espaço", "Inicia ou pausa o cronômetro"],
          ["←", "Volta um nível"],
          ["→", "Avança um nível"],
          ["F11", "Tela cheia / sai da tela cheia (na janela Display)"],
          ["Esc", "Sai da tela cheia"]
        ],
        [3000, 6360]
      ),

      heading("6. Mudando a estrutura do torneio", HeadingLevel.HEADING_1),
      p("Na parte de baixo do Controle tem o quadro \"Estrutura\" com cinco botões para escolher rapidamente:"),
      bullet("Turbo — noite curta (≈2h), níveis de 10 minutos."),
      bullet("Padrão — equilibrado (≈3h)."),
      bullet("Deep stack — noite longa (≈4h+), níveis de 30 minutos."),
      bullet("Inimigos do Royal Flush — o oficial da Liga (6 níveis de 20 min + 2 breaks)."),
      bullet("Vazia — começa do zero, você adiciona linha por linha."),
      p("Para adicionar um nível novo: clique + Nível (ele já vem com 20 min). Para adicionar um intervalo: + Break."),
      p("Para apagar um nível: clique nele na lista e depois no botão −."),
      p("Para mudar SB, BB ou duração: dê um clique na célula e edite direto."),

      heading("7. Salvando e carregando estruturas", HeadingLevel.HEADING_1),
      p("Se você montou uma estrutura customizada e quer reusar em outra noite:"),
      step("Clique em 💾 Salvar como..."),
      step("Dê um nome (ex: \"Aniversário do Carlos\")."),
      step("Da próxima vez, clique em 📂 Carregar e escolha o nome na lista."),
      note("A estrutura oficial \"Inimigos do Royal Flush\" já vem salva automaticamente na primeira execução."),

      heading("8. Problemas comuns", HeadingLevel.HEADING_1),

      heading("\"Esqueci de pausar e parei o jogo, perdi o tempo?\"", HeadingLevel.HEADING_3),
      p("Não. Reabra o TimePoker — ele detecta a sessão anterior e pergunta se quer restaurar. Diga Sim e o tempo volta como Pausado (por segurança), você só clica Iniciar quando quiser continuar."),

      heading("\"O alarme não está tocando.\"", HeadingLevel.HEADING_3),
      p("Confira se o 🔔 Alarme está marcado. Se sim, vá em Configurações do Windows → Som → Saída e veja se o som está saindo para o dispositivo certo (TV vs Notebook)."),

      heading("\"O Display não foi pra TV.\"", HeadingLevel.HEADING_3),
      p("Verifique se a TV está conectada via HDMI e configurada como Estender (não Duplicar) no Windows. Depois, no Controle, clique \"Display fullscreen na 2ª tela\"."),

      heading("\"Cliquei Finalizar mas o gerenciador não recebeu o tempo.\"", HeadingLevel.HEADING_3),
      p("Abra o gerenciador, vá em Cadastro da Rodada e clique no botão ↻ Importar do timer. Ele vai puxar o tempo. Se mesmo assim não pegar, confirme que o gerenciador é versão 1.4.0 ou mais nova."),

      heading("Suporte", HeadingLevel.HEADING_1),
      p("Para reportar problemas ou sugerir melhorias:"),
      link("https://github.com/ceseidl/PokerTimeTracker/issues", "https://github.com/ceseidl/PokerTimeTracker/issues"),
      p(""),
      new Paragraph({ alignment: AlignmentType.CENTER,
        children: [new TextRun({ text: "© Seidl Software Ltda · Carlos Seidl", color: GREY, italics: true })] }),
    ]
  }]
});

const outPath = path.join(__dirname, "..", "docs", "Manual_Usuario.docx");
Packer.toBuffer(doc).then(buf => {
  fs.writeFileSync(outPath, buf);
  console.log("OK:", outPath, "(" + Math.round(buf.length / 1024) + " KB)");
});
