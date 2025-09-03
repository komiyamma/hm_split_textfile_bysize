# HmSplitTextFileBySize

秀丸で今開いているファイルを、指定の「キロバイト数」で分割するマクロです。

## 主な機能

*   UI（ダイアログ）を使って、対話的に分割したいファイルサイズ（KB）を指定できます。
*   マクロ内の値を直接編集することで、UIを使わずに常に同じ設定でファイルを分割することも可能です。

## 動作環境

*   秀丸エディタ v9.25 以上

## インストール

ダウンロードしたzipファイルを解凍し、以下のファイルをすべて同じフォルダーに配置してください。

*   `SplitTextFileBySize.exe`
*   `HmSplitTextFileBySize.ui.mac`
*   `HmSplitTextFileBySize.value.mac`
*   `HmSplitTextFileBySize.main.mac`
*   `HmSplitTextFileBySize.ui.html`

## 使い方

**注意点:** 分割されたファイルは、元のファイルと同じフォルダに `ファイル名_001.txt`, `ファイル名_002.txt` ... のように連番で作成されます。意図しない場所にファイルが作成されるのを防ぐため、**専用の作業フォルダに分割したいファイルをコピーしてから実行する**ことをお勧めします。

### UIを使ってサイズを指定する場合

1.  分割したいテキストファイルを秀丸で開きます。
2.  `HmSplitTextFileBySize.ui.mac` を実行します。
3.  表示されたダイアログに、1ファイルあたりのサイズをキロバイト単位で入力し、「分割」ボタンを押します。

### マクロで直接サイズを指定する場合

毎回同じサイズで分割するなど、UIが不要な場合はこちらを利用します。

1.  `HmSplitTextFileBySize.value.mac` をテキストエディタで開きます。
2.  ファイル内の `SplitSize` 変数の値を、希望のキロバイト数に編集して保存します。
3.  分割したいテキストファイルを秀丸で開きます。
4.  編集した `HmSplitTextFileBySize.value.mac` を実行します。

## ライセンス

*   MIT License

## 解説サイト

より詳しい情報や更新履歴については、以下のサイトをご覧ください。

https://秀丸マクロ.net/?page=nobu_tool_hm_split_textfile_bysize
