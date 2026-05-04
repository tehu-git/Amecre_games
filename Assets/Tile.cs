using UnityEngine;
using UnityEngine.UI; // Imageの操作に必要になります
using TMPro;

public class Tile : MonoBehaviour
{
    public TextMeshProUGUI numberText;
    public Image backgroundImage; // 【追加】タイルの背景色を変えるための変数
    public Vector3 targetPosition;
    private int tileValue;

    public void Setup(int initialValue, Vector3 startPos)
    {
        tileValue = initialValue;
        numberText.text = tileValue.ToString();
        transform.position = startPos;
        targetPosition = startPos;
        
        UpdateAppearance(); // 生成された時に色を反映
    }

    public void UpdateValue(int newValue)
    {
        tileValue = newValue;
        numberText.text = tileValue.ToString();
        
        UpdateAppearance(); // 合体して数字が変わった時に色を反映
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);
    }

    // 【追加】数字に応じてタイルの色と文字色を変える処理
    void UpdateAppearance()
    {
        Color bgColor;
        Color textColor = Color.white; // 基本の文字色は白にする

        switch (tileValue)
        {
            case 2:
                ColorUtility.TryParseHtmlString("#EEE4DA", out bgColor);
                textColor = new Color(0.47f, 0.43f, 0.4f); // 2と4は文字色を暗い茶色に
                break;
            case 4:
                ColorUtility.TryParseHtmlString("#EDE0C8", out bgColor);
                textColor = new Color(0.47f, 0.43f, 0.4f);
                break;
            case 8:
                ColorUtility.TryParseHtmlString("#F2B179", out bgColor);
                break;
            case 16:
                ColorUtility.TryParseHtmlString("#F59563", out bgColor);
                break;
            case 32:
                ColorUtility.TryParseHtmlString("#F67C5F", out bgColor);
                break;
            case 64:
                ColorUtility.TryParseHtmlString("#F65E3B", out bgColor);
                break;
            case 128:
                ColorUtility.TryParseHtmlString("#EDCF72", out bgColor);
                break;
            case 256:
                ColorUtility.TryParseHtmlString("#EDCC61", out bgColor);
                break;
            case 512:
                ColorUtility.TryParseHtmlString("#EDC850", out bgColor);
                break;
            case 1024:
                ColorUtility.TryParseHtmlString("#EDC53F", out bgColor);
                break;
            case 2048:
                ColorUtility.TryParseHtmlString("#EDC22E", out bgColor);
                break;
            default:
                ColorUtility.TryParseHtmlString("#3C3A32", out bgColor); // 4096以上のスーパーカラー
                break;
        }

        // 決定した色をImageとTextに適用する
        backgroundImage.color = bgColor;
        numberText.color = textColor;
    }
}