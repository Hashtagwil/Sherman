using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonColorSelection : MonoBehaviour
{
    public TankController TankController;
    [SerializeField] Color Color = Color.white;

    private TankController lastTankController = null;
    private Color lastColor = Color.white;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        if (TankController != null && lastTankController != TankController)
        {
            TankController.SelectBody();
            lastTankController = TankController;
        }
        if (lastColor != Color)
        {
            GetComponentInChildren<Image>().color = Color;
        }
    }

    public void OnClick()
    {
        if (TankController != null)
        {
            TankController.SetColorOfSelectedPart(Color);
        }
    }
}
