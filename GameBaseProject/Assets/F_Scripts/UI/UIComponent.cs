using UnityEngine;
using System.Collections;

public interface IUIComponentContainer
{
    void SetUIComponent<T>(T Comp) where T : UIComponent<T>;
}

public class UIComponent<T> :UIComponentBase where T:UIComponent<T> {

}
