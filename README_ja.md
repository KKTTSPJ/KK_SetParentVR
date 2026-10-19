[English](README.md) | 日本語

**注意**: このプロジェクトは、私が作ったものではない既存のプラグインを逆コンパイルし、改良したものです。
元のプラグインは http://uppervolta-3d.net/illu/upload.php で配布されていました。

# KK_SetParentVR
> コイカツ！公式 VR で、キャラクターの位置・姿勢・アニメーションの速度をコントローラーで操作します。
> *体位が足りなければ、その場で作ればいい*

**このフォークについて**: [MayouKurayami/KK_SetParentVR](https://github.com/MayouKurayami/KK_SetParentVR) 2.1.8 のフォークです。BepInEx 5 と、コイカツパーティ（Steam 版）の VR で動きます。
GUID は元のまま（*MK.KK_SetParentVR*）なので、元のプラグインと置き換えて使います。変更点は *CHANGES FROM ORIGINAL VERSION (0.9b).md* の 2.1.9 にあります。

この日本語版は英語の [README.md](README.md) を訳したものです。内容が食い違うときは英語版を正としてください。


## 必要なもの
- アフタースクール（追加ディスク）
- [コイカツ！公式 VR パッチ 0531](http://www.illusion.jp/preview/koikatu/download/vr.php)、またはコイカツパーティ（Steam 版）の VR
- BepInEx 5（BepInEx 4 Patcher はもう要りません）
- （任意、推奨）[CrossFader](https://github.com/MayouKurayami/KK_CrossFader/releases) プラグイン
- （任意、強く推奨）[KoikatuVRAssistPlugin](https://mega.nz/#!YQZyWRwQ!C2FX0Iwp-X7F5z55ytTlQGkjfqH6kQP-wcDPfNBvT0s) プラグイン


## 導入
- [最新のリリースから **KK_SetParentVR の zip** をダウンロード](https://github.com/KKTTSPJ/KK_SetParentVR/releases)し、ゲームのフォルダ（ゲームの exe と BepInEx フォルダがある場所）に展開します。上書きを聞かれたら置き換えてください。
  *KK_SetParentVR.dll* が *BepInEx/plugins* に入れば正しい配置です。

- 古い BepInEx 4 版（*BepInEx/KK_SetParentVR.dll*）が入っていれば消してください。同じ GUID なので、どちらか一方しか読み込まれません。

- 古い IPA 版（*SetParentKK.dll*）が入っていれば消してください。

- **（任意）** 浮かぶメニューの文字を翻訳するには、[AutoTranslator](https://github.com/bbepis/XUnity.AutoTranslator/releases) を入れます（HF Patch に含まれています）。
  そのうえで *BepInEx/config/AutoTranslatorConfig.ini* の [*TextFrameworks*] で **EnableUGUI** を **True** にします。

- アニメーションや体位の切り替えを滑らかにするため、[**CrossFader**](https://github.com/MayouKurayami/KK_CrossFader/releases) プラグインの導入を勧めます。

- https://vr-erogamer.com/archives/322 で配布されている [**KoikatuVRAssistPlugin**](https://mega.nz/#!YQZyWRwQ!C2FX0Iwp-X7F5z55ytTlQGkjfqH6kQP-wcDPfNBvT0s) も**強く推奨**します。VR での移動や操作が楽になります。
  - 入れると、トリガーを押している間はいつでもカメラを回転・移動できます。
  - UI のメニューが出ている間にグリップを 1 秒押すと、メニューをその場に固定できます。グリップを押したままにするとメニューを動かせます。グリップをダブルクリックすると、メニューがコントローラーに戻ります。

- 他の入力を自分で確かめない KoikatuVRAssistPlugin の版（例: KoikatuVRAssistPlugin改 1.2.0.9）にも対応しています。SetParent がボタンを使っている間、このプラグインは次のことをします。
  - グリップの長押しでゲームのメニューが出ないようにする。
  - メニューが浮いている間、スティックで速度が変わらないようにする。
  - KoikatuVRAssistPlugin のタッチイベントを止める。ボタンを離して 0.5 秒たつと、タッチイベントは元に戻ります。

  ほかのボタン（例: メニューボタンや、*Disable Input of Parent Controller* が無効のときの親のコントローラー）で起きるタッチイベントは止めません。邪魔になる場合は、KoikatuVRAssistPlugin の設定でそのタッチイベントを 0（なし）にしてください。

## 使い方
### [日本語の説明動画（NSFW）](https://mega.nz/file/RPpCDKIa#RRb_kYaRrrIqtDyojm3U2BVU_0dgin7B5nNCzXTvVQo)

- **メニューボタン（Rift では B か Y）を 1 秒以上**長押しすると、浮かぶ補助メニューを表示・非表示にできます。

- SetParent の有効・無効は、浮かぶメニューのボタン、**メニューボタン（Rift では B か Y）**と**トリガー**の同時押し、または設定したキーボードショートカットで切り替えます。
  - ボタンを同時押ししなかった方のコントローラーで、女の子を操作するようになります。
  - 設定によって、女の子の位置とピストンの速度の両方または片方が、そのコントローラーに合わせて動きます。

- 女の子の手足に触れるとつかめます（設定で無効にできます）。**トリガー**を押すと、その位置のまま手足を放します。浮かぶメニューのボタンで、手足ごとに位置を固定することもできます。

- 手や足の近くで**トリガーをダブルクリック**すると、その手足が元の位置に戻ります。**どの手足にも近くないところでトリガーをダブルクリック**すると、*すべての*手足が元の位置に戻ります。手足が変なところに引っかかったときにとても便利です。

- **グリップを押したままトリガーをダブルクリック**すると、男の足をその場に固定・解除します。

- SetParent が有効な間は、次の操作ができます。
  - 浮かぶメニューが非表示のときは、どちらかのコントローラーを頭に近づけると一時的に表示されます（設定で既定で有効）。
  - **グリップ**と**トラックパッド（スティック）の*下***を同時に押すと、男の位置を調整できます。性器のアニメーションのずれやめり込みを直すときに役立ちます。
  - **グリップ**と**トラックパッド（スティック）の*上***を同時に押すと、親でない方のコントローラーで女の子の位置を一時的に調整できます。
  - **グリップ**と**トリガー**を同時に押すと、**両方の**キャラクターを連れたままカメラを動かせます。
  - 設定で有効にすると、女の子の手足と男の足が、触れた物にくっつきます。**女の子の手足は、あなたの肩にもくっつきます**。
  - 一定以上引っ張られると、手足は物から自動で離れます。*注: コントローラーや浮かぶメニューで手動で固定した手足は、自動では離れません。*

- ゲームのメニューで体位や H のカテゴリを選び、ゲームのメニューか浮かぶメニューで挿入します。挿入後は、女の子が親子付けされたコントローラーを動かすと動きの速さを操作できます。
  - コントローラーを設定した範囲より大きく動かすと、弱い動きから強い動きに切り替わります。
  - 設定したしきい値の範囲内の動きを **1.5 秒**続けると、強い動きから弱い動きに戻ります。

- 左の浮かぶメニューの体位ボタンは、ゲームのメニューでの体位変更の代わりでは**ありません**。キャラクターの位置や向きは変えず、女の子の**姿勢**だけを変えるためのものです。

- 設定で有効にすると、女の子の興奮ゲージが 70 を超えた状態を設定した秒数続けると、自動で絶頂します。

- 誤操作を防ぐため、親のコントローラーの入力は無効になります（設定で有効にできます）。同じく、親のコントローラーの 3D モデルは非表示になります（AnimationOnly モードでは、設定で表示にできます）。

<br>

## 設定
設定は *BepInEx/config/MK.KK_SetParentVR.cfg* にあります。BepInEx 4 版の設定（*BepInEx/config.ini*）は引き継がれません。

***設定ファイルを直接書き換えるより、ゲーム内のプラグイン設定画面（VR の外で開けます）で変えることを勧めます。***

- **Autofinish Timer (Finishcount)** - 女の子の興奮ゲージが絶頂のしきい値（70）を超えている状態がこの秒数続くと、絶頂します。**0 にすると無効** **（既定: 0）**

- **Control Mode (SetParentMode)** - コントローラーで女の子の位置・アニメーション・その両方のどれを操作するか。**（既定: PositionAndAnimation）**

- **Disable Input of Parent Controller (DisableParentInput)** - 有効にすると、誤操作を防ぐため親のコントローラーの入力を無効にします。**（既定: True）**

- **Distance to Detach Female Arms (StretchLimitArms)** - 物にくっついている腕がこの距離以上引っ張られると離れます。単位はおよそメートル。浮かぶメニューのボタンやコントローラーで手動で固定した腕には効きません。**0 にすると、手が物に自動でくっつかなくなります** **（既定: 0.5）**

- **Distance to Detach Female Legs (StretchLimitLegs)** - 物にくっついている脚がこの距離以上引っ張られると離れます。単位はおよそメートル。浮かぶメニューのボタンやコントローラーで手動で固定した脚には効きません。**0 にすると、脚が物に自動でくっつかなくなります** **（既定: 0.7）**

- **Distance to Display Hidden Menu (MenuUpProximity)** - 浮かぶメニューが非表示のとき、コントローラーをヘッドセットにこの距離まで近づけると一時的に表示します。単位はおよそメートル。0 にすると無効 **（既定: 0.25）**

- **Enable Holding Girl's Limbs with Controllers (SetControllerCollider)** - 有効にすると、コントローラーで女の子の手足に触れたとき、その手足がコントローラーにくっつきます。**（既定: True）**

- **Gaze Control** - メニューの項目を 1 秒以上見つめると選択する機能の有効・無効。**（既定: False）**

- **Groping Hands Display Mode (GropeHandsDisplay)** - 男の手をコントローラーに合わせているとき、女の子の体に触れたら追加の手を表示するか。Auto にすると、手の近さに応じて自動で隠します。**（既定: Auto）**

- **Hide Floating Menu by Default (MenuHideDefault)** - 浮かぶメニューを最初は隠すか。メニュー（A/B）ボタンを 1 秒以上押すか、SetParent が有効なときにコントローラーをヘッドセットに近づけると表示されます。**（既定: True）**

- **Hide Parent Controller in Animation Only Mode (HideParentConAlways)** - AnimationOnly モードでも親のコントローラーを隠します。無効にすると、AnimationOnly モードでは表示し、他のモードでは隠します。**（既定: True）**

- **Make Male's Feet Stick to Objects (SetMaleFeetCollider)** - 有効にすると、男の足が触れた物に自動でくっつきます。男の足が宙に浮いたり床にめり込んだりするのを防ぎます。**（既定: True）**

- **Synchronize Male's Hands with Controllers (SyncMaleHands)** - 有効にすると、SetParentVR を有効にしたとき、男の手と腕を既定でコントローラーに合わせます。男の手が変な位置にあるときに便利です。浮かぶメニューで切り替えられます。**（既定: True）**

- **Synchronize Male's Head with Headset (SetParentMale)** - 有効にすると、男の頭がヘッドセットに合うように男の体を回します。男の頭が目の前に来なくなります。**（既定: True）**

- **Which Controller Controls Animation (CalcController)** - アニメーションの速度と弱い・強い動きの切り替えに使うコントローラー。**（既定: SetParentController）**

### Keyboard Shortcuts
- **Limb Release** - このキーを押すと、すべての手足の固定を解除します。**（既定: なし）**

- **Male Feet Fix/Release** - このキーを押すと、男の両足をその場に固定・解除します。**グリップを押したままトリガーをダブルクリック**でも切り替えられます。**（既定: なし）**

- **SetParent Toggle** - このキーを押すと、左のコントローラーを親として SetParentVR を有効・無効にします。**（既定: なし）**

<br>

### Advanced Settings
（内部の設定です。慎重に変えてください）
- **Animation Max Threshold** - コントローラーの動きの量がこの値に達すると、アニメーションの速度が最大になります。単位はおよそメートル。**（既定: 0.23）**

- **Animation Start Threshold** - コントローラーの動きの量がこのしきい値を超えると、ピストンのアニメーションが始まります。単位はおよそメートル。**（既定: 0.02）**

- **Controller Average Position Pool Size (MoveCoordinatePoolSize)** - コントローラーの位置を、このフレーム数の平均で計算します。コントローラーの動きの範囲の計算に使います。**（既定: 30）**

- **Controller Moved Distance Pool Size (MoveDistancePoolSize)** - コントローラーの動きの量を、このフレーム数の移動距離の合計で計算します。**（既定: 55）**

- **Diagnostic Log (DiagnosticLog)** - SetParent の動作（有効化、操作の状態、浮かぶメニューのボタン、手足、動きの切り替え）を *output_log.txt* に出し、ログで動作を確かめられるようにします。**（既定: False）**

- **Male Yaw Rotation (MaleYaw)** - 男の同期が有効なとき、男の体の左右の回転（ヨー）を有効にするか。**（既定: True）**

- **Part of Girl's Body to Parent with The Controller (ParentPart)** - コントローラーの動きと回転に合わせる中心にする、女の子の体の部位。**（既定: Ass）**

- **Smooth Tracking (TrackingMode)** - 女の子の体がコントローラーに滑らかについてくるようにします。無効にすると、すぐにそのままついてきます。**（既定: True）**

- **Strong Motion Speed Maximum Multiplier (StrongThresholdMultiplier)** - 強い動きでは、動きの範囲が広くて最大速度に達しやすいため、速度の最大のしきい値にこの数を掛けます。**（既定: 1.1）**

- **Strong Motion Threshold** - *MoveCoordinatePoolSize* で計算したコントローラーの動きの範囲がこのしきい値を超えると、強い動きに切り替わります。単位はおよそメートル。**（既定: 0.04）**

- **Weak Motion Threshold** - コントローラーの動きの範囲がこのしきい値の中に **1.5 秒**とどまると、弱い動きに切り替わります。単位はおよそメートル。**（既定: 0.03）**

<br>

## 注意と制限
- コイカツパーティ（Steam 版）の VR で動きます。

- kPlug: **kPlug VR との併用は推奨しません。** 次の未解決の問題があります。
  - kPlug VR で SetParentVR を使っている間に、キャラクターが白くなることがありました（女の子が消えることもありました）。原因はまだ分かっていません。
  - kPlug の追加の体位で、男の位置の調整（グリップ＋トラックパッド下）や両方のキャラクターを連れた移動（グリップ＋トリガー）をすると、kPlug で例外が起き、H シーンが絶頂を繰り返したまま進まなくなることがあります。

- 本編の非公式 VR MOD では今のところ動きません。

- 動きの間でアニメーションの差が大きい体位（例: 騎乗位）とは相性がよくありません。
  回避策として、浮かぶメニューで **Animation Only Mode** に切り替えるか、強い・弱い動きの差が小さい同じ体位の別の版に切り替えてください。

- 左の浮かぶメニューの体位ボタンは、ゲームのメニューでの体位変更の代わりでは**ありません**。キャラクターの位置や向きは変えず、女の子の**姿勢**だけを変えるためのものです。

- 浮かぶメニューで女の子の体位を変えても、男の位置は変わりません。そのため、男の手が不自然な位置になることがあります。男の手をコントローラーに合わせる設定で和らげられます。

- 同じく、床の近くで立ちの体位を使うと、男の足が床にめり込みやすくなります。男の足のコライダーの設定で少しは和らぎます。男の足を好きな位置に置いてから、コントローラーかキーボードショートカットで固定することもできます。

- 3P では機能が限られます。今のところ操作できるのは最初の女の子の位置だけで、2 人目の女の子の位置や姿勢は操作できません。

- 男の同期が有効なとき、男の体の影が気になることがあります。[KK_HSceneOptions](https://github.com/MayouKurayami/KK_HSceneOptions) で消せます。

- SetParentVR でアニメーションの速度を操作していると、ピストンの速度が絶えず変わるため、女の子がセリフを話さなくなることがあります。[KK_HSceneOptions](https://github.com/MayouKurayami/KK_HSceneOptions) の **Speech Control** で回避できます。

<br>

## クレジット
バージョン 0.9b までのプラグインの功績は、すべてこのプラグインを作った作者不明の開発者にあります。

## 免責
このソフトウェアとその開発者は、使いすぎによるハードウェアの損傷やデータの消失について責任を負いません。
