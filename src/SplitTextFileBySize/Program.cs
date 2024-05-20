using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace SplitTextFileBySize;

internal class Program
{

    static string getDivitionFileName(string filePath, int num, int numberWidth)
    {
        var strNumber = num.ToString("D" + numberWidth);

        // ファイル名を拡張子とそれ以外に分ける
        string fileName = Path.GetFileNameWithoutExtension(filePath); // 拡張子を除いたファイル名を取得
        string fileExtension = Path.GetExtension(filePath); // 拡張子を取得

        string newFileName = fileName + "_" + strNumber + fileExtension;
        return newFileName;
    }

    static void Main(string[] args)
    {
        try
        {
            // argの1番目にファイル名
            string filePath = args[0];

            // argの2番目にカットしたい長さ
            int targetCurLength = int.Parse(args[1]);

            // argの3番目にエンコード
            int targetEncodingCodePage = int.Parse(args[2]);

            // argの4番目に改行モード
            int targetLineMode = 0;
            if (args.Length >= 4)
            {
                targetLineMode = int.Parse(args[3]);
            }

            var encode = System.Text.Encoding.GetEncoding(targetEncodingCodePage);
            // ファイルを読み込む
            string allText = System.IO.File.ReadAllText(filePath, encode);

            // 文字列をバイト配列に変換

            List<string> fileTextList = new List<string>();
            while (true)
            {
                int cutLength = targetCurLength;
                int minLength = cutLength;
                if (minLength > allText.Length)
                {
                    minLength = allText.Length;
                }
                string headText = allText.Substring(0, minLength); // 高速化するためにヘッダー
                byte[] headerArray = System.Text.Encoding.GetEncoding(targetEncodingCodePage).GetBytes(headText);

                if (cutLength > headerArray.Count())
                {
                    cutLength = headerArray.Count();
                }

                byte[] cutArray = new byte[cutLength];
                Array.Copy(headerArray, 0, cutArray, 0, cutLength);

                try
                {
                    // カット後の配列を文字列に変換
                    string cutStr = System.Text.Encoding.GetEncoding(targetEncodingCodePage).GetString(cutArray);

                    if (headerArray.Length < targetCurLength)
                    {
                        fileTextList.Add(cutStr);
                        break;
                    }

                    // curStrから1文字カット
                    cutStr = cutStr.Substring(0, cutStr.Length - 1);
                    // 末尾が\rなら削除
                    if (cutStr.EndsWith("\r"))
                    {
                        cutStr = cutStr.Substring(0, cutStr.Length - 1);
                    }

                    // 改行を考慮するモード
                    if (targetLineMode == 1)
                    {
                        int lastNewLineIndex = cutStr.LastIndexOf('\n');
                        if (lastNewLineIndex != -1)
                        {
                            cutStr = cutStr.Substring(0, lastNewLineIndex + 1);
                        }
                    }
                    fileTextList.Add(cutStr);

                    // strからcutStrを先頭から削除
                    allText = allText.Substring(cutStr.Length);

                    if (allText == "")
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }

            /*
            for (int i = 0; i < fileTextList.Count; i++)
            {
                var newFilePath = getDivitionFileName(filePath, i, numberWidth);
                // list.Countの数値の長さを0埋めする
                Console.WriteLine(newFilePath);
                // ファイルに書き込む
                System.IO.File.WriteAllText(newFilePath, fileTextList[i], encode);

            }
            */

            // ファイル作成は重いので、並列処理してしまう。
            var count = fileTextList.Count;
            var numberWidth = count.ToString().Length;
            Parallel.For(0, fileTextList.Count, i =>
            {
                var newFilePath = getDivitionFileName(filePath, i + 1, numberWidth);

                // ファイルに書き込む
                System.IO.File.WriteAllText(newFilePath, fileTextList[i], encode);
            });

            Console.WriteLine(count + "個のファイルに分割しました。");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}

