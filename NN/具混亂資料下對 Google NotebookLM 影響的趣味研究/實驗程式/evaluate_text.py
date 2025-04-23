#!/usr/bin/env python
"""
evaluate_text.py — 計算候選文本 (candidate) 與參考文本 (reference) 的 BLEU 與 ROUGE 分數
=======================================================================

此腳本可同時計算 BLEU‑1~4 以及 ROUGE‑1/2/L，並支援：

* **中文與英文**：中文自動以 jieba 斷詞，英文則使用空白切分。
* **語料層級 (corpus) 與句子層級 (sentence) 評估**
* **JSON 與人類可讀 (pretty) 輸出**

安裝相依套件::

    pip install nltk==3.* rouge-score jieba

如果只需 ROUGE，可省略 nltk；若只需 BLEU，可省略 rouge‑score。

用法範例::

    # 計算全部指標，輸出為漂亮的文字表格
    python evaluate_text.py -c candidate.txt -r reference.txt 

    # 僅算 ROUGE，輸出 JSON，逐句平均
    python evaluate_text.py -c cand.txt -r ref.txt -m rouge -l sentence -o json

"""

from __future__ import annotations

import argparse
import json
import sys
from pathlib import Path
from typing import List, Sequence

# --- 相依套件導入 -----------------------------------------------------------

try:
    from nltk.translate.bleu_score import corpus_bleu, SmoothingFunction  
except ModuleNotFoundError:  
    corpus_bleu = None  

try:
    from rouge_score import rouge_scorer  
except ModuleNotFoundError:  
    rouge_scorer = None  

try:
    import jieba  
except ModuleNotFoundError:  
    jieba = None  

# ---------------------------------------------------------------------------

def read_text(path: Path) -> str:
    """以 UTF‑8 讀取檔案，如果不存在則印錯並退出。"""
    if not path.exists():
        sys.exit(f"[Error] File not found: {path}")
    return path.read_text(encoding="utf-8")


def tokenize(text: str, lang: str) -> List[str]:
    """依語言簡易斷詞。中文使用 jieba，英文以空白切分。"""
    if lang == "zh":
        if jieba is None:
            sys.exit("[Error] jieba is not installed but required for Chinese tokenization")
        return list(filter(str.strip, jieba.cut(text)))
    else:
        return text.split()


def detect_lang(text: str) -> str:
    """極簡語言偵測：若含中文 Unicode 區段即視為中文。"""
    return "zh" if any("\u4e00" <= ch <= "\u9fff" for ch in text) else "en"


# ------------------------------ BLEU ---------------------------------------

_smooth = SmoothingFunction().method1 if corpus_bleu else None  


def compute_bleu(refs: Sequence[Sequence[str]], cands: Sequence[Sequence[str]]) -> float:
    if corpus_bleu is None:
        sys.exit("[Error] NLTK is not installed; cannot compute BLEU")
    return corpus_bleu(refs, cands, smoothing_function=_smooth)  


# ------------------------------ ROUGE --------------------------------------

_ROUGE_TYPES = ["rouge1", "rouge2", "rougeL"]


def compute_rouge(ref_texts: Sequence[str], cand_texts: Sequence[str]) -> dict[str, float]:
    if rouge_scorer is None:
        sys.exit("[Error] rouge-score is not installed; cannot compute ROUGE")

    scorer = rouge_scorer.RougeScorer(_ROUGE_TYPES, use_stemmer=True)
    agg = {rtype: 0.0 for rtype in _ROUGE_TYPES}

    for ref, cand in zip(ref_texts, cand_texts):
        scores = scorer.score(ref, cand)
        for rtype in _ROUGE_TYPES:
            agg[rtype] += scores[rtype].fmeasure

    for rtype in _ROUGE_TYPES:
        agg[rtype] /= len(cand_texts)
    return agg


# --------------------------- 主程式入口點 ----------------------------------

def parse_args() -> argparse.Namespace:  
    p = argparse.ArgumentParser(description="Evaluate text with BLEU / ROUGE scores")
    p.add_argument("-c", "--candidate", required=True, type=Path, help="候選文本檔案路徑")
    p.add_argument("-r", "--reference", required=True, type=Path, help="參考文本檔案路徑")
    p.add_argument("-m", "--metrics", nargs="*", choices=["bleu", "rouge"], default=["bleu", "rouge"], help="要計算的指標 (預設全部)")
    p.add_argument("-l", "--level", choices=["corpus", "sentence"], default="corpus", help="評估粒度：整體 (corpus) 或逐句平均 (sentence)")
    p.add_argument("-o", "--output", choices=["pretty", "json"], default="pretty", help="輸出格式")
    return p.parse_args()


def split_sentences(text: str) -> List[str]:
    """極簡句子切分：以標點 (。！？!?\n) 為界。若需更準確可接 spaCy 等。"""
    import re

    parts = re.split(r"(?<=[。！？!?])|\n", text)
    return [p.strip() for p in parts if p.strip()]


def main() -> None:  
    args = parse_args()

    cand_text = read_text(args.candidate)
    ref_text = read_text(args.reference)

    lang = detect_lang(cand_text + ref_text)

    if args.level == "sentence":
        cand_units = split_sentences(cand_text)
        ref_units = split_sentences(ref_text)
        if len(cand_units) != len(ref_units):
            sys.exit("[Error] 逐句評估時，candidate 與 reference 句數必須相等")
    else:  
        cand_units = [cand_text]
        ref_units = [ref_text]

    results: dict[str, float | dict[str, float]] = {}

    # --- 斷詞 --------------------------------------------------------------
    cand_tokens_list = [tokenize(t, lang) for t in cand_units]
    ref_tokens_list = [tokenize(t, lang) for t in ref_units]

    # --- BLEU --------------------------------------------------------------
    if "bleu" in args.metrics:
        refs_nested = [[rtok] for rtok in ref_tokens_list]
        bleu = compute_bleu(refs_nested, cand_tokens_list)
        results["bleu"] = bleu

    # --- ROUGE -------------------------------------------------------------
    if "rouge" in args.metrics:
        rouge = compute_rouge(ref_units, cand_units)
        results["rouge"] = rouge

    # --- 輸出 --------------------------------------------------------------
    if args.output == "json":
        print(json.dumps(results, ensure_ascii=False, indent=2))
    else:
        print("\n==== 評估結果 ====")
        if "bleu" in results:
            print(f"BLEU  : {results['bleu']:.4f}")
        if "rouge" in results:
            r: dict[str, float] = results["rouge"]  
            for k in _ROUGE_TYPES:
                print(f"{k.upper():7}: {r[k]:.4f}")


if __name__ == "__main__":
    main()
