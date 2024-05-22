
try {
    function outputAlert(err) {
        let dll = loaddll("HmOutputPane.dll");
        dll.dllFunc.Output(hidemaru.getCurrentWindowHandle(), err + "\r\n");
    }

    function throwIfNotFileName() {
        // ファイル名無しは対象にしない
        if (!hidemaru.getFileFullPath()) {
            outputAlert("ファイル名が付いているファイルのみ対象とします。");
            throwCancelException();
        }
    }

    throwIfNotFileName();

    const currentmacrodir = currentmacrodirectory();

    // ラベルはこのマクロ独自にしておく
    const strTargetLabel = "HmSplitTextFileByRegex";

    // レンダリングペインを開く
    function openRenderPane() {
        // カレントマクロフォルダのhtmlファイルを使う
        const absoluteUrl = new URL(currentmacrodir + "\\" + "HmSplitTextFileByRegex.help.html").href;


        const json_arg = {
            target: strTargetLabel,
            uri: absoluteUrl,
            show: 1,
            place: "leftside",
        };

        renderpanecommand(json_arg);
    }

    // 本当にレンダリングペインが開いて準備がととのったのかのチェック
    let checkCount_HmSplitTextFileByRegex = 0;
    function checkComplete_HmSplitTextFileByRegex() {
        let readyState = renderpanecommand({ target: strTargetLabel, get: "readyState" });
        if (readyState == "complete") {
            hidemaru.clearInterval(idIntervalInitialize_HmSplitTextFileByRegex);
            onRenderPaneShown_HmSplitTextFileByRegex();
        }
    }

    openRenderPane();

    // 次のマクロ実行の際に見るのでletではなくvarにして、ライフサイクルを js { } 外に伸ばす必要がある。
    // 他のjsを使った秀丸マクロと決して被らない「マクロ空間」にしておく必要がある。(よってtickcount空間という被らない空間に配置している)

    // 最初のレンダリングペインCompleteチェック用のものは、比較的細やかにチェックする。
    // これは表示しをえたら直ぐにIntervalが止まるのでIntervalが小さくでも大丈夫
    var idIntervalInitialize_HmSplitTextFileByRegex;
    if (typeof (idIntervalInitialize_HmSplitTextFileByRegex) != "undefined") {
        hidemaru.clearInterval(idIntervalInitialize_HmSplitTextFileByRegex);
    }
    idIntervalInitialize_HmSplitTextFileByRegex = hidemaru.setInterval(checkComplete_HmSplitTextFileByRegex, 300);
} catch(err) {
    outputAlert(err);
}
