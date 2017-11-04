# Framework_Cl_WebSocket 
UnityとWebSocketを利用した、個人研究・開発用のプロジェクトです。  
Assets/ 下のSubtreeで利用することを想定しています。

--- 

### Subtree
#### Subtree の追加
```c#
// リモートリポジトリの追加
git remote add [リモートリポジトリ名] [Gitのパス]

// フェッチ
git fetch [リモートリポジトリ名]

// サブツリーのクローン
git subtree add --prefix=[ローカルに配置するパス（Assets/Subtree）] [リモートリポジトリ名] [サブツリーのブランチ名]
```

#### Subtree への反映
```c#
// 通常通りコミット
git add [サブツリーのパス]/hoge.cs
git commit -m "hogehoge"

// サブツリーへのプッシュ
git subtree push --prefix=[ローカルに配置したパス（Assets/Subtree）] [リモートリポジトリ名] [サブツリーのブランチ名] 
```

#### Subtree の変更の取り込み
```c#
// サブツリーのプル
git subtree pull --prefix=[ローカルに配置したパス（Assets/Subtree）] [リモートリポジトリ名] [サブツリーのブランチ名]
```

--- 

### ライセンス

* [The MIT License (MIT)](LICENSE)


### 使用ライブラリ

#### [Json.NET](http://www.newtonsoft.com/json)

> The MIT License (MIT)
> 
> Copyright (c) 2007 James Newton-King

* **ライセンス :** The MIT License (MIT)
* **ライセンス全文 :** [Licenses/Json.NET.txt](Licenses/Json.NET.txt)

#### [UniRx](https://github.com/neuecc/UniRx)

> The MIT License (MIT)
> 
> Copyright (c) 2014 Yoshifumi Kawai

* **ライセンス :** The MIT License (MIT)
* **ライセンス全文 :** [Licenses/UniRx.txt](Licenses/UniRx.txt)

#### [websocket-sharp](https://github.com/neuecc/UniRx)

> The MIT License (MIT)
> 
> Copyright (c) 2010-2017 sta.blockhead

* **ライセンス :** The MIT License (MIT)
* **ライセンス全文 :** [Licenses/websocket-sharp.txt](Licenses/websocket-sharp.txt)
