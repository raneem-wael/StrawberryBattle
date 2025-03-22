using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
public class JsonBasics : MonoBehaviour
{
    [SerializeField] private UnityEngine.Object jsonString;
    [SerializeField] private TextMeshProUGUI jsonText;
    public void DisplayJson()
    {
        jsonText.text = jsonString.ToString();
    }
    public void DisplayJsonValue(){
        JObject jsonObject = JObject.Parse(jsonString.ToString());
        string value = jsonObject["user"]["firstName"].ToString();
        //string value = jsonObject["statistics"]["totalLogins"].ToString();
        jsonText.text = value;
    }
}
