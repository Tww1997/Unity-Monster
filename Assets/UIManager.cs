using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public GameObject m_FadePanelGO;
    public FadePanel m_FadePanel;

    public GameObject m_ExitBtn;

    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            else
            {
                new UIManager();
                return _instance;
            }
        }
    }
   
    private UIManager()
    {
        _instance = this;
    }

	// Use this for initialization
	void Start () {
        m_FadePanelGO = Instantiate(Resources.Load("UI/FadePanel")) as GameObject;
        
        m_ExitBtn = Instantiate(Resources.Load("UI/ExitBtn")) as GameObject;
        
        m_FadePanel = m_FadePanelGO.GetComponent<FadePanel>();

        UIInit(m_FadePanelGO,this.transform);
        UIInit(m_ExitBtn, this.gameObject.transform);
    }

    void UIInit(GameObject go,Transform parent)
    {
        go.transform.parent = parent;
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = Vector3.zero;
        go.transform.localScale = Vector3.one;
    }

}
