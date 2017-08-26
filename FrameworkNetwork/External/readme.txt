
【更新内容】

========================================================================================
2017/08/26

Connect() 時にサーバーが立ち上がっていない場合、90秒のフリーズが発生してしまう。
ConnectAsync() に変えると、Close() 時に同様のフリーズが発生する。
これは90000ミリ秒のタイムアウト指定がハードコードされていることが原因のもよう。

そのため5000ミリ秒（５秒）へ変更してビルドし直した。

C:\pie\Unity\Framework\External\websocket_sharp\websocket-sharp\bin\Release
websocket-sharp.dll

https://github.com/sta/websocket-sharp/pull/293
https://github.com/sta/websocket-sharp/pull/293/files

========================================================================================
