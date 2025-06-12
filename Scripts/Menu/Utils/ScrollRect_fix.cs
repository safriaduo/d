using UnityEngine;
using UnityEngine.UI;
public class ScrollRect_fix : ScrollRect
{

    [SerializeField] private float setSize = .5f;
    private float horizontalBaseResolution = 1920;
    private float horizontalRatio=1;



    override protected void LateUpdate() {
        base.LateUpdate();
        if (this.horizontalScrollbar) {
            this.horizontalScrollbar.size=setSize*horizontalRatio;
        }
    }
    
    override public void Rebuild(CanvasUpdate executing) {
        base.Rebuild(executing);
        if (this.horizontalScrollbar) {
            this.horizontalScrollbar.size=setSize*horizontalRatio;
        }
    }
}