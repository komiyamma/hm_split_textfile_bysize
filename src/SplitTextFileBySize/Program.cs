using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Text;

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

            // argの4番目にBOMがあるかどうか
            int hasBom = int.Parse(args[3]);

            // argの5番目に改行モード
            int targetLineMode = 0;
            if (args.Length >= 5)
            {
                targetLineMode = int.Parse(args[4]);
            }

            var encode = System.Text.Encoding.GetEncoding(targetEncodingCodePage);
            // ファイルを読み込む
            string allText = System.IO.File.ReadAllText(filePath, encode);

            // 文字列をバイト配列に変換

            List<string> fileTextList = new List<string>();
            while (true)
            {
                // まず、minLengthまわりの処理は不要なのであるが、「allText」をすべてバイト配列にすると、長大だと非常に不可が大きくなるため、
                // 調査の対象を短くしている。
                // 「最大でもallText」の長さ、「最低で」targetCurLengthの長さがあれば良い。
                // なぜなら、「文字列」のバイト配列の長さは、かならず「文字列」の長さ(改行等含む)以上だからである。
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

                // 調査範囲として、短くしたバイト配列を確保
                byte[] cutArray = new byte[cutLength];
                Array.Copy(headerArray, 0, cutArray, 0, cutLength);

                try
                {
                    // 調査範囲のものをバイト範囲から再び文字列ｊに戻す
                    string cutStr = System.Text.Encoding.GetEncoding(targetEncodingCodePage).GetString(cutArray);

                    // その長さが、指定されているバッファーよりも短いのであれば、最早調査する必要はない。
                    // まるまるその文字列は、１つのファイルに対応する形で書き込む対象となる。
                    if (headerArray.Length < targetCurLength)
                    {
                        fileTextList.Add(cutStr);
                        break;
                    }

                    // 一番最後の「1文字」はバイトが中途半端で壊れている可能性がきわめて高いので1文字カット（次のループで処理される）
                    cutStr = cutStr.Substring(0, cutStr.Length - 1);

                    // 末尾が\rならカット(次のループで処理される)
                    if (cutStr.EndsWith("\r"))
                    {
                        cutStr = cutStr.Substring(0, cutStr.Length - 1);
                    }

                    // 改行を考慮するモード
                    if (targetLineMode == 1)
                    {
                        // 改行あるならば、そこまでを範囲とする。(残りは次のループで処理される）
                        int lastNewLineIndex = cutStr.LastIndexOf('\n');
                        if (lastNewLineIndex != -1)
                        {
                            cutStr = cutStr.Substring(0, lastNewLineIndex + 1);
                        }
                    }

                    // ここまで処理した文字列を１つのファイルに対応する文字列として登録
                    fileTextList.Add(cutStr);

                    // 全体の文字列の先頭から先ほど使用した文字列分を削除して、次の処理へ
                    allText = allText.Substring(cutStr.Length);

                    // 削除したら空っぽになったのなら、次のループに入るまでもなくここで終了
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

            // utf8でかつBOM無しの場合、BOM無しで
            if (hasBom == 0 && targetEncodingCodePage == 65001)
            {
                var utf8nobom = new UTF8Encoding(false);
                Parallel.For(0, fileTextList.Count, i =>
                {
                    var newFilePath = getDivitionFileName(filePath, i + 1, numberWidth);
                    

                    // 上書き保存でBOM無。
                    using (StreamWriter sw = new StreamWriter(newFilePath, false, utf8nobom))
                    {
                        // ファイルに書き込む
                        sw.Write(fileTextList[i]);
                    }
                });
            }
            // こっちは各種エンコーディングの主流に従う
            else
            {
                Parallel.For(0, fileTextList.Count, i =>
                {
                    var newFilePath = getDivitionFileName(filePath, i + 1, numberWidth);

                    // ファイルに書き込む
                    System.IO.File.WriteAllText(newFilePath, fileTextList[i], encode);
                });
            }

            Console.WriteLine(count + "個のファイルに分割しました。");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}

