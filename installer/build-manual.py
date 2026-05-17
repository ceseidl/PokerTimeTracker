"""
Converte docs/MANUAL.md em docs/MANUAL.html autocontido.

Saída: 1 único arquivo HTML que abre no navegador, mesmo offline / movido —
sem dependências externas. Imagens locais (se houver) viram base64 inline.

Pré-requisito: pip install markdown
"""
import base64
import re
import sys
from pathlib import Path
import markdown

ROOT = Path(__file__).resolve().parent.parent
DOCS = ROOT / "docs"
SRC = DOCS / "MANUAL.md"
OUT = DOCS / "MANUAL.html"


def md_to_html(md_text: str) -> str:
    return markdown.markdown(
        md_text,
        extensions=["tables", "fenced_code", "toc", "attr_list", "sane_lists"],
        output_format="html5",
    )


def embedar_imagens(html: str) -> str:
    """Substitui <img src="..."> locais por base64 (PNG)."""
    def trocar(m):
        caminho_rel = m.group(1).split("#")[0].split("?")[0]
        arquivo = (DOCS / caminho_rel).resolve()
        if not arquivo.exists():
            print(f"  ! imagem não encontrada: {arquivo}", file=sys.stderr)
            return m.group(0)
        b64 = base64.b64encode(arquivo.read_bytes()).decode("ascii")
        return f'src="data:image/png;base64,{b64}"'

    return re.sub(r'src="((?!data:|http)[^"]+\.png)"', trocar, html)


def embrulhar(corpo: str) -> str:
    return f"""<!DOCTYPE html>
<html lang="pt-BR">
<head>
  <meta charset="utf-8" />
  <title>Manual do Usuário · TimePoker</title>
  <style>
    :root {{
      --bg: #0A0E0B;
      --card: #16201C;
      --gold: #C9A24B;
      --gold-bright: #E5BE6E;
      --gold-soft: #8E7335;
      --text: #F5EFE0;
      --text-mute: #A89F86;
      --green-dark: #1F3A2A;
      --red: #B8404A;
    }}
    * {{ box-sizing: border-box; }}
    body {{
      background: var(--bg); color: var(--text);
      font-family: 'Segoe UI', system-ui, -apple-system, sans-serif;
      line-height: 1.65; margin: 0; padding: 0;
    }}
    main {{ max-width: 920px; margin: 0 auto; padding: 40px 28px 80px; }}
    h1, h2, h3, h4 {{ color: var(--gold-bright); letter-spacing: 0.01em; }}
    h1 {{ font-size: 2.2rem; margin-top: 0; border-bottom: 2px solid var(--gold-soft); padding-bottom: 12px; }}
    h2 {{ font-size: 1.5rem; margin-top: 2.4em; padding-bottom: 6px; border-bottom: 1px solid var(--gold-soft); }}
    h3 {{ font-size: 1.15rem; margin-top: 1.8em; color: var(--gold); }}
    h4 {{ font-size: 1rem; margin-top: 1.4em; }}
    a {{ color: var(--gold-bright); text-decoration: none; }}
    a:hover {{ text-decoration: underline; }}
    blockquote {{
      border-left: 4px solid var(--gold); background: var(--card);
      margin: 1.4em 0; padding: 12px 18px;
      color: var(--text-mute); font-style: italic;
    }}
    code {{
      background: var(--card); color: var(--gold-bright);
      padding: 2px 6px; border-radius: 3px;
      font-family: 'Consolas', monospace; font-size: 0.92em;
    }}
    pre {{
      background: var(--card); border: 1px solid var(--gold-soft);
      border-radius: 4px; padding: 14px 18px; overflow-x: auto; font-size: 0.9rem;
    }}
    pre code {{ background: none; padding: 0; color: var(--text); }}
    table {{ border-collapse: collapse; width: 100%; margin: 1.2em 0; background: var(--card); }}
    th, td {{ border: 1px solid var(--gold-soft); padding: 9px 14px; text-align: left; vertical-align: top; }}
    th {{ background: var(--green-dark); color: var(--gold-bright); font-weight: 600; }}
    img {{
      max-width: 100%; height: auto;
      border: 1px solid var(--gold-soft); border-radius: 4px;
      margin: 14px auto; display: block;
      box-shadow: 0 4px 16px rgba(0,0,0,0.4);
    }}
    hr {{ border: none; border-top: 1px solid var(--gold-soft); margin: 2.8em 0; }}
    ul, ol {{ padding-left: 24px; }}
    li {{ margin: 0.3em 0; }}
    strong {{ color: var(--gold-bright); }}
  </style>
</head>
<body>
<main>
{corpo}
</main>
</body>
</html>
"""


def main():
    if not SRC.exists():
        print(f"ERRO: {SRC} não encontrado", file=sys.stderr); sys.exit(1)
    md = SRC.read_text(encoding="utf-8")
    html_corpo = md_to_html(md)
    html_corpo = embedar_imagens(html_corpo)
    html_final = embrulhar(html_corpo)
    OUT.write_text(html_final, encoding="utf-8")
    tamanho_kb = OUT.stat().st_size // 1024
    print(f"OK: {OUT} ({tamanho_kb} KB)")


if __name__ == "__main__":
    main()
