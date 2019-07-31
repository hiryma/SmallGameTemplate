# SmallGameTemplate

小さいゲームを作る時のテンプレート。
「小さい」の定義はだいたい以下。

- シーンは基本MainSceneだけ。
    - アーティストからシーンの形で背景モデルをもらった場合はAdditiveで加える
- AssetBundleは用いない。関連機能は入れない。
- サーバとの通信は基本行わない。

## デバグ機能

- 原則デバグ機能はDebugServiceに入れる
- slackに投げる機能
- DebugUi
- アスペクト比エミュレーション
    - 例えばiPad上でiPhoneXの画面比率で描画する。
- デバグ機能は隠しコマンドによって有効になり、developent_buildでなくても入ったままになる。
    - リリース時に完全に切断したい場合はMain.csにおいてDEBUG_ENABLEDをコメントアウトする。

## シーン遷移設計

- SubSceneを継承するクラスと、それをルートのgameObjectにつけたprefabをセットで用意する。
- SubSceneのprefabはResources以下に置く。
- 初期化にはManualStart、更新にはManualUpdateを用いる。
    - ManualUpdateはシーン遷移が発生する時には次のシーン名をout引数で返す。
