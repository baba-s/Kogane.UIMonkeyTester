# UniUIMonkeyTester

UI のモンキーテストを行うためのクラス

## 使用例

```cs
using Kogane;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class Example : MonoBehaviour
{
    // UI のモンキーテストを行うためのインスタンス
    private UIMonkeyTester m_tester;

    private void Awake()
    {
        // UI のモンキーテストを行うためのインスタンスを作成
        m_tester = new UIMonkeyTester
        (
            // デバッグメニューの UI はクリックしないようにする（省略可）
            gameObjectFilter: gameObject => gameObject.scene.name != "DebugMenuScene",

            // UI をクリックする処理を設定（省略可）
            onClick: ( handler, data ) =>
            {
                var pointerClickHandler = handler as IPointerClickHandler;
                pointerClickHandler?.OnPointerClick( data );
            }
        );

        // 常にモンキーテストするために DontDestroyOnLoad 実行
        DontDestroyOnLoad( gameObject );
    }

    private void Update()
    {
        // 毎フレームランダムに画面をクリック
        var x = Random.value * Screen.width;
        var y = Random.value * Screen.height;

        m_tester.Click( x, y );
    }
}
```
